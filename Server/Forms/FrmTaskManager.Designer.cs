namespace Server.Forms
{
    partial class FrmTaskManager
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
            contextMenuStrip = new ContextMenuStrip(components);
            killProcessToolStripMenuItem = new ToolStripMenuItem();
            startProcessToolStripMenuItem = new ToolStripMenuItem();
            lineToolStripMenuItem = new ToolStripSeparator();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel = new TableLayoutPanel();
            lstTasks = new ListView();
            hProcessname = new ColumnHeader();
            hPID = new ColumnHeader();
            hTitle = new ColumnHeader();
            statusStrip = new StatusStrip();
            processesToolStripStatusLabel = new ToolStripStatusLabel();
            contextMenuStrip.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { killProcessToolStripMenuItem, startProcessToolStripMenuItem, lineToolStripMenuItem, refreshToolStripMenuItem });
            contextMenuStrip.Name = "ctxtMenu";
            contextMenuStrip.Size = new Size(142, 76);
            // 
            // killProcessToolStripMenuItem
            // 
            killProcessToolStripMenuItem.Name = "killProcessToolStripMenuItem";
            killProcessToolStripMenuItem.Size = new Size(141, 22);
            killProcessToolStripMenuItem.Text = "Kill Process";
            killProcessToolStripMenuItem.Click += killProcessToolStripMenuItem_Click;
            // 
            // startProcessToolStripMenuItem
            // 
            startProcessToolStripMenuItem.Name = "startProcessToolStripMenuItem";
            startProcessToolStripMenuItem.Size = new Size(141, 22);
            startProcessToolStripMenuItem.Text = "Start Process";
            startProcessToolStripMenuItem.Click += startProcessToolStripMenuItem_Click;
            // 
            // lineToolStripMenuItem
            // 
            lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            lineToolStripMenuItem.Size = new Size(138, 6);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(141, 22);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(lstTasks, 0, 0);
            tableLayoutPanel.Controls.Add(statusStrip, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tableLayoutPanel.Size = new Size(821, 493);
            tableLayoutPanel.TabIndex = 2;
            // 
            // lstTasks
            // 
            lstTasks.Columns.AddRange(new ColumnHeader[] { hProcessname, hPID, hTitle });
            lstTasks.ContextMenuStrip = contextMenuStrip;
            lstTasks.Dock = DockStyle.Fill;
            lstTasks.FullRowSelect = true;
            lstTasks.GridLines = true;
            lstTasks.Location = new Point(3, 3);
            lstTasks.Name = "lstTasks";
            lstTasks.Size = new Size(815, 465);
            lstTasks.TabIndex = 1;
            lstTasks.UseCompatibleStateImageBehavior = false;
            lstTasks.View = View.Details;
            // 
            // hProcessname
            // 
            hProcessname.Text = "Process Name";
            hProcessname.Width = 202;
            // 
            // hPID
            // 
            hPID.Text = "PID";
            // 
            // hTitle
            // 
            hTitle.Text = "Title";
            hTitle.Width = 531;
            // 
            // statusStrip
            // 
            statusStrip.Dock = DockStyle.Fill;
            statusStrip.Items.AddRange(new ToolStripItem[] { processesToolStripStatusLabel });
            statusStrip.Location = new Point(0, 471);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(821, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip1";
            // 
            // processesToolStripStatusLabel
            // 
            processesToolStripStatusLabel.Name = "processesToolStripStatusLabel";
            processesToolStripStatusLabel.Size = new Size(70, 17);
            processesToolStripStatusLabel.Text = "Processes: 0";
            // 
            // FrmTaskManager
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(821, 493);
            Controls.Add(tableLayoutPanel);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MinimumSize = new Size(351, 449);
            Name = "FrmTaskManager";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Task Manager []";
            FormClosing += FrmTaskManager_FormClosing;
            Load += FrmTaskManager_Load;
            contextMenuStrip.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem killProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startProcessToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader hProcessname;
        private System.Windows.Forms.ColumnHeader hPID;
        private System.Windows.Forms.ColumnHeader hTitle;
        private System.Windows.Forms.ToolStripSeparator lineToolStripMenuItem;
        private ListView lstTasks;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel processesToolStripStatusLabel;
    }
}