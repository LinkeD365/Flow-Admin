using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace LinkeD365.FlowAdmin
{
    public partial class Owner : Form
    {
        private HttpClient graphClient;

        public Owner(HttpClient graphClient)
        {
            InitializeComponent();
            this.graphClient = graphClient;
            SelectedOwners.Clear();
        }

        private List<FlowOwner> flowOwners = new List<FlowOwner>();
        public List<FlowOwner> SelectedOwners { get; private set; } = new List<FlowOwner>();

        private void GetUsers()
        {
            string url = chkUsers.Checked ? $"https://graph.microsoft.com/v1.0/users?$filter=startswith(displayName,'{txtSearch.Text}') or startsWith(mail,'{txtSearch.Text}') or startsWith(userPrincipalName,'{txtSearch.Text}')&$select=displayName,mail,userPrincipalName,id" : "https://graph.microsoft.com/v1.0";
            var response = graphClient.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                flowOwners.Clear();
                var ownerListJO = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (ownerListJO["value"].HasValues)
                {
                    foreach (JToken ownerJO in ownerListJO["value"].Children())
                    {
                        flowOwners.Add(
                            new FlowOwner()
                            {
                                Id = Guid.Parse(ownerJO["id"].ToString()),
                                Name = ownerJO["displayName"].ToString(),
                                Email = ownerJO["mail"].ToString(),
                                Principle = ownerJO["userPrincipalName"].ToString()
                            });
                    }
                }
                gridOwners.DataSource = null;
                gridOwners.DataSource = flowOwners;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            timerSearch.Stop();
            timerSearch.Start();
        }

        private void timerSearch_Tick(object sender, EventArgs e)
        {
            timerSearch.Stop();
            if (txtSearch.Text != string.Empty)
            {
                GetUsers();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (gridOwners.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an owner to add", "No Owner Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Please ensure that you trust the new owner(s). You are sharing permission to the Flow's connections. \n\nDo you want to continue?", "Before you share...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;

            foreach (DataGridViewRow row in gridOwners.SelectedRows)
            {
                SelectedOwners.Add(row.DataBoundItem as FlowOwner);
            }
        }

        private void gridOwners_SelectionChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = gridOwners.SelectedRows.Count > 0;
        }
    }
}