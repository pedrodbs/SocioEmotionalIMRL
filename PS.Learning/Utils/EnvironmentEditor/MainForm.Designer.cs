namespace PS.Learning.Utils.EnvironmentEditor
{
    partial class MainForm
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.envPanel = new System.Windows.Forms.Panel();
            this.cellsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectedCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDescToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.copyAllElementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteElementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSelectedElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearElementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.envMenuStrip = new System.Windows.Forms.MenuStrip();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeTxtBox = new System.Windows.Forms.ToolStripTextBox();
            this.cellListBox = new System.Windows.Forms.ListBox();
            this.cellMenuStrip = new System.Windows.Forms.MenuStrip();
            this.newCellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.cellsContextMenuStrip.SuspendLayout();
            this.envMenuStrip.SuspendLayout();
            this.cellMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.Filter = "XML files|*.xml";
            this.openFileDialog.InitialDirectory = "../../bin/config";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.FileName = "environment.xml";
            this.saveFileDialog.Filter = "XML files|*.xml";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.envPanel);
            this.splitContainer1.Panel1.Controls.Add(this.envMenuStrip);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cellListBox);
            this.splitContainer1.Panel2.Controls.Add(this.cellMenuStrip);
            this.splitContainer1.Size = new System.Drawing.Size(652, 182);
            this.splitContainer1.SplitterDistance = 403;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // envPanel
            // 
            this.envPanel.BackColor = System.Drawing.Color.White;
            this.envPanel.ContextMenuStrip = this.cellsContextMenuStrip;
            this.envPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.envPanel.Location = new System.Drawing.Point(0, 25);
            this.envPanel.Name = "envPanel";
            this.envPanel.Size = new System.Drawing.Size(403, 157);
            this.envPanel.TabIndex = 1;
            // 
            // cellsContextMenuStrip
            // 
            this.cellsContextMenuStrip.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cellsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedCellsToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.clearSelectionToolStripMenuItem});
            this.cellsContextMenuStrip.Name = "contextMenuStrip1";
            this.cellsContextMenuStrip.Size = new System.Drawing.Size(167, 76);
            this.cellsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.CellsContextMenuStripOpening);
            // 
            // selectedCellsToolStripMenuItem
            // 
            this.selectedCellsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editDescToolStripMenuItem,
            this.toolStripSeparator4,
            this.copyAllElementsToolStripMenuItem,
            this.pasteElementsToolStripMenuItem,
            this.toolStripSeparator2,
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearElementsToolStripMenuItem});
            this.selectedCellsToolStripMenuItem.Enabled = false;
            this.selectedCellsToolStripMenuItem.Name = "selectedCellsToolStripMenuItem";
            this.selectedCellsToolStripMenuItem.Size = new System.Drawing.Size(166, 24);
            this.selectedCellsToolStripMenuItem.Text = "&Selected cells";
            // 
            // editDescToolStripMenuItem
            // 
            this.editDescToolStripMenuItem.Name = "editDescToolStripMenuItem";
            this.editDescToolStripMenuItem.Size = new System.Drawing.Size(188, 24);
            this.editDescToolStripMenuItem.Text = "Edit &description";
            this.editDescToolStripMenuItem.Click += new System.EventHandler(this.EditDescToolStripMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(185, 6);
            // 
            // copyAllElementsToolStripMenuItem
            // 
            this.copyAllElementsToolStripMenuItem.Name = "copyAllElementsToolStripMenuItem";
            this.copyAllElementsToolStripMenuItem.Size = new System.Drawing.Size(188, 24);
            this.copyAllElementsToolStripMenuItem.Text = "&Copy all elements";
            this.copyAllElementsToolStripMenuItem.Click += new System.EventHandler(this.CopyAllElementsToolStripMenuItemClick);
            // 
            // pasteElementsToolStripMenuItem
            // 
            this.pasteElementsToolStripMenuItem.Enabled = false;
            this.pasteElementsToolStripMenuItem.Name = "pasteElementsToolStripMenuItem";
            this.pasteElementsToolStripMenuItem.Size = new System.Drawing.Size(188, 24);
            this.pasteElementsToolStripMenuItem.Text = "&Paste elements";
            this.pasteElementsToolStripMenuItem.Click += new System.EventHandler(this.PasteElementsToolStripMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSelectedElementToolStripMenuItem,
            this.toolStripSeparator3});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(188, 24);
            this.addToolStripMenuItem.Text = "&Add";
            // 
            // addSelectedElementToolStripMenuItem
            // 
            this.addSelectedElementToolStripMenuItem.Name = "addSelectedElementToolStripMenuItem";
            this.addSelectedElementToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.addSelectedElementToolStripMenuItem.Text = "&Selected element";
            this.addSelectedElementToolStripMenuItem.Click += new System.EventHandler(this.AddSelectedElementToolStripMenuItemClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(180, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(188, 24);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // clearElementsToolStripMenuItem
            // 
            this.clearElementsToolStripMenuItem.Name = "clearElementsToolStripMenuItem";
            this.clearElementsToolStripMenuItem.Size = new System.Drawing.Size(188, 24);
            this.clearElementsToolStripMenuItem.Text = "&Clear elements";
            this.clearElementsToolStripMenuItem.Click += new System.EventHandler(this.ClearElementsToolStripMenuItemClick);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(166, 24);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItemClick);
            // 
            // clearSelectionToolStripMenuItem
            // 
            this.clearSelectionToolStripMenuItem.Name = "clearSelectionToolStripMenuItem";
            this.clearSelectionToolStripMenuItem.Size = new System.Drawing.Size(166, 24);
            this.clearSelectionToolStripMenuItem.Text = "&Clear selection";
            this.clearSelectionToolStripMenuItem.Click += new System.EventHandler(this.ClearSelectionToolStripMenuItemClick);
            // 
            // envMenuStrip
            // 
            this.envMenuStrip.BackColor = System.Drawing.Color.White;
            this.envMenuStrip.Font = new System.Drawing.Font("Candara", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.envMenuStrip.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.envMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.printToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.sizeToolStripMenuItem});
            this.envMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.envMenuStrip.Name = "envMenuStrip";
            this.envMenuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.envMenuStrip.Size = new System.Drawing.Size(403, 25);
            this.envMenuStrip.TabIndex = 0;
            this.envMenuStrip.Text = "envMenuStrip";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(48, 23);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItemClick);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(64, 23);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(49, 23);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(77, 23);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItemClick);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Enabled = false;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printToolStripMenuItem.Size = new System.Drawing.Size(50, 23);
            this.printToolStripMenuItem.Text = "&Print";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItemClick);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(42, 23);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // sizeToolStripMenuItem
            // 
            this.sizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sizeTxtBox});
            this.sizeToolStripMenuItem.Name = "sizeToolStripMenuItem";
            this.sizeToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.sizeToolStripMenuItem.Text = "S&ize";
            // 
            // sizeTxtBox
            // 
            this.sizeTxtBox.Name = "sizeTxtBox";
            this.sizeTxtBox.Size = new System.Drawing.Size(100, 25);
            this.sizeTxtBox.Text = "50";
            this.sizeTxtBox.Click += new System.EventHandler(this.SizeTxtBoxClick);
            // 
            // cellListBox
            // 
            this.cellListBox.BackColor = System.Drawing.Color.White;
            this.cellListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellListBox.ForeColor = System.Drawing.Color.Black;
            this.cellListBox.FormattingEnabled = true;
            this.cellListBox.ItemHeight = 15;
            this.cellListBox.Location = new System.Drawing.Point(0, 25);
            this.cellListBox.Name = "cellListBox";
            this.cellListBox.Size = new System.Drawing.Size(244, 157);
            this.cellListBox.Sorted = true;
            this.cellListBox.TabIndex = 1;
            this.cellListBox.SelectedIndexChanged += new System.EventHandler(this.CellListBoxSelectedIndexChanged);
            // 
            // cellMenuStrip
            // 
            this.cellMenuStrip.Enabled = false;
            this.cellMenuStrip.Font = new System.Drawing.Font("Candara", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cellMenuStrip.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cellMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCellToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.deleteAllToolStripMenuItem});
            this.cellMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.cellMenuStrip.Name = "cellMenuStrip";
            this.cellMenuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.cellMenuStrip.Size = new System.Drawing.Size(244, 25);
            this.cellMenuStrip.TabIndex = 0;
            this.cellMenuStrip.Text = "cellMenuStrip";
            // 
            // newCellToolStripMenuItem
            // 
            this.newCellToolStripMenuItem.Name = "newCellToolStripMenuItem";
            this.newCellToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.newCellToolStripMenuItem.Size = new System.Drawing.Size(48, 23);
            this.newCellToolStripMenuItem.Text = "N&ew";
            this.newCellToolStripMenuItem.Click += new System.EventHandler(this.NewCellToolStripMenuItemClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.editToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.EditToolStripMenuItemClick);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(60, 23);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(79, 23);
            this.deleteAllToolStripMenuItem.Text = "Delete A&ll";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.DeleteAllToolStripMenuItemClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(652, 182);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Candara", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Environment Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.cellsContextMenuStrip.ResumeLayout(false);
            this.envMenuStrip.ResumeLayout(false);
            this.envMenuStrip.PerformLayout();
            this.cellMenuStrip.ResumeLayout(false);
            this.cellMenuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip envMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.MenuStrip cellMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newCellToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ListBox cellListBox;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.Panel envPanel;
        private System.Windows.Forms.ContextMenuStrip cellsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem selectedCellsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllElementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteElementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearElementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addSelectedElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem sizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox sizeTxtBox;
        private System.Windows.Forms.ToolStripMenuItem editDescToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}


