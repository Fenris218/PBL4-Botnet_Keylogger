namespace Server.Forms
{
    partial class FrmFileManager
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
            components = new System.ComponentModel.Container();
            contextMenuStripDirectory = new ContextMenuStrip(components);
            downloadToolStripMenuItem = new ToolStripMenuItem();
            uploadToolStripMenuItem = new ToolStripMenuItem();
            lineToolStripMenuItem = new ToolStripSeparator();
            executeToolStripMenuItem = new ToolStripMenuItem();
            renameToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            line2ToolStripMenuItem = new ToolStripSeparator();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            openDirectoryInShellToolStripMenuItem = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            stripLblStatus = new ToolStripStatusLabel();
            contextMenuStripTransfers = new ContextMenuStrip(components);
            cancelToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            clearToolStripMenuItem = new ToolStripMenuItem();
            TabControlFileManager = new TabControl();
            tabFileExplorer = new TabPage();
            lblPath = new Label();
            txtPath = new TextBox();
            lstDirectory = new ListView();
            hName = new ColumnHeader();
            hSize = new ColumnHeader();
            hType = new ColumnHeader();
            lblDrive = new Label();
            cmbDrives = new ComboBox();
            tabTransfers = new TabPage();
            btnOpenDLFolder = new Button();
            lstTransfers = new ListView();
            hID = new ColumnHeader();
            hTransferType = new ColumnHeader();
            hStatus = new ColumnHeader();
            hFilename = new ColumnHeader();
            contextMenuStripDirectory.SuspendLayout();
            statusStrip.SuspendLayout();
            contextMenuStripTransfers.SuspendLayout();
            TabControlFileManager.SuspendLayout();
            tabFileExplorer.SuspendLayout();
            tabTransfers.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStripDirectory
            // 
            contextMenuStripDirectory.Items.AddRange(new ToolStripItem[] { downloadToolStripMenuItem, uploadToolStripMenuItem, lineToolStripMenuItem, executeToolStripMenuItem, renameToolStripMenuItem, deleteToolStripMenuItem, line2ToolStripMenuItem, refreshToolStripMenuItem, openDirectoryInShellToolStripMenuItem });
            contextMenuStripDirectory.Name = "ctxtMenu";
            contextMenuStripDirectory.Size = new Size(240, 170);
            // 
            // downloadToolStripMenuItem
            // 
            downloadToolStripMenuItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            downloadToolStripMenuItem.Size = new Size(239, 22);
            downloadToolStripMenuItem.Text = "Download";
            downloadToolStripMenuItem.Click += downloadToolStripMenuItem_Click;
            // 
            // uploadToolStripMenuItem
            // 
            uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            uploadToolStripMenuItem.Size = new Size(239, 22);
            uploadToolStripMenuItem.Text = "Upload";
            uploadToolStripMenuItem.Click += uploadToolStripMenuItem_Click;
            // 
            // lineToolStripMenuItem
            // 
            lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            lineToolStripMenuItem.Size = new Size(236, 6);
            // 
            // executeToolStripMenuItem
            // 
            executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            executeToolStripMenuItem.Size = new Size(239, 22);
            executeToolStripMenuItem.Text = "Execute";
            executeToolStripMenuItem.Click += executeToolStripMenuItem_Click;
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(239, 22);
            renameToolStripMenuItem.Text = "Rename";
            renameToolStripMenuItem.Click += renameToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(239, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // line2ToolStripMenuItem
            // 
            line2ToolStripMenuItem.Name = "line2ToolStripMenuItem";
            line2ToolStripMenuItem.Size = new Size(236, 6);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(239, 22);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
            // 
            // openDirectoryInShellToolStripMenuItem
            // 
            openDirectoryInShellToolStripMenuItem.Name = "openDirectoryInShellToolStripMenuItem";
            openDirectoryInShellToolStripMenuItem.Size = new Size(239, 22);
            openDirectoryInShellToolStripMenuItem.Text = "Open Directory in Remote Shell";
            openDirectoryInShellToolStripMenuItem.Click += openDirectoryToolStripMenuItem_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { stripLblStatus });
            statusStrip.Location = new Point(0, 456);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(858, 22);
            statusStrip.TabIndex = 3;
            statusStrip.Text = "statusStrip1";
            // 
            // stripLblStatus
            // 
            stripLblStatus.Name = "stripLblStatus";
            stripLblStatus.Size = new Size(131, 17);
            stripLblStatus.Text = "Status: Loading drives...";
            // 
            // contextMenuStripTransfers
            // 
            contextMenuStripTransfers.Items.AddRange(new ToolStripItem[] { cancelToolStripMenuItem, toolStripMenuItem1, clearToolStripMenuItem });
            contextMenuStripTransfers.Name = "ctxtMenu2";
            contextMenuStripTransfers.Size = new Size(150, 54);
            // 
            // cancelToolStripMenuItem
            // 
            cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            cancelToolStripMenuItem.Size = new Size(149, 22);
            cancelToolStripMenuItem.Text = "Cancel";
            cancelToolStripMenuItem.Click += cancelToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(146, 6);
            // 
            // clearToolStripMenuItem
            // 
            clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            clearToolStripMenuItem.Size = new Size(149, 22);
            clearToolStripMenuItem.Text = "Clear transfers";
            clearToolStripMenuItem.Click += clearToolStripMenuItem_Click;
            // 
            // TabControlFileManager
            // 
            TabControlFileManager.Controls.Add(tabFileExplorer);
            TabControlFileManager.Controls.Add(tabTransfers);
            TabControlFileManager.Dock = DockStyle.Fill;
            TabControlFileManager.Location = new Point(0, 0);
            TabControlFileManager.Name = "TabControlFileManager";
            TabControlFileManager.SelectedIndex = 0;
            TabControlFileManager.Size = new Size(858, 456);
            TabControlFileManager.SizeMode = TabSizeMode.Fixed;
            TabControlFileManager.TabIndex = 5;
            // 
            // tabFileExplorer
            // 
            tabFileExplorer.BackColor = SystemColors.Control;
            tabFileExplorer.Controls.Add(lblPath);
            tabFileExplorer.Controls.Add(txtPath);
            tabFileExplorer.Controls.Add(lstDirectory);
            tabFileExplorer.Controls.Add(lblDrive);
            tabFileExplorer.Controls.Add(cmbDrives);
            tabFileExplorer.Location = new Point(4, 22);
            tabFileExplorer.Name = "tabFileExplorer";
            tabFileExplorer.Padding = new Padding(3);
            tabFileExplorer.Size = new Size(850, 430);
            tabFileExplorer.TabIndex = 0;
            tabFileExplorer.Text = "File Explorer";
            // 
            // lblPath
            // 
            lblPath.AutoSize = true;
            lblPath.Location = new Point(279, 12);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(75, 13);
            lblPath.TabIndex = 4;
            lblPath.Text = "Remote Path:";
            // 
            // txtPath
            // 
            txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPath.BorderStyle = BorderStyle.FixedSingle;
            txtPath.Location = new Point(360, 8);
            txtPath.Name = "txtPath";
            txtPath.ReadOnly = true;
            txtPath.Size = new Size(484, 22);
            txtPath.TabIndex = 3;
            txtPath.Text = "\\";
            // 
            // lstDirectory
            // 
            lstDirectory.AllowDrop = true;
            lstDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstDirectory.Columns.AddRange(new ColumnHeader[] { hName, hSize, hType });
            lstDirectory.ContextMenuStrip = contextMenuStripDirectory;
            lstDirectory.FullRowSelect = true;
            lstDirectory.GridLines = true;
            lstDirectory.Location = new Point(8, 35);
            lstDirectory.Name = "lstDirectory";
            lstDirectory.Size = new Size(836, 388);
            lstDirectory.TabIndex = 2;
            lstDirectory.UseCompatibleStateImageBehavior = false;
            lstDirectory.View = View.Details;
            lstDirectory.DragDrop += lstDirectory_DragDrop;
            lstDirectory.DragEnter += lstDirectory_DragEnter;
            lstDirectory.DoubleClick += lstDirectory_DoubleClick;
            // 
            // hName
            // 
            hName.Text = "Name";
            hName.Width = 360;
            // 
            // hSize
            // 
            hSize.Text = "Size";
            hSize.Width = 125;
            // 
            // hType
            // 
            hType.Text = "Type";
            hType.Width = 168;
            // 
            // lblDrive
            // 
            lblDrive.AutoSize = true;
            lblDrive.Location = new Point(8, 12);
            lblDrive.Name = "lblDrive";
            lblDrive.Size = new Size(36, 13);
            lblDrive.TabIndex = 0;
            lblDrive.Text = "Drive:";
            // 
            // cmbDrives
            // 
            cmbDrives.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDrives.FormattingEnabled = true;
            cmbDrives.Location = new Point(50, 8);
            cmbDrives.Name = "cmbDrives";
            cmbDrives.Size = new Size(212, 21);
            cmbDrives.TabIndex = 1;
            cmbDrives.SelectedIndexChanged += cmbDrives_SelectedIndexChanged;
            // 
            // tabTransfers
            // 
            tabTransfers.BackColor = SystemColors.Control;
            tabTransfers.Controls.Add(btnOpenDLFolder);
            tabTransfers.Controls.Add(lstTransfers);
            tabTransfers.Location = new Point(4, 24);
            tabTransfers.Name = "tabTransfers";
            tabTransfers.Padding = new Padding(3);
            tabTransfers.Size = new Size(850, 428);
            tabTransfers.TabIndex = 1;
            tabTransfers.Text = "Transfers";
            // 
            // btnOpenDLFolder
            // 
            btnOpenDLFolder.Location = new Point(8, 8);
            btnOpenDLFolder.Name = "btnOpenDLFolder";
            btnOpenDLFolder.Size = new Size(145, 21);
            btnOpenDLFolder.TabIndex = 0;
            btnOpenDLFolder.Text = "&Open Download Folder";
            btnOpenDLFolder.UseVisualStyleBackColor = true;
            btnOpenDLFolder.Click += btnOpenDLFolder_Click;
            // 
            // lstTransfers
            // 
            lstTransfers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstTransfers.Columns.AddRange(new ColumnHeader[] { hID, hTransferType, hStatus, hFilename });
            lstTransfers.ContextMenuStrip = contextMenuStripTransfers;
            lstTransfers.FullRowSelect = true;
            lstTransfers.GridLines = true;
            lstTransfers.Location = new Point(8, 35);
            lstTransfers.Name = "lstTransfers";
            lstTransfers.Size = new Size(834, 389);
            lstTransfers.TabIndex = 1;
            lstTransfers.UseCompatibleStateImageBehavior = false;
            lstTransfers.View = View.Details;
            // 
            // hID
            // 
            hID.Text = "ID";
            hID.Width = 128;
            // 
            // hTransferType
            // 
            hTransferType.Text = "Transfer Type";
            hTransferType.Width = 93;
            // 
            // hStatus
            // 
            hStatus.Text = "Status";
            hStatus.Width = 173;
            // 
            // hFilename
            // 
            hFilename.Text = "Filename";
            hFilename.Width = 289;
            // 
            // FrmFileManager
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(858, 478);
            Controls.Add(TabControlFileManager);
            Controls.Add(statusStrip);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            KeyPreview = true;
            MinimumSize = new Size(663, 377);
            Name = "FrmFileManager";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "File Manager []";
            FormClosing += FrmFileManager_FormClosing;
            Load += FrmFileManager_Load;
            KeyDown += FrmFileManager_KeyDown;
            contextMenuStripDirectory.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            contextMenuStripTransfers.ResumeLayout(false);
            TabControlFileManager.ResumeLayout(false);
            tabFileExplorer.ResumeLayout(false);
            tabFileExplorer.PerformLayout();
            tabTransfers.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblDrive;
        private System.Windows.Forms.ColumnHeader hName;
        private System.Windows.Forms.ColumnHeader hSize;
        private System.Windows.Forms.ColumnHeader hType;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDirectory;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.Button btnOpenDLFolder;
        private TabControl TabControlFileManager;
        private System.Windows.Forms.TabPage tabFileExplorer;
        private System.Windows.Forms.TabPage tabTransfers;
        private System.Windows.Forms.ColumnHeader hStatus;
        private System.Windows.Forms.ColumnHeader hFilename;
        private System.Windows.Forms.ColumnHeader hID;
        private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator lineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator line2ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTransfers;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDirectoryInShellToolStripMenuItem;
        private System.Windows.Forms.ComboBox cmbDrives;
        private ListView lstDirectory;
        private ListView lstTransfers;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel stripLblStatus;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader hTransferType;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}