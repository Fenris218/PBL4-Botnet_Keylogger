namespace Server.Forms
{
    partial class FrmSystemInformation
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
            lstSystem = new ListView();
            hComponent = new ColumnHeader();
            hValue = new ColumnHeader();
            contextMenuStrip = new ContextMenuStrip(components);
            copyToClipboardToolStripMenuItem = new ToolStripMenuItem();
            copyAllToolStripMenuItem = new ToolStripMenuItem();
            copySelectedToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // lstSystem
            // 
            lstSystem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstSystem.Columns.AddRange(new ColumnHeader[] { hComponent, hValue });
            lstSystem.ContextMenuStrip = contextMenuStrip;
            lstSystem.FullRowSelect = true;
            lstSystem.GridLines = true;
            lstSystem.Location = new Point(12, 12);
            lstSystem.Name = "lstSystem";
            lstSystem.Size = new Size(661, 353);
            lstSystem.TabIndex = 0;
            lstSystem.UseCompatibleStateImageBehavior = false;
            lstSystem.View = View.Details;
            // 
            // hComponent
            // 
            hComponent.Text = "Component";
            hComponent.Width = 200;
            // 
            // hValue
            // 
            hValue.Text = "Value";
            hValue.Width = 400;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { copyToClipboardToolStripMenuItem, toolStripMenuItem2, refreshToolStripMenuItem });
            contextMenuStrip.Name = "ctxtMenu";
            contextMenuStrip.Size = new Size(172, 54);
            // 
            // copyToClipboardToolStripMenuItem
            // 
            copyToClipboardToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { copyAllToolStripMenuItem, copySelectedToolStripMenuItem });
            copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            copyToClipboardToolStripMenuItem.Size = new Size(171, 22);
            copyToClipboardToolStripMenuItem.Text = "Copy to Clipboard";
            // 
            // copyAllToolStripMenuItem
            // 
            copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            copyAllToolStripMenuItem.Size = new Size(118, 22);
            copyAllToolStripMenuItem.Text = "All";
            copyAllToolStripMenuItem.Click += copyAllToolStripMenuItem_Click;
            // 
            // copySelectedToolStripMenuItem
            // 
            copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            copySelectedToolStripMenuItem.Size = new Size(118, 22);
            copySelectedToolStripMenuItem.Text = "Selected";
            copySelectedToolStripMenuItem.Click += copySelectedToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(168, 6);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(171, 22);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
            // 
            // FrmSystemInformation
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(685, 377);
            Controls.Add(lstSystem);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MinimumSize = new Size(576, 373);
            Name = "FrmSystemInformation";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "System Information";
            FormClosing += FrmSystemInformation_FormClosing;
            Load += FrmSystemInformation_Load;
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ColumnHeader hComponent;
        private System.Windows.Forms.ColumnHeader hValue;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private System.Windows.Forms.ListView lstSystem;
        private System.Windows.Forms.ToolStripMenuItem copyAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}