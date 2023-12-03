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
    }
}