namespace LinkeD365.FlowAdmin
{
    partial class Owner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitTop = new System.Windows.Forms.SplitContainer();
            this.splitSearch = new System.Windows.Forms.SplitContainer();
            this.lblSearch = new System.Windows.Forms.Label();
            this.splitUser = new System.Windows.Forms.SplitContainer();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.chkGroups = new System.Windows.Forms.CheckBox();
            this.chkUsers = new System.Windows.Forms.CheckBox();
            this.gridOwners = new System.Windows.Forms.DataGridView();
            this.timerSearch = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTop)).BeginInit();
            this.splitTop.Panel1.SuspendLayout();
            this.splitTop.Panel2.SuspendLayout();
            this.splitTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitSearch)).BeginInit();
            this.splitSearch.Panel1.SuspendLayout();
            this.splitSearch.Panel2.SuspendLayout();
            this.splitSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitUser)).BeginInit();
            this.splitUser.Panel1.SuspendLayout();
            this.splitUser.Panel2.SuspendLayout();
            this.splitUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOwners)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(347, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 107;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(266, 14);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 106;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.splitTop);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.btnCancel);
            this.splitMain.Panel2.Controls.Add(this.btnOK);
            this.splitMain.Size = new System.Drawing.Size(434, 211);
            this.splitMain.SplitterDistance = 158;
            this.splitMain.TabIndex = 108;
            // 
            // splitTop
            // 
            this.splitTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitTop.Location = new System.Drawing.Point(0, 0);
            this.splitTop.Name = "splitTop";
            this.splitTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitTop.Panel1
            // 
            this.splitTop.Panel1.Controls.Add(this.splitSearch);
            // 
            // splitTop.Panel2
            // 
            this.splitTop.Panel2.Controls.Add(this.gridOwners);
            this.splitTop.Size = new System.Drawing.Size(434, 158);
            this.splitTop.SplitterDistance = 25;
            this.splitTop.TabIndex = 0;
            // 
            // splitSearch
            // 
            this.splitSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitSearch.IsSplitterFixed = true;
            this.splitSearch.Location = new System.Drawing.Point(0, 0);
            this.splitSearch.Name = "splitSearch";
            // 
            // splitSearch.Panel1
            // 
            this.splitSearch.Panel1.Controls.Add(this.lblSearch);
            // 
            // splitSearch.Panel2
            // 
            this.splitSearch.Panel2.Controls.Add(this.splitUser);
            this.splitSearch.Size = new System.Drawing.Size(434, 25);
            this.splitSearch.SplitterDistance = 64;
            this.splitSearch.TabIndex = 0;
            // 
            // lblSearch
            // 
            this.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSearch.Location = new System.Drawing.Point(0, 0);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(64, 25);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitUser
            // 
            this.splitUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitUser.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitUser.Location = new System.Drawing.Point(0, 0);
            this.splitUser.Name = "splitUser";
            // 
            // splitUser.Panel1
            // 
            this.splitUser.Panel1.Controls.Add(this.txtSearch);
            this.splitUser.Panel1MinSize = 240;
            // 
            // splitUser.Panel2
            // 
            this.splitUser.Panel2.Controls.Add(this.chkGroups);
            this.splitUser.Panel2.Controls.Add(this.chkUsers);
            this.splitUser.Size = new System.Drawing.Size(366, 25);
            this.splitUser.SplitterDistance = 268;
            this.splitUser.TabIndex = 0;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(268, 20);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // chkGroups
            // 
            this.chkGroups.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkGroups.AutoSize = true;
            this.chkGroups.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkGroups.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkGroups.Location = new System.Drawing.Point(43, 0);
            this.chkGroups.Name = "chkGroups";
            this.chkGroups.Size = new System.Drawing.Size(51, 25);
            this.chkGroups.TabIndex = 1;
            this.chkGroups.Text = "Groups";
            this.chkGroups.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkGroups.UseVisualStyleBackColor = true;
            // 
            // chkUsers
            // 
            this.chkUsers.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkUsers.AutoSize = true;
            this.chkUsers.Checked = true;
            this.chkUsers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUsers.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkUsers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkUsers.Location = new System.Drawing.Point(0, 0);
            this.chkUsers.Name = "chkUsers";
            this.chkUsers.Size = new System.Drawing.Size(44, 25);
            this.chkUsers.TabIndex = 0;
            this.chkUsers.Text = "Users";
            this.chkUsers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkUsers.UseVisualStyleBackColor = true;
            // 
            // gridOwners
            // 
            this.gridOwners.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridOwners.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOwners.Location = new System.Drawing.Point(0, 0);
            this.gridOwners.Name = "gridOwners";
            this.gridOwners.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridOwners.Size = new System.Drawing.Size(434, 129);
            this.gridOwners.TabIndex = 0;
            this.gridOwners.SelectionChanged += new System.EventHandler(this.gridOwners_SelectionChanged);
            // 
            // timerSearch
            // 
            this.timerSearch.Interval = 2000;
            this.timerSearch.Tick += new System.EventHandler(this.timerSearch_Tick);
            // 
            // Owner
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 211);
            this.Controls.Add(this.splitMain);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(450, 250);
            this.Name = "Owner";
            this.Text = "Owner";
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitTop.Panel1.ResumeLayout(false);
            this.splitTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTop)).EndInit();
            this.splitTop.ResumeLayout(false);
            this.splitSearch.Panel1.ResumeLayout(false);
            this.splitSearch.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitSearch)).EndInit();
            this.splitSearch.ResumeLayout(false);
            this.splitUser.Panel1.ResumeLayout(false);
            this.splitUser.Panel1.PerformLayout();
            this.splitUser.Panel2.ResumeLayout(false);
            this.splitUser.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitUser)).EndInit();
            this.splitUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOwners)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SplitContainer splitTop;
        private System.Windows.Forms.SplitContainer splitSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.SplitContainer splitUser;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckBox chkUsers;
        private System.Windows.Forms.CheckBox chkGroups;
        private System.Windows.Forms.Timer timerSearch;
        private System.Windows.Forms.DataGridView gridOwners;
    }
}