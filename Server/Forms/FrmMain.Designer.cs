namespace Server.Forms
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            contextMenuStrip = new ContextMenuStrip(components);
            systemToolStripMenuItem = new ToolStripMenuItem();
            systemInformationToolStripMenuItem = new ToolStripMenuItem();
            fileManagerToolStripMenuItem = new ToolStripMenuItem();
            taskManagerToolStripMenuItem = new ToolStripMenuItem();
            remoteShellToolStripMenuItem = new ToolStripMenuItem();
            actionsToolStripMenuItem = new ToolStripMenuItem();
            shutdownToolStripMenuItem = new ToolStripMenuItem();
            restartToolStripMenuItem = new ToolStripMenuItem();
            standbyToolStripMenuItem = new ToolStripMenuItem();
            monitoringToolStripMenuItem = new ToolStripMenuItem();
            keyloggerToolStripMenuItem = new ToolStripMenuItem();
            remoteDesktopToolStripMenuItem = new ToolStripMenuItem();
            userSupportToolStripMenuItem = new ToolStripMenuItem();
            showMessageboxToolStripMenuItem = new ToolStripMenuItem();
            clientManagementToolStripMenuItem = new ToolStripMenuItem();
            elevateClientPermissionsToolStripMenuItem = new ToolStripMenuItem();
            reconnectToolStripMenuItem = new ToolStripMenuItem();
            disconnectToolStripMenuItem = new ToolStripMenuItem();
            lstClients = new ListView();
            hIp = new ColumnHeader();
            hUserPC = new ColumnHeader();
            hOS = new ColumnHeader();
            hCountry = new ColumnHeader();
            hStatus = new ColumnHeader();
            hUserStatus = new ColumnHeader();
            hAccountType = new ColumnHeader();
            tableLayoutPanel = new TableLayoutPanel();
            contextMenuStrip.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { systemToolStripMenuItem, monitoringToolStripMenuItem, userSupportToolStripMenuItem, clientManagementToolStripMenuItem });
            contextMenuStrip.Name = "ctxtMenu";
            contextMenuStrip.Size = new Size(180, 92);
            // 
            // systemToolStripMenuItem
            // 
            systemToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { systemInformationToolStripMenuItem, fileManagerToolStripMenuItem, taskManagerToolStripMenuItem, remoteShellToolStripMenuItem, actionsToolStripMenuItem });
            systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            systemToolStripMenuItem.Size = new Size(179, 22);
            systemToolStripMenuItem.Text = "System";
            // 
            // systemInformationToolStripMenuItem
            // 
            systemInformationToolStripMenuItem.Name = "systemInformationToolStripMenuItem";
            systemInformationToolStripMenuItem.Size = new Size(180, 22);
            systemInformationToolStripMenuItem.Text = "System Information";
            systemInformationToolStripMenuItem.Click += systemInformationToolStripMenuItem_Click;
            // 
            //// fileManagerToolStripMenuItem
            //// 
            //fileManagerToolStripMenuItem.Name = "fileManagerToolStripMenuItem";
            //fileManagerToolStripMenuItem.Size = new Size(180, 22);
            //fileManagerToolStripMenuItem.Text = "File Manager";
            //fileManagerToolStripMenuItem.Click += fileManagerToolStripMenuItem_Click;
            //// 
            //// taskManagerToolStripMenuItem
            //// 
            //taskManagerToolStripMenuItem.Name = "taskManagerToolStripMenuItem";
            //taskManagerToolStripMenuItem.Size = new Size(180, 22);
            //taskManagerToolStripMenuItem.Text = "Task Manager";
            //taskManagerToolStripMenuItem.Click += taskManagerToolStripMenuItem_Click;
            //// 
            // remoteShellToolStripMenuItem
            // 
            remoteShellToolStripMenuItem.Name = "remoteShellToolStripMenuItem";
            remoteShellToolStripMenuItem.Size = new Size(180, 22);
            remoteShellToolStripMenuItem.Text = "Remote Shell";
            remoteShellToolStripMenuItem.Click += remoteShellToolStripMenuItem_Click;
            // 
            // actionsToolStripMenuItem
            // 
            actionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { shutdownToolStripMenuItem, restartToolStripMenuItem, standbyToolStripMenuItem });
            actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            actionsToolStripMenuItem.Size = new Size(180, 22);
            actionsToolStripMenuItem.Text = "Actions";
            // 
            // shutdownToolStripMenuItem
            // 
            shutdownToolStripMenuItem.Name = "shutdownToolStripMenuItem";
            shutdownToolStripMenuItem.Size = new Size(180, 22);
            shutdownToolStripMenuItem.Text = "Shutdown";
            shutdownToolStripMenuItem.Click += shutdownToolStripMenuItem_Click;
            // 
            // restartToolStripMenuItem
            // 
            restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            restartToolStripMenuItem.Size = new Size(180, 22);
            restartToolStripMenuItem.Text = "Restart";
            restartToolStripMenuItem.Click += restartToolStripMenuItem_Click;
            // 
            // standbyToolStripMenuItem
            // 
            standbyToolStripMenuItem.Name = "standbyToolStripMenuItem";
            standbyToolStripMenuItem.Size = new Size(180, 22);
            standbyToolStripMenuItem.Text = "Standby";
            standbyToolStripMenuItem.Click += standbyToolStripMenuItem_Click;
            // 
            // monitoringToolStripMenuItem
            // 
            monitoringToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { keyloggerToolStripMenuItem, remoteDesktopToolStripMenuItem });
            monitoringToolStripMenuItem.Name = "monitoringToolStripMenuItem";
            monitoringToolStripMenuItem.Size = new Size(179, 22);
            monitoringToolStripMenuItem.Text = "Monitoring";
            // 
            //// keyloggerToolStripMenuItem
            //// 
            //keyloggerToolStripMenuItem.Name = "keyloggerToolStripMenuItem";
            //keyloggerToolStripMenuItem.Size = new Size(161, 22);
            //keyloggerToolStripMenuItem.Text = "Keylogger";
            //keyloggerToolStripMenuItem.Click += keyloggerToolStripMenuItem_Click;
            //// 
            // remoteDesktopToolStripMenuItem
            // 
            remoteDesktopToolStripMenuItem.Name = "remoteDesktopToolStripMenuItem";
            remoteDesktopToolStripMenuItem.Size = new Size(161, 22);
            remoteDesktopToolStripMenuItem.Text = "Remote Desktop";
            remoteDesktopToolStripMenuItem.Click += remoteDesktopToolStripMenuItem_Click;
            // 
            // userSupportToolStripMenuItem
            // 
            userSupportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showMessageboxToolStripMenuItem });
            userSupportToolStripMenuItem.Name = "userSupportToolStripMenuItem";
            userSupportToolStripMenuItem.Size = new Size(179, 22);
            userSupportToolStripMenuItem.Text = "User Support";
            // 
            //// showMessageboxToolStripMenuItem
            //// 
            //showMessageboxToolStripMenuItem.Name = "showMessageboxToolStripMenuItem";
            //showMessageboxToolStripMenuItem.Size = new Size(172, 22);
            //showMessageboxToolStripMenuItem.Text = "Show Messagebox";
            //showMessageboxToolStripMenuItem.Click += showMessageboxToolStripMenuItem_Click;
            //// 
            // clientManagementToolStripMenuItem
            // 
            clientManagementToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { elevateClientPermissionsToolStripMenuItem, reconnectToolStripMenuItem, disconnectToolStripMenuItem });
            clientManagementToolStripMenuItem.Name = "clientManagementToolStripMenuItem";
            clientManagementToolStripMenuItem.Size = new Size(179, 22);
            clientManagementToolStripMenuItem.Text = "Client Management";
            // 
            // elevateClientPermissionsToolStripMenuItem
            // 
            elevateClientPermissionsToolStripMenuItem.Name = "elevateClientPermissionsToolStripMenuItem";
            elevateClientPermissionsToolStripMenuItem.Size = new Size(211, 22);
            elevateClientPermissionsToolStripMenuItem.Text = "Elevate Client Permissions";
            elevateClientPermissionsToolStripMenuItem.Click += elevateClientPermissionsToolStripMenuItem_Click;
            // 
            // reconnectToolStripMenuItem
            // 
            reconnectToolStripMenuItem.Name = "reconnectToolStripMenuItem";
            reconnectToolStripMenuItem.Size = new Size(211, 22);
            reconnectToolStripMenuItem.Text = "Reconnect";
            reconnectToolStripMenuItem.Click += reconnectToolStripMenuItem_Click;
            // 
            // disconnectToolStripMenuItem
            // 
            disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            disconnectToolStripMenuItem.Size = new Size(211, 22);
            disconnectToolStripMenuItem.Text = "Disconnect";
            disconnectToolStripMenuItem.Click += disconnectToolStripMenuItem_Click;
            // 
            // lstClients
            // 
            lstClients.Columns.AddRange(new ColumnHeader[] { hIp, hUserPC, hOS, hCountry, hStatus, hUserStatus, hAccountType });
            lstClients.ContextMenuStrip = contextMenuStrip;
            lstClients.Dock = DockStyle.Fill;
            lstClients.FullRowSelect = true;
            lstClients.Location = new Point(3, 3);
            lstClients.Name = "lstClients";
            lstClients.ShowItemToolTips = true;
            lstClients.Size = new Size(1018, 431);
            lstClients.TabIndex = 0;
            lstClients.UseCompatibleStateImageBehavior = false;
            lstClients.View = View.Details;
            lstClients.SelectedIndexChanged += lstClients_SelectedIndexChanged;
            // 
            // hIp
            // 
            hIp.Text = "IP Address";
            hIp.Width = 120;
            // 
            // hUserPC
            // 
            hUserPC.Text = "User@PC";
            hUserPC.Width = 220;
            // 
            // hOS
            // 
            hOS.Text = "Operating System";
            hOS.Width = 250;
            // 
            // hCountry
            // 
            hCountry.Text = "Country";
            hCountry.Width = 120;
            // 
            // hStatus
            // 
            hStatus.Text = "Status";
            hStatus.Width = 100;
            // 
            // hUserStatus
            // 
            hUserStatus.Text = "User Status";
            hUserStatus.Width = 100;
            // 
            // hAccountType
            // 
            hAccountType.Text = "Account Type";
            hAccountType.Width = 100;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(lstClients, 0, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tableLayoutPanel.Size = new Size(1024, 437);
            tableLayoutPanel.TabIndex = 6;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1024, 437);
            Controls.Add(tableLayoutPanel);
            MinimumSize = new Size(680, 415);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Server";
            FormClosing += FrmMain_FormClosing;
            Load += FrmMain_Load;
            contextMenuStrip.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView lstClients;
        private ColumnHeader hIp;
        private ColumnHeader hUserPC;
        private ColumnHeader hOS;
        private ColumnHeader hCountry;
        private TableLayoutPanel tableLayoutPanel;
        private ColumnHeader hAccountType;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem systemToolStripMenuItem;
        private ToolStripMenuItem systemInformationToolStripMenuItem;
        private ToolStripMenuItem fileManagerToolStripMenuItem;
        private ToolStripMenuItem taskManagerToolStripMenuItem;
        private ToolStripMenuItem remoteShellToolStripMenuItem;
        private ToolStripMenuItem actionsToolStripMenuItem;
        private ToolStripMenuItem monitoringToolStripMenuItem;
        private ToolStripMenuItem shutdownToolStripMenuItem;
        private ToolStripMenuItem restartToolStripMenuItem;
        private ToolStripMenuItem standbyToolStripMenuItem;
        private ToolStripMenuItem keyloggerToolStripMenuItem;
        private ToolStripMenuItem remoteDesktopToolStripMenuItem;
        private ToolStripMenuItem userSupportToolStripMenuItem;
        private ToolStripMenuItem showMessageboxToolStripMenuItem;
        private ToolStripMenuItem clientManagementToolStripMenuItem;
        private ToolStripMenuItem elevateClientPermissionsToolStripMenuItem;
        private ToolStripMenuItem reconnectToolStripMenuItem;
        private ToolStripMenuItem disconnectToolStripMenuItem;
        private ColumnHeader hStatus;
        private ColumnHeader hUserStatus;
    }
}
