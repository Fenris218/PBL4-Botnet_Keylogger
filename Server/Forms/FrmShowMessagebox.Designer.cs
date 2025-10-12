namespace Server.Forms
{
    partial class FrmShowMessagebox
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
            groupMsgSettings = new GroupBox();
            cmbMsgIcon = new ComboBox();
            lblMsgIcon = new Label();
            cmbMsgButtons = new ComboBox();
            lblMsgButtons = new Label();
            txtText = new TextBox();
            txtCaption = new TextBox();
            lblText = new Label();
            lblCaption = new Label();
            btnPreview = new Button();
            btnSend = new Button();
            groupMsgSettings.SuspendLayout();
            SuspendLayout();
            // 
            // groupMsgSettings
            // 
            groupMsgSettings.Controls.Add(cmbMsgIcon);
            groupMsgSettings.Controls.Add(lblMsgIcon);
            groupMsgSettings.Controls.Add(cmbMsgButtons);
            groupMsgSettings.Controls.Add(lblMsgButtons);
            groupMsgSettings.Controls.Add(txtText);
            groupMsgSettings.Controls.Add(txtCaption);
            groupMsgSettings.Controls.Add(lblText);
            groupMsgSettings.Controls.Add(lblCaption);
            groupMsgSettings.Location = new Point(12, 12);
            groupMsgSettings.Name = "groupMsgSettings";
            groupMsgSettings.Size = new Size(325, 146);
            groupMsgSettings.TabIndex = 0;
            groupMsgSettings.TabStop = false;
            groupMsgSettings.Text = "Messagebox Settings";
            // 
            // cmbMsgIcon
            // 
            cmbMsgIcon.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMsgIcon.FormattingEnabled = true;
            cmbMsgIcon.Location = new Point(147, 107);
            cmbMsgIcon.Name = "cmbMsgIcon";
            cmbMsgIcon.Size = new Size(162, 21);
            cmbMsgIcon.TabIndex = 8;
            // 
            // lblMsgIcon
            // 
            lblMsgIcon.AutoSize = true;
            lblMsgIcon.Location = new Point(42, 110);
            lblMsgIcon.Name = "lblMsgIcon";
            lblMsgIcon.Size = new Size(99, 13);
            lblMsgIcon.TabIndex = 7;
            lblMsgIcon.Text = "Messagebox Icon:";
            // 
            // cmbMsgButtons
            // 
            cmbMsgButtons.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMsgButtons.FormattingEnabled = true;
            cmbMsgButtons.Location = new Point(147, 80);
            cmbMsgButtons.Name = "cmbMsgButtons";
            cmbMsgButtons.Size = new Size(162, 21);
            cmbMsgButtons.TabIndex = 6;
            // 
            // lblMsgButtons
            // 
            lblMsgButtons.AutoSize = true;
            lblMsgButtons.Location = new Point(23, 83);
            lblMsgButtons.Name = "lblMsgButtons";
            lblMsgButtons.Size = new Size(117, 13);
            lblMsgButtons.TabIndex = 5;
            lblMsgButtons.Text = "Messagebox Buttons:";
            // 
            // txtText
            // 
            txtText.Location = new Point(60, 49);
            txtText.MaxLength = 256;
            txtText.Name = "txtText";
            txtText.Size = new Size(249, 22);
            txtText.TabIndex = 4;
            txtText.Text = "Hello";
            // 
            // txtCaption
            // 
            txtCaption.Location = new Point(60, 21);
            txtCaption.MaxLength = 256;
            txtCaption.Name = "txtCaption";
            txtCaption.Size = new Size(249, 22);
            txtCaption.TabIndex = 2;
            txtCaption.Text = "Information";
            // 
            // lblText
            // 
            lblText.AutoSize = true;
            lblText.Location = new Point(24, 52);
            lblText.Name = "lblText";
            lblText.Size = new Size(30, 13);
            lblText.TabIndex = 3;
            lblText.Text = "Text:";
            // 
            // lblCaption
            // 
            lblCaption.AutoSize = true;
            lblCaption.Location = new Point(6, 24);
            lblCaption.Name = "lblCaption";
            lblCaption.Size = new Size(51, 13);
            lblCaption.TabIndex = 1;
            lblCaption.Text = "Caption:";
            // 
            // btnPreview
            // 
            btnPreview.Location = new Point(181, 164);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(75, 23);
            btnPreview.TabIndex = 1;
            btnPreview.Text = "Preview";
            btnPreview.UseVisualStyleBackColor = true;
            btnPreview.Click += btnPreview_Click;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(262, 164);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // FrmShowMessagebox
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(349, 199);
            Controls.Add(btnSend);
            Controls.Add(btnPreview);
            Controls.Add(groupMsgSettings);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmShowMessagebox";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Show Messagebox []";
            Load += FrmShowMessagebox_Load;
            groupMsgSettings.ResumeLayout(false);
            groupMsgSettings.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupMsgSettings;
        private System.Windows.Forms.ComboBox cmbMsgIcon;
        private System.Windows.Forms.Label lblMsgIcon;
        private System.Windows.Forms.ComboBox cmbMsgButtons;
        private System.Windows.Forms.Label lblMsgButtons;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.TextBox txtCaption;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSend;
    }
}