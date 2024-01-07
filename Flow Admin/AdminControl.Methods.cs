using System;
using System.ComponentModel;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowAdmin
{
    public partial class AdminControl : PluginControlBase
    {
        private void SortGrid(string name, SortOrder sortOrder)
        {
            return;
            //SortableBindingList<FlowDefinition> sortingFlows = (SortableBindingList<FlowDefinition>)gridFlows.DataSource;
            //sortingFlows.Sort(new FlowDefComparer(name, sortOrder));
            //gridFlows.DataSource = null;
            //gridFlows.DataSource = sortingFlows;
            //InitGrid();
            //gridFlows.Columns[name].HeaderCell.SortGlyphDirection = sortOrder;
        }

        private void InitGrid()
        {
            gridFlows.SelectionChanged -= gridFlows_SelectionChanged;
            gridFlows.ClearSelection();

            gridFlows.DataSource = null;
            gridFlows.DataSource = flows;

            gridFlows.AutoResizeColumns();
            gridFlows.Columns["Name"].SortMode = DataGridViewColumnSortMode.Automatic;
            gridFlows.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            gridFlows.Columns["Managed"].SortMode = DataGridViewColumnSortMode.Automatic;
            gridFlows.Sort(gridFlows.Columns["Name"], ListSortDirection.Ascending);
            //SortGrid("Name", SortOrder.Ascending);
            gridFlows.ClearSelection();

            gridFlows.SelectionChanged += gridFlows_SelectionChanged;
        }

        internal void ShowError(string error, string caption = null)
        {
            ShowError(new Exception(error), caption);
        }

        private void ShowError(Exception error, string caption = null)
        {
            LogError(error.ToString());
            if (error.InnerException != null)
            {
                ShowError(error.InnerException);
            }
            else
            {
                MessageBox.Show(error.Message, caption ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnableControls()
        {
            ddEnable.Enabled = gridFlows.SelectedRows.Count != 1;
            btnCancel.Enabled = gridFlows.SelectedRows.Count == 1;
            btnAddOwner.Enabled = gridFlows.SelectedRows.Count == 1;
            btnRuns.Enabled = gridFlows.SelectedRows.Count == 1;
            btnRemove.ReadOnly = gridFlows.SelectedRows.Count == 1;
            btnDisable.Enabled = gridFlows.SelectedRows.Count == 1;

            if (gridFlows.SelectedRows.Count != 1)
            {
                gridFlowRuns.DataSource = null;
                gridOwners.DataSource = null;
                txtCreated.Text = string.Empty;
                txtModified.Text = string.Empty;
                txtStatus.Text = string.Empty;
                txtPlan.Text = string.Empty;
            }
        }

        private void EnableDisableMulti(bool enable)
        {
            if (gridFlows.SelectedRows.Count > 1)
            {
                if (MessageBox.Show($"Are you sure you want to {(enable ? "enable" : "disable")}  the selected flows?", $"{(enable ? "Enable" : "Disable")} Flows", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                foreach (DataGridViewRow row in gridFlows.SelectedRows)
                {
                    var flow = row.DataBoundItem as FlowDefinition;
                    DisableEnableFlow(flow, enable);
                }

                Utils.Ai.WriteEvent("Flow En/Disabled", gridFlows.SelectedRows.Count);
            }
        }
    }
}