using LinkeD365.FlowAdmin.Properties;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowAdmin
{
    public partial class AdminControl : PluginControlBase
    {
        private SortableBindingList<FlowDefinition> flows;
        private FlowDefinition selectedFlow;
        private HttpClient flowClient;
        private HttpClient graphClient;
        private APIConn flowConn;
        private APIConn graphConn;
        private APIConns aPIConnections;
        private bool fromAPIButton;

        public string nextLinkUrl { get; private set; }

        private void Connect(bool Graph = false)
        {
            if (!Graph && flowClient != null) { return; }
            if (Graph && graphClient != null) { return; }

            if (!fromAPIButton && ((CrmServiceClient)Service).ActiveAuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth)
            {
                CrmServiceClient crmService = Service as CrmServiceClient;
                if (Graph)
                {
                    graphConn = new GraphConn { AppId = ConnectionDetail.AzureAdAppId.ToString(), TenantId = crmService.TenantId.ToString(), ReturnURL = ConnectionDetail.ReplyUrl };
                    graphClient = new ApiConnection(graphConn, true).Connect();
                }
                else
                {
                    flowConn = new FlowConn { AppId = ConnectionDetail.AzureAdAppId.ToString(), TenantId = crmService.TenantId.ToString(), ReturnURL = ConnectionDetail.ReplyUrl, Environment = crmService.EnvironmentId };
                    flowClient = new ApiConnection(flowConn, false).Connect();
                }
                fromAPIButton = false;
            }
            else
            {
                ApiConnection apiConnection = new ApiConnection(aPIConnections, Graph);
                try
                {
                    if (Graph)
                    {
                        graphClient = apiConnection.GetClient();
                        if (graphClient != null)
                            graphConn = apiConnection.graphConn;
                        else
                            return;

                        graphConn = apiConnection.graphConn;
                    }
                    else
                    {
                        flowClient = apiConnection.GetClient();
                        if (flowClient != null)
                        {
                            flowConn = apiConnection.flowConn;
                        }
                        else return;
                    }
                }
                catch (AdalServiceException adalExec)
                {
                    LogError("Adal Error", adalExec.GetBaseException());

                    if (adalExec.ErrorCode == "authentication_canceled")
                    {
                        return;
                    }

                    // ShowError(adalExec, "Error in
                    // connecting, please check details");
                }
                catch (Exception e)
                {
                    LogError("Error getting connection", e.Message);
                    // ShowError(e, "Error in connecting,
                    // please check entered details");
                    return;
                }
            }
        }

        #region Load Flows from Dataverse

        public void LoadFlowsFromDV()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Flows",
                Work = (w, e) =>
                {
                    var qe = new QueryExpression("workflow");
                    qe.ColumnSet.AddColumns("ismanaged", "clientdata", "description", "name", "createdon", "modifiedon", "modifiedby", "createdby", "workflowidunique", "ownerid");
                    qe.Criteria.AddCondition("category", ConditionOperator.Equal, 5);
                    var su = qe.AddLink("systemuser", "ownerid", "systemuserid");
                    su.EntityAlias = "su";

                    // Add columns to su.Columns
                    su.Columns.AddColumns("azureactivedirectoryobjectid");

                    SortableBindingList<FlowDefinition> flowList = new SortableBindingList<FlowDefinition>();

                    var flowRecords = Service.RetrieveMultiple(qe);
                    foreach (var flowRecord in flowRecords.Entities)
                    {
                        flowList.Add(new FlowDefinition
                        {
                            Id = flowRecord["workflowid"].ToString(),
                            Name = flowRecord["name"].ToString(),
                            Definition = flowRecord["clientdata"].ToString(),
                            Description = !flowRecord.Attributes.Contains("description") ? string.Empty : flowRecord["description"].ToString(),
                            Solution = true,
                            Managed = (bool)flowRecord["ismanaged"],
                            UniqueId = flowRecord["workflowidunique"].ToString(),
                            OwnerId = ((EntityReference)flowRecord["ownerid"]).Id.ToString(),

                            AzureOwnerId = !flowRecord.Attributes.Contains("su.azureactivedirectoryobjectid") ? string.Empty : ((AliasedValue)flowRecord["su.azureactivedirectoryobjectid"]).Value.ToString(),

                            CreatedOn = (DateTime)flowRecord["createdon"],
                            Modified = (DateTime)flowRecord["modifiedon"],
                        });
                    }

                    e.Result = flowList;
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(e.Error.Message);
                        return;
                    }
                    flows = e.Result as SortableBindingList<FlowDefinition>;
                    InitGrid();
                    //if (flows.Any())
                    //{
                    //    flows = new SortableBindingList<FlowDefinition>( returnFlows);
                    //    gridFlows.DataSource = flows;
                    //    //SortGrid("Name", SortOrder.Ascending);
                    //}

                    btnConnectDataverse.Visible = !flows.Any();

                    btnConnectFlow.Visible = flows.Any();

                    //flowRecords = (EntityCollection)e.Result;
                    //gridFlows.DataSource = flowRecords;
                },
            });
        }

        private void LoadSolutionsFromDV()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving the Solutions",
                Work = (w, e) =>
                {
                    QueryExpression solQry = new QueryExpression("solution");
                    solQry.Distinct = true;
                    solQry.ColumnSet = new ColumnSet("friendlyname", "version", "publisherid", "solutionid");
                    solQry.AddOrder("friendlyname", OrderType.Ascending);
                    solQry.Criteria = new FilterExpression();
                    solQry.Criteria.AddCondition(new ConditionExpression("isvisible", ConditionOperator.Equal, true));
                    solQry.Criteria.AddCondition(new ConditionExpression("uniquename", ConditionOperator.NotEqual, "Default"));
                    List<Solution> solList = new List<Solution>();

                    var solutionRows = Service.RetrieveMultiple(solQry);
                    foreach (var solution in solutionRows.Entities)
                    {
                        solList.Add(new Solution
                        {
                            Id = solution["solutionid"].ToString(),
                            Name = solution["friendlyname"].ToString(),
                            Publisher = ((EntityReference)solution["publisherid"]).Name
                        });
                    }

                    e.Result = solList;
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var solList = e.Result as List<Solution>;
                    splitTop.Panel2Collapsed = !solList.Any();

                    solList.Insert(0, new Solution() { Name = "Filter on Solution" });
                    ddlSolutions.DataSource = solList;
                    ddlSolutions.DisplayMember = "Name";
                },
            });
        }

        private void GetFlowsForSolution()
        {
            if (ddlSolutions.SelectedIndex == 0) { LoadFlowsFromDV(); return; }
            string solId = ((Solution)ddlSolutions.SelectedItem).Id;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Flows for Solution",
                Work = (w, e) =>
                {
                    var qe = new QueryExpression("workflow");
                    qe.ColumnSet.AddColumns("ismanaged", "clientdata", "description", "name", "createdon", "modifiedon", "modifiedby", "createdby", "workflowidunique", "ownerid");
                    qe.AddOrder("name", OrderType.Ascending);
                    qe.Criteria.AddCondition("category", ConditionOperator.Equal, 5);
                    var solComp = qe.AddLink("solutioncomponent", "workflowid", "objectid", JoinOperator.Inner);
                    solComp.EntityAlias = "solComp";
                    var sol = solComp.AddLink("solution", "solutionid", "solutionid");
                    sol.EntityAlias = "sol";
                    sol.LinkCriteria.AddCondition("solutionid", ConditionOperator.Equal, solId);
                    var su = qe.AddLink("systemuser", "ownerid", "systemuserid");
                    su.EntityAlias = "su";

                    // Add columns to su.Columns
                    su.Columns.AddColumns("azureactivedirectoryobjectid");
                    SortableBindingList<FlowDefinition> flowList = new SortableBindingList<FlowDefinition>();

                    var flowRecords = Service.RetrieveMultiple(qe);
                    foreach (var flowRecord in flowRecords.Entities)
                    {
                        flowList.Add(new FlowDefinition
                        {
                            Id = flowRecord["workflowid"].ToString(),
                            Name = flowRecord["name"].ToString(),
                            Definition = flowRecord["clientdata"].ToString(),
                            Description = !flowRecord.Attributes.Contains("description") ? string.Empty : flowRecord["description"].ToString(),
                            Solution = true,
                            Managed = (bool)flowRecord["ismanaged"],
                            AzureOwnerId = !flowRecord.Attributes.Contains("su.azureactivedirectoryobjectid") ? string.Empty : ((AliasedValue)flowRecord["su.azureactivedirectoryobjectid"]).Value.ToString(),

                            CreatedOn = (DateTime)flowRecord["createdon"],
                            Modified = (DateTime)flowRecord["modifiedon"],
                            UniqueId = flowRecord["workflowidunique"].ToString(),
                            OwnerId = ((EntityReference)flowRecord["ownerid"]).Id.ToString(),
                        });
                    }

                    e.Result = flowList;
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var returnFlows = e.Result as SortableBindingList<FlowDefinition>;
                    InitGrid();
                    flows = returnFlows;
                    gridFlows.SelectionChanged -= gridFlows_SelectionChanged;
                    gridFlows.DataSource = flows;
                    if (returnFlows.Any())
                    {
                        SortGrid("Name", SortOrder.Ascending);
                    }
                    gridFlows.ClearSelection();

                    gridFlows.SelectionChanged += gridFlows_SelectionChanged;
                    btnConnectDataverse.Visible = !returnFlows.Any();
                    btnConnectFlow.Visible = returnFlows.Any();
                },
            });
        }

        #endregion Load Flows from Dataverse

        #region Get Flows from API

        private void LoadUnSolutionedFlows()
        {
            if (flowConn == null) Connect();
            if (flowClient == null) return;

            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Loading Flows",
                    Work =
                        (w, args) => args.Result = GetAllFlows(w),

                    PostWorkCallBack =
                        args =>
                        {
                            if (args.Error != null)
                            {
                                ShowError(args.Error, "Error retrieving Flows via API");
                                return;
                            }

                            if (args.Result is SortableBindingList<FlowDefinition>)
                            {
                                flows = args.Result as SortableBindingList<FlowDefinition>;
                                InitGrid();
                                btnConnectDataverse.Visible = flows.Any();
                                btnConnectFlow.Visible = !flows.Any();
                                splitTop.Panel2Collapsed = true;
                            }
                            else ShowError(new Exception("Error retrieving Flows via API"), "Error retrieving Flows via API");
                        }
                }
            );
        }

        private SortableBindingList<FlowDefinition> GetAllFlows(BackgroundWorker w)
        {
            var flows = new SortableBindingList<FlowDefinition>();
            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows?$top=50&api-version=2016-11-01";
            flows = GetFlows(flows, url, w);
            url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows?$filter=search(%27team%27)&$top50&api-version=2016-11-01";
            flows = GetFlows(flows, url, w);
            Utils.Ai.WriteEvent("Flows Loaded", flows.Count);
            return flows;
        }

        private SortableBindingList<FlowDefinition> GetFlows(SortableBindingList<FlowDefinition> flows, string url, BackgroundWorker w)
        {
            HttpResponseMessage response = flowClient.GetAsync(url).GetAwaiter()
                   .GetResult();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var flowDefs = JObject.Parse(
                    response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (flowDefs["value"].HasValues)
                {
                    foreach (JToken flowDef in flowDefs["value"].Children())
                    {
                        flows.Add(
                            new FlowDefinition
                            {
                                Id = flowDef["name"].ToString(),
                                UniqueId = flowDef["name"].ToString(),
                                Solution = false,
                                Name = flowDef["properties"]["displayName"].ToString(),
                                OwnerType = flowDef["properties"]?["userType"]?.ToString() ?? string.Empty
                            });
                    }
                }
                if (flowDefs.GetValue("nextLink") != null)
                {
                    flows = GetFlows(flows, flowDefs["nextLink"].ToString(), w);
                }
                return flows;
            }
            else
            {
                LogError("Get Flows via API", response);
                ShowError(
                    $"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.",
                    "Get Flows via API error");
                return null;
            }
        }

        #endregion Get Flows from API

        private void GetFlowDetails()
        {
            if (flowConn == null) Connect();
            if (flowClient == null) return;
            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{selectedFlow.UniqueId}?api-version=2016-11-01";
            //flowRuns = new List<FlowRun>();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Flow Details",
                Work = (w, args) =>
                {
                    HttpResponseMessage response = flowClient.GetAsync(url).GetAwaiter().GetResult();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var flowDetail = JObject.Parse(
                                               response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                        args.Result = flowDetail;
                    }
                },
                PostWorkCallBack = args =>
                {
                    if (args?.Error != null)
                    {
                        // ShowError(args.Error, "Error
                        // retrieving Flow Runs via API");
                        return;
                    }
                    var flowDetail = args.Result as JObject;
                    selectedFlow.CreatedOn = ((DateTime)flowDetail["properties"]["createdTime"]).ToLocalTime();
                    selectedFlow.Modified = ((DateTime)flowDetail["properties"]["lastModifiedTime"]).ToLocalTime();
                    selectedFlow.Status = flowDetail["properties"]["state"].ToString();
                    selectedFlow.Plan = flowDetail["properties"]["userType"].ToString();
                    selectedFlow.AzureOwnerId = flowDetail["properties"]?["creator"]?["userId"]?.ToString() ?? string.Empty;
                    txtCreated.Text = selectedFlow.CreatedOn.ToString();
                    txtModified.Text = selectedFlow.Modified.ToString();
                    txtStatus.Text = selectedFlow.Status;
                    txtPlan.Text = selectedFlow.Plan;
                    btnDisable.Text = selectedFlow.Status == "Stopped" ? "Enable" : "Disable";
                    btnDisable.Image = selectedFlow.Status == "Stopped" ? Resources.Power_Green : Resources.Power_Red;
                    GetOwners();
                }
            });
        }

        private void GetOwners()
        {
            if (graphConn == null) Connect(true);
            if (graphClient == null) return;
            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);
            string ownersUrl = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{selectedFlow.UniqueId}/owners?api-version=2016-11-01";
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Flow Owners",
                Work = (w, args) =>
                {
                    HttpResponseMessage response = flowClient.GetAsync(ownersUrl).GetAwaiter().GetResult();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var owners = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                        var ownerList = new List<FlowOwner>();
                        foreach (var ownerJson in owners["value"])
                        {
                            var owner = new FlowOwner
                            {
                                Id = Guid.Parse(ownerJson["name"].ToString())
                            };

                            string ownerUrl = $"https://graph.microsoft.com/v1.0/users/{owner.Id}";
                            HttpResponseMessage ownerResponse = graphClient.GetAsync(ownerUrl).GetAwaiter().GetResult();

                            if (ownerResponse.StatusCode == HttpStatusCode.OK)
                            {
                                var ownerDetail = JObject.Parse(ownerResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                                owner.Name = ownerDetail["displayName"].ToString();
                                owner.Email = ownerDetail["mail"].ToString();
                                owner.Principle = ownerDetail["userPrincipalName"].ToString();
                            }
                            ownerList.Add(owner);
                        }

                        args.Result = ownerList;
                    }
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error retrieving Flow Owners via API");
                        return;
                    }
                    var ownerList = args.Result as List<FlowOwner>;
                    gridOwners.AutoGenerateColumns = false;
                    gridOwners.DataSource = ownerList;
                    SetupOwnersGrid();
                    foreach (DataGridViewRow row in gridOwners.Rows)
                    {
                        var owner = row.DataBoundItem as FlowOwner;
                        if (owner.Id.ToString() == selectedFlow.AzureOwnerId)
                        {
                            ((DataGridViewImageCell)row.Cells[2]).Value = Resources.bingrey;
                        }
                        else ((DataGridViewImageCell)row.Cells[2]).Value = Resources.bin;
                    }
                }
            });
        }

        private void SetupOwnersGrid()
        {
            if (gridOwners.Columns.Count > 1)
                return;

            var fieldName = new DataGridViewTextBoxColumn { DataPropertyName = "Name", Name = "Name" };
            gridOwners.Columns.Insert(0, fieldName);

            var emailField = new DataGridViewTextBoxColumn { DataPropertyName = "Email", Name = "Email" };
            gridOwners.Columns.Insert(1, emailField);

            var removeField = gridOwners.Columns[2] as DataGridViewImageColumn;
            removeField.DefaultCellStyle.NullValue = null;
        }

        private void GetAllRunning(bool cancelRuns)
        {
            Connect();
            nextLinkUrl = string.Empty;
            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{selectedFlow.UniqueId}/runs?&api-version=2016-11-01/runs&$filter=status eq 'running'";
            var flowRuns = new List<FlowRun>();
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Runs",
                Work = (w, args) => args.Result = GetAllRuns(url, w),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error retrieving Flow Runs via API");
                        return;
                    }

                    if (args.Result is List<FlowRun>)
                    {
                        flowRuns = args.Result as List<FlowRun>;
                        //url = nextLinkUrl;
                    }
                    if (!flowRuns.Any())
                    { MessageBox.Show("No running flows found", "No Flows", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                    if (cancelRuns)
                    {
                        if (MessageBox.Show("Cancel all " + flowRuns.Count + " runs of " + selectedFlow.Name + "?", "Cancel All Runs", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            CancelAllFlows(flowRuns);
                        }
                    }
                }
            });
        }

        private List<FlowRun> GetAllRuns(string url, BackgroundWorker w)
        {
            var flowRuns = new List<FlowRun>();
            HttpResponseMessage response = flowClient.GetAsync(url).GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var flowRunsJO = JObject.Parse(
                                       response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (flowRunsJO["value"].HasValues)
                {
                    foreach (JToken flowRunJO in flowRunsJO["value"].Children())
                    {
                        flowRuns.Add(
                            new FlowRun
                            {
                                Id = flowRunJO["name"].ToString(),// 2023-03-06T16:43:39.6247123Z
                                Start = (DateTime)flowRunJO["properties"]["startTime"],//  DateTime.ParseExact(flowRunJO["properties"]["startTime"].ToString(), "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                End = (DateTime?)flowRunJO["properties"]["endTime"],
                                Status = flowRunJO["properties"]["status"].ToString()
                            }); ; ;
                    }
                }

                if (flowRunsJO.GetValue("nextLink") != null)
                    flowRuns.AddRange(GetAllRuns(flowRunsJO.GetValue("nextLink").ToString(), w));
                return flowRuns;
            }
            else
            {
                LogError("Get Flow Runs via API", response);
                ShowError(
                            $"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.",
                            "Get Flow Runs via API error");
                return null;
            }
        }

        private void GetFirstFlowRuns(FlowDefinition flow, string option)
        {
            Connect();
            nextLinkUrl = string.Empty;
            //if (_client == null) return;
            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{flow.UniqueId}/runs?&api-version=2016-11-01";

            switch (option)
            {
                case "All":
                    url += "&$top=20";
                    break;

                case "Failed":
                    url += "&$filter=status eq 'failed'&$top=20";
                    break;

                case "Cancelled":
                    url += "&$filter=status eq 'cancelled'&$top=20";
                    break;

                case "Succeeded":
                    url += "&$filter=status eq 'succeeded'&$top=20";
                    break;

                case "Running":
                    url += "&$filter=status eq 'running'&$top=20";
                    break;
            }
            runsBS.Clear();
            GetRuns(url);

            // GetFlowRuns(url);
        }

        private void GetRuns(string url)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Runs",
                Work = (w, args) => args.Result = GetFlowRuns(url, w),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error retrieving Flow Runs via API");
                        return;
                    }

                    if (args.Result is List<FlowRun>)
                    {
                        var flowRuns = args.Result as List<FlowRun>;
                        runsBS.AddRange(flowRuns);
                        Utils.Ai.WriteEvent("Loaded Runs", flowRuns.Count);
                    }
                }
            });
            // return flowRuns;
        }

        private List<FlowRun> GetFlowRuns(string url, BackgroundWorker w)
        {
            var flowRuns = new List<FlowRun>();
            HttpResponseMessage response = flowClient.GetAsync(url).GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var flowRunsJO = JObject.Parse(
                                       response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (flowRunsJO["value"].HasValues)
                {
                    foreach (JToken flowRunJO in flowRunsJO["value"].Children())
                    {
                        flowRuns.Add(
                            new FlowRun
                            {
                                Id = flowRunJO["name"].ToString(),// 2023-03-06T16:43:39.6247123Z
                                Start = (DateTime)flowRunJO["properties"]["startTime"],//  DateTime.ParseExact(flowRunJO["properties"]["startTime"].ToString(), "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                End = (DateTime?)flowRunJO["properties"]["endTime"],
                                Status = flowRunJO["properties"]["status"].ToString()
                            }); ; ;
                    }
                }

                if (flowRunsJO.GetValue("nextLink") != null)
                    nextLinkUrl = flowRunsJO.GetValue("nextLink").ToString();
                else nextLinkUrl = string.Empty;
                return flowRuns;
                // return flows;
            }
            else
            {
                LogError("Get Flow Runs via API", response);
                ShowError(
                    $"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.",
                    "Get Flow Runs via API error");
                return null;
            }
        }

        private void CancelAllFlows(List<FlowRun> flowRuns)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Cancelling " + flowRuns.Count + " Flows for " + selectedFlow.Name,
                Work = (w, args) => args.Result = CancelFlows(flowRuns, w),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null) { ShowError(args.Error.Message, "Error"); }
                    else
                    {
                        List<FlowRun> returnFlows = args.Result as List<FlowRun>;
                        // DialogResult = DialogResult.Yes; this.Close();
                    }
                }
            });

            Utils.Ai.WriteEvent("Flow Runs Cancelled", flowRuns.Count);
        }

        private List<FlowRun> CancelFlows(List<FlowRun> flowRuns, BackgroundWorker w)
        {
            return flowRuns.Select(fr => CancelFlow(fr)).ToList();
        }

        private FlowRun CancelFlow(FlowRun flowRun)
        {
            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{selectedFlow.UniqueId}/runs/{flowRun.Id}/cancel?api-version=2016-11-01";
            flowRun.Message = flowClient.PostAsync(url, null).Result;
            return flowRun;
        }

        private void DisableEnableFlow()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = $"{(selectedFlow.Status == "Stopped" ? "Enabling" : "Disabling")} Flow " + selectedFlow.Name,
                Work = (w, args) =>
                {
                    if (!selectedFlow.Solution)
                    {
                        string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{selectedFlow.UniqueId}/{(selectedFlow.Status == "Stopped" ? "start" : "stop")}?api-version=2016-11-01";

                        HttpResponseMessage response = flowClient.PostAsync(url, null).GetAwaiter()
                   .GetResult();

                        if (response.StatusCode == HttpStatusCode.OK)
                            return;

                        LogError("Disable Flow via API", response);
                        ShowError(
                            $"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.",
                            "Get Flows via API error");
                    }
                    else
                    {
                        Entity workFlow = new Entity("workflow", Guid.Parse(selectedFlow.Id));
                        workFlow["statecode"] = new OptionSetValue(selectedFlow.Status == "Stopped" ? 1 : 0);
                        workFlow["statuscode"] = new OptionSetValue(selectedFlow.Status == "Stopped" ? 2 : 1);
                        Service.Update(workFlow);
                    }
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null) { ShowError(args.Error.Message, "Error"); }
                    else
                    {
                    }
                    GetFlowDetails();
                }
            });
            Utils.Ai.WriteEvent("Flow En/Disabled");
        }

        private void RemoveOwnerDV(FlowOwner owner)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Removing Owner " + owner.Name,
                Work = (w, e) =>
                {
                    List<OrganizationResponse> responses = new List<OrganizationResponse>();
                    OrganizationRequest orgReq = new OrganizationRequest();
                    orgReq.RequestName = "RevokeAccess";
                    orgReq.Parameters = new ParameterCollection();
                    EntityReference target = new EntityReference("workflow", selectedFlow.Guid);
                    orgReq.Parameters.Add("Target", target);

                    if (owner.SysId == Guid.Empty)
                    {
                        var query = new QueryExpression("systemuser");

                        query.ColumnSet.AddColumn("systemuserid");

                        // Add conditions to query.Criteria
                        query.Criteria.AddCondition("azureactivedirectoryobjectid", ConditionOperator.Equal, owner.Id);

                        EntityCollection userResp = Service.RetrieveMultiple(query);
                        if (userResp.Entities.Count == 0)
                        {
                            e.Result = "User not found in Dataverse";
                            w.CancelAsync();
                        }
                        owner.SysId = userResp.Entities[0].Id;
                    }
                    EntityReference revokee = new EntityReference("systemuser", owner.SysId);

                    orgReq.Parameters.Add("Revokee", revokee);
                    responses.Add(Service.Execute(orgReq));

                    if (e.Result == null) e.Result = responses;
                    //rgReq.Parameters.Add("Target", new EntityReference("flow", selectedFlow.Guid));
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(this, "Error while removing the owner: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (e.Result.GetType() == typeof(string))
                    {
                        MessageBox.Show(this, "Error while updating the owner(s): " + e.Result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(this, "Owner removed successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                },
            });
            GetFlowDetails();

            Utils.Ai.WriteEvent("Flow Owner Removed", 1);
        }

        private void UpdateOwnerAPI(List<FlowOwner> owners)
        {
            if (flowConn == null) Connect();
            if (flowClient == null) return;
            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{selectedFlow.UniqueId}/modifyowners?api-version=2016-11-01&cascadeoperation=true";

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Adding Owner(s) ",
                Work = (w, args) =>
                {
                    List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
                    foreach (FlowOwner owner in owners)
                    {
                        string payload = JsonConvert.SerializeObject(new
                        {
                            put = new List<object>()
                            {
                               new
                              {
                            name = owner.Id,
                            properties = new
                            {
                                principal = new
                                {
                                    id = owner.Id,
                                    displayName = owner.Name,
                                    email = owner.Email,
                                    type = "User"
                                }
                            }
                              }
                            }
                        });
                        var content = new StringContent(payload);
                        content.Headers.ContentType.MediaType = "application/json";

                        responses.Add(flowClient.PostAsync(url, content).GetAwaiter().GetResult());
                    }
                    args.Result = responses;
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(this, "Error while removing the owner: " + args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (args.Result is List<HttpResponseMessage>)
                    {
                        List<HttpResponseMessage> responses = (List<HttpResponseMessage>)args.Result;
                        if (responses.Any(res => !res.IsSuccessStatusCode))
                        {
                            MessageBox.Show(this, "Error while removing the owner: " + responses.First(res => !res.IsSuccessStatusCode).ReasonPhrase, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show(this, "Owner(s) removed successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Utils.Ai.WriteEvent("Flow Owner Updated", owners.Count);
                        }
                    }
                    GetFlowDetails();
                }
            }); ; ;
        }

        private void UpdateOwnerDV(List<FlowOwner> owners)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Updating the owner(s)",
                Work = (w, e) =>
                {
                    List<OrganizationResponse> responses = new List<OrganizationResponse>();
                    OrganizationRequest orgReq = new OrganizationRequest();
                    orgReq.RequestName = "GrantAccess";
                    orgReq.Parameters = new ParameterCollection();
                    EntityReference target = new EntityReference("workflow", selectedFlow.Guid);
                    orgReq.Parameters.Add("Target", target);
                    PrincipalAccess prinAccess = new PrincipalAccess();
                    prinAccess.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess | AccessRights.CreateAccess | AccessRights.DeleteAccess | AccessRights.ShareAccess | AccessRights.AssignAccess;
                    foreach (FlowOwner owner in owners)
                    {
                        if (owner.SysId == Guid.Empty)
                        {
                            var query = new QueryExpression("systemuser");

                            query.ColumnSet.AddColumn("systemuserid");

                            // Add conditions to query.Criteria
                            query.Criteria.AddCondition("azureactivedirectoryobjectid", ConditionOperator.Equal, owner.Id);

                            EntityCollection userResp = Service.RetrieveMultiple(query);
                            if (userResp.Entities.Count == 0)
                            {
                                e.Result = "User not found in Dataverse";
                                break;
                            }
                            owner.SysId = userResp.Entities[0].Id;
                        }

                        prinAccess.Principal = new EntityReference("systemuser", owner.SysId);
                        orgReq.Parameters.Add("PrincipalAccess", prinAccess);
                        responses.Add(Service.Execute(orgReq));
                    }
                    if (e.Result == null) e.Result = responses;

                    //rgReq.Parameters.Add("Target", new EntityReference("flow", selectedFlow.Guid));
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(this, "Error while updating the owner(s): " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (e.Result.GetType() == typeof(string))
                    {
                        MessageBox.Show(this, "Error while updating the owner(s): " + e.Result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(this, "Owner(s) updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Utils.Ai.WriteEvent("Flow Owner Updated", owners.Count);
                    }

                    GetFlowDetails();
                },
            });
        }

        private void RemoveOwnerAPI(FlowOwner owner)
        {
            if (flowConn == null) Connect();
            if (flowClient == null) return;
            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{selectedFlow.UniqueId}/owners/{owner.Id}?api-version=2016-11-01";

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Deleting Owner " + owner.Name,
                Work = (w, args) =>
                {
                    HttpResponseMessage response = flowClient.DeleteAsync(url).GetAwaiter().GetResult();
                    args.Result = response;
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(this, "Error while removing the owner: " + args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (args.Result is HttpResponseMessage)
                    {
                        HttpResponseMessage response = (HttpResponseMessage)args.Result;
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show(this, "Owner removed successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Utils.Ai.WriteEvent("Flow Owner Removed", 1);
                        }
                        else
                        {
                            MessageBox.Show(this, "Error while removing the owner: " + response.ReasonPhrase, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    GetFlowDetails();
                }
            });
        }
    }
}