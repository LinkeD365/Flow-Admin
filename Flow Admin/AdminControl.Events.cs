using System;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowAdmin
{
    public partial class AdminControl : PluginControlBase
    {
        private void btnConnectDataverse_Click(object sender, EventArgs e)
        {
            flowConn = null;
            flowClient = null;
            ExecuteMethod(this.LoadFlowsFromDV);
            ExecuteMethod(this.LoadSolutionsFromDV);
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textSearch.Text))
            {
                gridFlows.DataSource = flows.Where(flw => flw.Name.ToLower().Contains(textSearch.Text.ToLower())).ToList();
            }
            else
            {
                gridFlows.DataSource = flows;
            }
        }

        private void ddlSolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteMethod(GetFlowsForSolution);
        }

        private void gridFlows_SelectionChanged(object sender, EventArgs e)
        {
            EnableControls();
            if (gridFlows.SelectedRows.Count != 1) { return; }
            // if (gridFlows.SelectedRows.Count > 1) {
            // return; }
            var flow = gridFlows.SelectedRows[0].DataBoundItem as FlowDefinition;
            if (selectedFlow?.Id == flow.Id) return;
            selectedFlow = flow;
            runsBS.Clear();
            btnRuns.Text = "Runs";
            ExecuteMethod(GetFlowDetails);
        }

        private void btnConnectFlow_Click(object sender, EventArgs e)
        {
            flowConn = null;
            flowClient = null;
            fromAPIButton = true;
            LoadUnSolutionedFlows();
        }

        private void btnAddOwner_Click(object sender, EventArgs e)
        {
            Connect(true);
            if (graphClient == null) return;
            var owner = new Owner(graphClient);
            if (owner.ShowDialog() == DialogResult.OK && owner.SelectedOwners.Any())
            {
                if (selectedFlow.Solution) ExecuteMethod(UpdateOwnerDV, owner.SelectedOwners);
                else ExecuteMethod(UpdateOwnerAPI, owner.SelectedOwners);
            };
        }

        private void gridOwners_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 2) return;

            var owner = gridOwners.Rows[e.RowIndex].DataBoundItem as FlowOwner;
            if (owner.Id.ToString() == selectedFlow.AzureOwnerId) return; // Can't remove the owner of the flow
            if (MessageBox.Show($@"Do you want to remove {owner.Name} as an owner of this flow?", "Remove Owner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (selectedFlow.Solution) ExecuteMethod(RemoveOwnerDV, owner);
                else ExecuteMethod(RemoveOwnerAPI, owner);
            }
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            if (selectedFlow == null) return;
            bool enable = false;
            switch (selectedFlow.Status)
            {
                case "Stopped":
                case "Suspended":
                    enable = true;
                    break;

                case "Enabled":
                case "Started":
                    enable = false; break;
                default:
                    return; ;
            }
            if (MessageBox.Show($"Are you sure you want to {(enable ? "enable" : "disable")} the flow {selectedFlow.Name}?", $"{(enable ? "Enable" : "Disable")} Flow", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            DisableEnableFlow(selectedFlow, enable);
            Utils.Ai.WriteEvent("Flow En/Disabled", 1);
            GetFlowDetails();
        }

        private void btnEnableMulti_Click(object sender, EventArgs args)
        {
            EnableDisableMulti(true);
        }

        private void btnDisableMulti_Click(object sender, EventArgs e)
        {
            EnableDisableMulti(false);
        }
    }
}