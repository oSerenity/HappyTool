
namespace HappyTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Decompress = new System.Windows.Forms.GroupBox();
            this.Compress = new System.Windows.Forms.GroupBox();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
            this.TurnOffServer = new DevExpress.XtraEditors.CheckButton();
            this.Patchexe = new DevExpress.XtraEditors.GroupControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.Port = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.Address = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.BlowfishEncryption = new DevExpress.XtraEditors.GroupControl();
            this.BlowfishDecryption = new DevExpress.XtraEditors.GroupControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.xtraTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Patchexe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Port.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Address.Properties)).BeginInit();
            this.xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlowfishEncryption)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlowfishDecryption)).BeginInit();
            this.SuspendLayout();
            // 
            // Decompress
            // 
            this.Decompress.Dock = System.Windows.Forms.DockStyle.Left;
            this.Decompress.ForeColor = System.Drawing.Color.White;
            this.Decompress.Location = new System.Drawing.Point(0, 0);
            this.Decompress.Name = "Decompress";
            this.Decompress.Size = new System.Drawing.Size(245, 127);
            this.Decompress.TabIndex = 0;
            this.Decompress.TabStop = false;
            this.Decompress.Text = "Decompress";
            this.Decompress.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragDrop);
            this.Decompress.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragEnter);
            // 
            // Compress
            // 
            this.Compress.Dock = System.Windows.Forms.DockStyle.Right;
            this.Compress.ForeColor = System.Drawing.Color.White;
            this.Compress.Location = new System.Drawing.Point(251, 0);
            this.Compress.Name = "Compress";
            this.Compress.Size = new System.Drawing.Size(259, 127);
            this.Compress.TabIndex = 1;
            this.Compress.TabStop = false;
            this.Compress.Text = "Compress";
            this.Compress.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragDrop);
            this.Compress.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragEnter);
            // 
            // defaultLookAndFeel1
            // 
            this.defaultLookAndFeel1.LookAndFeel.SkinName = "Visual Studio 2013 Dark";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(512, 154);
            this.xtraTabControl1.TabIndex = 2;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage3,
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.Decompress);
            this.xtraTabPage1.Controls.Add(this.Compress);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(510, 127);
            this.xtraTabPage1.Text = "Zlib (Xbox)";
            // 
            // xtraTabPage3
            // 
            this.xtraTabPage3.Controls.Add(this.TurnOffServer);
            this.xtraTabPage3.Controls.Add(this.Patchexe);
            this.xtraTabPage3.Controls.Add(this.labelControl4);
            this.xtraTabPage3.Controls.Add(this.Port);
            this.xtraTabPage3.Controls.Add(this.labelControl3);
            this.xtraTabPage3.Controls.Add(this.Address);
            this.xtraTabPage3.Controls.Add(this.labelControl2);
            this.xtraTabPage3.Controls.Add(this.labelControl1);
            this.xtraTabPage3.Controls.Add(this.simpleButton1);
            this.xtraTabPage3.Name = "xtraTabPage3";
            this.xtraTabPage3.Size = new System.Drawing.Size(510, 127);
            this.xtraTabPage3.Text = "Steam Server";
            // 
            // TurnOffServer
            // 
            this.TurnOffServer.AllowFocus = false;
            this.TurnOffServer.Location = new System.Drawing.Point(399, 89);
            this.TurnOffServer.Name = "TurnOffServer";
            this.TurnOffServer.Size = new System.Drawing.Size(75, 23);
            this.TurnOffServer.TabIndex = 13;
            this.TurnOffServer.Text = "Stop Sever...";
            this.TurnOffServer.CheckedChanged += new System.EventHandler(this.TurnOffSever_CheckedChanged);
            // 
            // Patchexe
            // 
            this.Patchexe.AllowDrop = true;
            this.Patchexe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Patchexe.Location = new System.Drawing.Point(307, 5);
            this.Patchexe.Name = "Patchexe";
            this.Patchexe.Size = new System.Drawing.Size(200, 80);
            this.Patchexe.TabIndex = 11;
            this.Patchexe.Text = "Patch EXE";
            this.Patchexe.DragDrop += new System.Windows.Forms.DragEventHandler(this.Patchexe_DragDrop);
            this.Patchexe.DragEnter += new System.Windows.Forms.DragEventHandler(this.Patchexe_DragEnter);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(11, 72);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(24, 13);
            this.labelControl4.TabIndex = 7;
            this.labelControl4.Text = "Port:";
            // 
            // Port
            // 
            this.Port.EditValue = "12345";
            this.Port.Location = new System.Drawing.Point(11, 91);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(100, 20);
            this.Port.TabIndex = 6;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(11, 27);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(78, 13);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Server Address:";
            // 
            // Address
            // 
            this.Address.EditValue = "localhost";
            this.Address.Location = new System.Drawing.Point(11, 46);
            this.Address.Name = "Address";
            this.Address.Size = new System.Drawing.Size(100, 20);
            this.Address.TabIndex = 4;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(52, 5);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(25, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Null..";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(11, 5);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(35, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Status:";
            // 
            // simpleButton1
            // 
            this.simpleButton1.AllowFocus = false;
            this.simpleButton1.Location = new System.Drawing.Point(307, 89);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(86, 23);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "Start Sever...";
            this.simpleButton1.Click += new System.EventHandler(this.StartServer_Click);
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Controls.Add(this.BlowfishEncryption);
            this.xtraTabPage2.Controls.Add(this.BlowfishDecryption);
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(510, 127);
            this.xtraTabPage2.Text = "Blowfish (Steam)";
            // 
            // BlowfishEncryption
            // 
            this.BlowfishEncryption.AllowDrop = true;
            this.BlowfishEncryption.Dock = System.Windows.Forms.DockStyle.Right;
            this.BlowfishEncryption.Location = new System.Drawing.Point(237, 0);
            this.BlowfishEncryption.Name = "BlowfishEncryption";
            this.BlowfishEncryption.Size = new System.Drawing.Size(273, 127);
            this.BlowfishEncryption.TabIndex = 1;
            this.BlowfishEncryption.Text = "Encrypt";
            this.BlowfishEncryption.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragDrop);
            this.BlowfishEncryption.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragEnter);
            // 
            // BlowfishDecryption
            // 
            this.BlowfishDecryption.AllowDrop = true;
            this.BlowfishDecryption.Dock = System.Windows.Forms.DockStyle.Left;
            this.BlowfishDecryption.Location = new System.Drawing.Point(0, 0);
            this.BlowfishDecryption.Name = "BlowfishDecryption";
            this.BlowfishDecryption.Size = new System.Drawing.Size(231, 127);
            this.BlowfishDecryption.TabIndex = 0;
            this.BlowfishDecryption.Text = "Decrypt";
            this.BlowfishDecryption.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragDrop);
            this.BlowfishDecryption.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dropbox_DragEnter);
            // 
            // timer1
            // 
            this.timer1.Interval = 2500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 154);
            this.Controls.Add(this.xtraTabControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("MainForm.IconOptions.Image")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Happy Tool - By TeddyHammer/Aka SDKSerenity";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.xtraTabPage3.ResumeLayout(false);
            this.xtraTabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Patchexe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Port.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Address.Properties)).EndInit();
            this.xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BlowfishEncryption)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlowfishDecryption)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Decompress;
        private System.Windows.Forms.GroupBox Compress;
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.GroupControl BlowfishEncryption;
        private DevExpress.XtraEditors.GroupControl BlowfishDecryption;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit Address;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit Port;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraEditors.GroupControl Patchexe;
        public DevExpress.XtraEditors.CheckButton TurnOffServer;
    }
}

