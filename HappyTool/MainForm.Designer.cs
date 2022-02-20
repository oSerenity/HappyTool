
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
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.TurnOffServer = new DevExpress.XtraEditors.CheckButton();
            this.Patchexe = new DevExpress.XtraEditors.GroupControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton6 = new DevExpress.XtraEditors.SimpleButton();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.simpleButton5 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
            this.textEdit2 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit3 = new DevExpress.XtraEditors.TextEdit();
            this.simpleButton7 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton8 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton9 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton10 = new DevExpress.XtraEditors.SimpleButton();
            this.textEdit4 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit5 = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Patchexe)).BeginInit();
            this.xtraTabPage1.SuspendLayout();
            this.xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit5.Properties)).BeginInit();
            this.SuspendLayout();
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
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage3;
            this.xtraTabControl1.Size = new System.Drawing.Size(486, 143);
            this.xtraTabControl1.TabIndex = 2;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage3,
            this.xtraTabPage1,
            this.xtraTabPage2});
            this.xtraTabControl1.Selected += new DevExpress.XtraTab.TabPageEventHandler(this.xtraTabControl1_Selected);
            // 
            // xtraTabPage3
            // 
            this.xtraTabPage3.Controls.Add(this.simpleButton2);
            this.xtraTabPage3.Controls.Add(this.labelControl3);
            this.xtraTabPage3.Controls.Add(this.textEdit1);
            this.xtraTabPage3.Controls.Add(this.TurnOffServer);
            this.xtraTabPage3.Controls.Add(this.Patchexe);
            this.xtraTabPage3.Controls.Add(this.labelControl2);
            this.xtraTabPage3.Controls.Add(this.labelControl1);
            this.xtraTabPage3.Controls.Add(this.simpleButton1);
            this.xtraTabPage3.Name = "xtraTabPage3";
            this.xtraTabPage3.Size = new System.Drawing.Size(484, 116);
            this.xtraTabPage3.Text = "Steam Server";
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(117, 64);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(26, 23);
            this.simpleButton2.TabIndex = 16;
            this.simpleButton2.Text = "Set";
            this.simpleButton2.Click += new System.EventHandler(this.SetTickets_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(11, 46);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(91, 13);
            this.labelControl3.TabIndex = 15;
            this.labelControl3.Text = "Set Ticket Number:";
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(11, 65);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Properties.MaxLength = 3;
            this.textEdit1.Size = new System.Drawing.Size(100, 20);
            this.textEdit1.TabIndex = 14;
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
            this.Patchexe.Size = new System.Drawing.Size(166, 80);
            this.Patchexe.TabIndex = 11;
            this.Patchexe.Text = "Patch EXE Or dll";
            this.Patchexe.DragDrop += new System.Windows.Forms.DragEventHandler(this.Patchexe_DragDrop);
            this.Patchexe.DragEnter += new System.Windows.Forms.DragEventHandler(this.Patchexe_DragEnter);
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
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.simpleButton8);
            this.xtraTabPage1.Controls.Add(this.simpleButton7);
            this.xtraTabPage1.Controls.Add(this.textEdit3);
            this.xtraTabPage1.Controls.Add(this.textEdit2);
            this.xtraTabPage1.Controls.Add(this.simpleButton3);
            this.xtraTabPage1.Controls.Add(this.simpleButton6);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(484, 116);
            this.xtraTabPage1.Text = "Xbox Files";
            // 
            // simpleButton3
            // 
            this.simpleButton3.Location = new System.Drawing.Point(136, 8);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(122, 23);
            this.simpleButton3.TabIndex = 24;
            this.simpleButton3.Text = "Compress Folder";
            this.simpleButton3.Click += new System.EventHandler(this.CompressFolder);
            // 
            // simpleButton6
            // 
            this.simpleButton6.Location = new System.Drawing.Point(8, 8);
            this.simpleButton6.Name = "simpleButton6";
            this.simpleButton6.Size = new System.Drawing.Size(122, 23);
            this.simpleButton6.TabIndex = 23;
            this.simpleButton6.Text = "Decompress Folder";
            this.simpleButton6.Click += new System.EventHandler(this.DecompressFolder);
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Controls.Add(this.simpleButton9);
            this.xtraTabPage2.Controls.Add(this.simpleButton10);
            this.xtraTabPage2.Controls.Add(this.textEdit4);
            this.xtraTabPage2.Controls.Add(this.textEdit5);
            this.xtraTabPage2.Controls.Add(this.simpleButton5);
            this.xtraTabPage2.Controls.Add(this.simpleButton4);
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(484, 116);
            this.xtraTabPage2.Text = "Steam Files";
            // 
            // simpleButton5
            // 
            this.simpleButton5.Location = new System.Drawing.Point(137, 7);
            this.simpleButton5.Name = "simpleButton5";
            this.simpleButton5.Size = new System.Drawing.Size(122, 23);
            this.simpleButton5.TabIndex = 22;
            this.simpleButton5.Text = "Folder Encrypt";
            this.simpleButton5.Click += new System.EventHandler(this.EncryptFolder);
            // 
            // simpleButton4
            // 
            this.simpleButton4.Location = new System.Drawing.Point(9, 7);
            this.simpleButton4.Name = "simpleButton4";
            this.simpleButton4.Size = new System.Drawing.Size(122, 23);
            this.simpleButton4.TabIndex = 21;
            this.simpleButton4.Text = "Folder Decrypt";
            this.simpleButton4.Click += new System.EventHandler(this.FolderDecrypt);
            // 
            // textEdit2
            // 
            this.textEdit2.Location = new System.Drawing.Point(8, 37);
            this.textEdit2.Name = "textEdit2";
            this.textEdit2.Size = new System.Drawing.Size(100, 20);
            this.textEdit2.TabIndex = 25;
            // 
            // textEdit3
            // 
            this.textEdit3.Location = new System.Drawing.Point(8, 63);
            this.textEdit3.Name = "textEdit3";
            this.textEdit3.Size = new System.Drawing.Size(100, 20);
            this.textEdit3.TabIndex = 26;
            // 
            // simpleButton7
            // 
            this.simpleButton7.Location = new System.Drawing.Point(114, 34);
            this.simpleButton7.Name = "simpleButton7";
            this.simpleButton7.Size = new System.Drawing.Size(16, 23);
            this.simpleButton7.TabIndex = 27;
            this.simpleButton7.Text = "I";
            // 
            // simpleButton8
            // 
            this.simpleButton8.Location = new System.Drawing.Point(114, 60);
            this.simpleButton8.Name = "simpleButton8";
            this.simpleButton8.Size = new System.Drawing.Size(16, 23);
            this.simpleButton8.TabIndex = 28;
            this.simpleButton8.Text = "O";
            // 
            // simpleButton9
            // 
            this.simpleButton9.Location = new System.Drawing.Point(115, 59);
            this.simpleButton9.Name = "simpleButton9";
            this.simpleButton9.Size = new System.Drawing.Size(16, 23);
            this.simpleButton9.TabIndex = 32;
            this.simpleButton9.Text = "O";
            // 
            // simpleButton10
            // 
            this.simpleButton10.Location = new System.Drawing.Point(115, 33);
            this.simpleButton10.Name = "simpleButton10";
            this.simpleButton10.Size = new System.Drawing.Size(16, 23);
            this.simpleButton10.TabIndex = 31;
            this.simpleButton10.Text = "I";
            // 
            // textEdit4
            // 
            this.textEdit4.Location = new System.Drawing.Point(9, 62);
            this.textEdit4.Name = "textEdit4";
            this.textEdit4.Size = new System.Drawing.Size(100, 20);
            this.textEdit4.TabIndex = 30;
            // 
            // textEdit5
            // 
            this.textEdit5.Location = new System.Drawing.Point(9, 36);
            this.textEdit5.Name = "textEdit5";
            this.textEdit5.Size = new System.Drawing.Size(100, 20);
            this.textEdit5.TabIndex = 29;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 143);
            this.Controls.Add(this.xtraTabControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("MainForm.IconOptions.Image")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Happy Tool - By Serenity ver.70";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage3.ResumeLayout(false);
            this.xtraTabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Patchexe)).EndInit();
            this.xtraTabPage1.ResumeLayout(false);
            this.xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit5.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.GroupControl Patchexe;
        public DevExpress.XtraEditors.CheckButton TurnOffServer;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.SimpleButton simpleButton4;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraEditors.SimpleButton simpleButton6;
        private DevExpress.XtraEditors.SimpleButton simpleButton5;
        private DevExpress.XtraEditors.SimpleButton simpleButton8;
        private DevExpress.XtraEditors.SimpleButton simpleButton7;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevExpress.XtraEditors.SimpleButton simpleButton9;
        private DevExpress.XtraEditors.SimpleButton simpleButton10;
        private DevExpress.XtraEditors.TextEdit textEdit4;
        private DevExpress.XtraEditors.TextEdit textEdit5;
    }
}

