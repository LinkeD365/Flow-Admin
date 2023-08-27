using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowAdmin
{
    public partial class AdminControl : PluginControlBase
    {
        private Settings mySettings;

        private BindingList<FlowRun> runsBS = new BindingList<FlowRun>();

        public AdminControl()
        {
            InitializeComponent();
            VScrollBar runsSB = gridFlowRuns.Controls.OfType<VScrollBar>().FirstOrDefault();
            runsSB.Scroll += RunsSB_EndScroll;

            gridFlowRuns.DataSource = runsBS;
        }

        private void RunsSB_EndScroll(object sender, ScrollEventArgs e)
        {
            if (e.Type != ScrollEventType.EndScroll) return;
            if (nextLinkUrl != string.Empty) { GetRuns(nextLinkUrl); }
            //throw new NotImplementedException();
        }

        private void AdminControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out aPIConnections))
            {
                aPIConnections = new APIConns();

                LogWarning("Settings not found => a new settings file has been created!");
            }
        }

        private void TsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void TsbSample_Click(object sender, EventArgs e)
        {
            ExecuteMethod(GetAccounts);
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been
        /// updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        private void lblCreated_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtModified_TextChanged(object sender, EventArgs e)
        {
        }

        private void lblPlan_Click(object sender, EventArgs e)
        {
        }

        private void menuRun_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (selectedFlow == null) return;
            GetFirstFlowRuns(selectedFlow, e.ClickedItem.Text);
            btnRuns.Text = e.ClickedItem.Text + " Runs";
            //foreach (ToolStripMenuItem item in menuRun.Items)
            //{
            //    item.Checked = false;
            //}
            //((ToolStripMenuItem)e.ClickedItem).Checked = true;
        }

        private void gridFlowRuns_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type != ScrollEventType.EndScroll) return;

            // if (nextLinkUrl != string.Empty) {
            // GetRuns(gridFlowRuns.DataSource as
            // List<FlowRun>, nextLinkUrl); }
        }

        private void btnCancelSelected_Click(object sender, EventArgs e)
        {
            if (!runsBS.Any(fr => fr.Selected && fr.Status == "Running"))
            {
                MessageBox.Show("Please select one or more running flows before cancelling", "Select a Flow", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRuns = runsBS.Where(fr => fr.Selected).ToList();
            CancelAllFlows(selectedRuns);
        }

        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            if (selectedFlow == null) return;
            GetAllRunning(true);
        }

        private void runsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (selectedFlow == null) return;
            GetFirstFlowRuns(selectedFlow, e.ClickedItem.Text);
            btnRuns.Text = e.ClickedItem.Text + " Runs";
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            if (selectedFlow == null) return;

            if (MessageBox.Show($"Are you sure you want to {(selectedFlow.Status == "Stopped" ? "enable" : "disable")} the flow {selectedFlow.Name}?", $"{(selectedFlow.Status == "Stopped" ? "Enable" : "Disable")} Flow", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            DisableEnableFlow();
        }
    }
}