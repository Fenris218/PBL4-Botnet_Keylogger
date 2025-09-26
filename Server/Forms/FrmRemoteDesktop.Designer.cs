using Server.Controls;
using System.Windows.Forms;

namespace Server.Forms
{
    partial class FrmRemoteDesktop
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
            btnStart = new Button();
            btnStop = new Button();
            barQuality = new TrackBar();
            lblQuality = new Label();
            lblQualityShow = new Label();
            btnMouse = new Button();
            panelTop = new Panel();
            btnKeyboard = new Button();
            cbMonitors = new ComboBox();
            btnHide = new Button();
            btnShow = new Button();
            toolTipButtons = new ToolTip(components);
            picDesktop = new RapidPictureBox();
            ((System.ComponentModel.ISupportInitialize)barQuality).BeginInit();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picDesktop).BeginInit();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(15, 5);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(68, 23);
            btnStart.TabIndex = 1;
            btnStart.TabStop = false;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(96, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(68, 23);
            btnStop.TabIndex = 2;
            btnStop.TabStop = false;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // barQuality
            // 
            barQuality.Location = new Point(206, -1);
            barQuality.Maximum = 100;
            barQuality.Minimum = 1;
            barQuality.Name = "barQuality";
            barQuality.Size = new Size(76, 56);
            barQuality.TabIndex = 3;
            barQuality.TabStop = false;
            barQuality.Value = 40;
            barQuality.Scroll += barQuality_Scroll;
            // 
            // lblQuality
            // 
            lblQuality.AutoSize = true;
            lblQuality.Location = new Point(167, 5);
            lblQuality.Name = "lblQuality";
            lblQuality.Size = new Size(56, 19);
            lblQuality.TabIndex = 4;
            lblQuality.Text = "Quality:";
            // 
            // lblQualityShow
            // 
            lblQualityShow.AutoSize = true;
            lblQualityShow.Location = new Point(220, 26);
            lblQualityShow.Name = "lblQualityShow";
            lblQualityShow.Size = new Size(64, 19);
            lblQualityShow.TabIndex = 5;
            lblQualityShow.Text = "75 (high)";
            // 
            // btnMouse
            // 
            btnMouse.Image = Properties.Resources.mouse_delete;
            btnMouse.Location = new Point(302, 5);
            btnMouse.Name = "btnMouse";
            btnMouse.Size = new Size(28, 28);
            btnMouse.TabIndex = 6;
            btnMouse.TabStop = false;
            toolTipButtons.SetToolTip(btnMouse, "Enable mouse input.");
            btnMouse.UseVisualStyleBackColor = true;
            btnMouse.Click += btnMouse_Click;
            // 
            // panelTop
            // 
            panelTop.BorderStyle = BorderStyle.FixedSingle;
            panelTop.Controls.Add(btnKeyboard);
            panelTop.Controls.Add(cbMonitors);
            panelTop.Controls.Add(btnHide);
            panelTop.Controls.Add(lblQualityShow);
            panelTop.Controls.Add(btnMouse);
            panelTop.Controls.Add(btnStart);
            panelTop.Controls.Add(btnStop);
            panelTop.Controls.Add(lblQuality);
            panelTop.Controls.Add(barQuality);
            panelTop.Location = new Point(189, -1);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(384, 57);
            panelTop.TabIndex = 7;
            // 
            // btnKeyboard
            // 
            btnKeyboard.Image = Properties.Resources.keyboard_delete;
            btnKeyboard.Location = new Point(336, 5);
            btnKeyboard.Name = "btnKeyboard";
            btnKeyboard.Size = new Size(28, 28);
            btnKeyboard.TabIndex = 9;
            btnKeyboard.TabStop = false;
            toolTipButtons.SetToolTip(btnKeyboard, "Enable keyboard input.");
            btnKeyboard.UseVisualStyleBackColor = true;
            btnKeyboard.Click += btnKeyboard_Click;
            // 
            // cbMonitors
            // 
            cbMonitors.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMonitors.FormattingEnabled = true;
            cbMonitors.Location = new Point(15, 30);
            cbMonitors.Name = "cbMonitors";
            cbMonitors.Size = new Size(149, 27);
            cbMonitors.TabIndex = 8;
            cbMonitors.TabStop = false;
            // 
            // btnHide
            // 
            btnHide.Location = new Point(170, 37);
            btnHide.Name = "btnHide";
            btnHide.Size = new Size(54, 19);
            btnHide.TabIndex = 7;
            btnHide.TabStop = false;
            btnHide.Text = "Hide";
            btnHide.UseVisualStyleBackColor = true;
            btnHide.Click += btnHide_Click;
            // 
            // btnShow
            // 
            btnShow.Location = new Point(0, 0);
            btnShow.Name = "btnShow";
            btnShow.Size = new Size(54, 19);
            btnShow.TabIndex = 8;
            btnShow.TabStop = false;
            btnShow.Text = "Show";
            btnShow.UseVisualStyleBackColor = true;
            btnShow.Visible = false;
            btnShow.Click += btnShow_Click;
            // 
            // picDesktop
            // 
            picDesktop.BackColor = Color.Black;
            picDesktop.BorderStyle = BorderStyle.FixedSingle;
            picDesktop.Dock = DockStyle.Fill;
            picDesktop.GetImageSafe = null;
            picDesktop.Location = new Point(0, 0);
            picDesktop.Name = "picDesktop";
            picDesktop.Running = false;
            picDesktop.Size = new Size(784, 562);
            picDesktop.SizeMode = PictureBoxSizeMode.StretchImage;
            picDesktop.TabIndex = 0;
            picDesktop.TabStop = false;
            picDesktop.MouseDown += picDesktop_MouseDown;
            picDesktop.MouseMove += picDesktop_MouseMove;
            picDesktop.MouseUp += picDesktop_MouseUp;
            // 
            // FrmRemoteDesktop
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(784, 562);
            Controls.Add(btnShow);
            Controls.Add(panelTop);
            Controls.Add(picDesktop);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            KeyPreview = true;
            MinimumSize = new Size(640, 480);
            Name = "FrmRemoteDesktop";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Remote Desktop []";
            FormClosing += FrmRemoteDesktop_FormClosing;
            Load += FrmRemoteDesktop_Load;
            Resize += FrmRemoteDesktop_Resize;
            ((System.ComponentModel.ISupportInitialize)barQuality).EndInit();
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picDesktop).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TrackBar barQuality;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.Label lblQualityShow;
        private System.Windows.Forms.Button btnMouse;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.ComboBox cbMonitors;
        private System.Windows.Forms.Button btnKeyboard;
        private System.Windows.Forms.ToolTip toolTipButtons;
        private Controls.RapidPictureBox picDesktop;
    }
}