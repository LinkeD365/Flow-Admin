using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowAdmin
{
    public partial class AdminControl : PluginControlBase
    {
        private Settings mySettings;

        private SortableBindingList<FlowRun> runsBS = new SortableBindingList<FlowRun>();

        public AdminControl()
        {
            InitializeComponent();
            VScrollBar runsSB = gridFlowRuns.Controls.OfType<VScrollBar>().FirstOrDefault();
            runsSB.Scroll += RunsSB_EndScroll;

            gridFlowRuns.DataSource = runsBS;
            InitRunGrid();
        }

        private void InitRunGrid()
        {
            gridFlowRuns.Columns["Selected"].Width = 30;
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

            GetFirstFlowRuns(selectedFlow, btnRuns.Text.Replace(" Runs", ""));
        }

        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            if (selectedFlow == null) return;
            GetAllRunning(true);
            GetFirstFlowRuns(selectedFlow, "All");
            btnRuns.Text = "All Runs";
        }

        private void runsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (selectedFlow == null) return;
            GetFirstFlowRuns(selectedFlow, e.ClickedItem.Text);
            btnRuns.Text = e.ClickedItem.Text + " Runs";
            gridFlowRuns.Sort(gridFlowRuns.Columns["Start"], ListSortDirection.Descending);
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            if (selectedFlow == null) return;

            if (MessageBox.Show($"Are you sure you want to {(selectedFlow.Status == "Stopped" ? "enable" : "disable")} the flow {selectedFlow.Name}?", $"{(selectedFlow.Status == "Stopped" ? "Enable" : "Disable")} Flow", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            DisableEnableFlow();
        }

        private void gridFlowRuns_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridFlowRuns.CurrentCell is DataGridViewCheckBoxCell)
                gridFlowRuns.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}