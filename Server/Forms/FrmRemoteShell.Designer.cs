namespace Server.Forms
{
    partial class FrmRemoteShell
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
            txtConsoleOutput = new RichTextBox();
            txtConsoleInput = new TextBox();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // txtConsoleOutput
            // 
            txtConsoleOutput.BackColor = Color.Black;
            txtConsoleOutput.BorderStyle = BorderStyle.None;
            txtConsoleOutput.Dock = DockStyle.Fill;
            txtConsoleOutput.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtConsoleOutput.ForeColor = Color.WhiteSmoke;
            txtConsoleOutput.Location = new Point(3, 3);
            txtConsoleOutput.Name = "txtConsoleOutput";
            txtConsoleOutput.ReadOnly = true;
            txtConsoleOutput.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtConsoleOutput.Size = new Size(767, 382);
            txtConsoleOutput.TabIndex = 1;
            txtConsoleOutput.Text = "";
            txtConsoleOutput.TextChanged += txtConsoleOutput_TextChanged;
            txtConsoleOutput.KeyPress += txtConsoleOutput_KeyPress;
            // 
            // txtConsoleInput
            // 
            txtConsoleInput.BackColor = Color.Black;
            txtConsoleInput.BorderStyle = BorderStyle.None;
            txtConsoleInput.Dock = DockStyle.Fill;
            txtConsoleInput.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtConsoleInput.ForeColor = Color.WhiteSmoke;
            txtConsoleInput.Location = new Point(3, 391);
            txtConsoleInput.MaxLength = 200;
            txtConsoleInput.Name = "txtConsoleInput";
            txtConsoleInput.Size = new Size(767, 20);
            txtConsoleInput.TabIndex = 0;
            txtConsoleInput.TextChanged += txtConsoleInput_TextChanged;
            txtConsoleInput.KeyDown += txtConsoleInput_KeyDown;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.BackColor = Color.Black;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtConsoleOutput, 0, 0);
            tableLayoutPanel.Controls.Add(txtConsoleInput, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new Size(773, 408);
            tableLayoutPanel.TabIndex = 2;
            // 
            // FrmRemoteShell
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(773, 408);
            Controls.Add(tableLayoutPanel);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "FrmRemoteShell";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Remote Shell []";
            FormClosing += FrmRemoteShell_FormClosing;
            Load += FrmRemoteShell_Load;
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TextBox txtConsoleInput;
        private System.Windows.Forms.RichTextBox txtConsoleOutput;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}