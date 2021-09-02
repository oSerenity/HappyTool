using System.Net.Http;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Nancy.Json;
using zlib;
using DevExpress.XtraEditors;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using Simias.Encryption;

namespace HappyTool
{
    public partial class MainForm : XtraForm
    {

        #region PlaceHolders
        public static string CurrentFullPath { get; set; }

        public static string CurrentFullName { get; set; }
        public static byte[] Data { get; set; }
        public string Data2 { get; set; }
        public static string DataReturned { get; private set; }
        public string CurrentExeFullPath { get; private set; }
        public string CurrentExeFullName { get; private set; }

        public byte[] ExeData { get; private set; }
        public static bool ON = true;
        private Thread _responseThread;
        public static HttpListenerContext context;
        static HttpListener _httpListener;
        public static bool FirstResponseHasHit = false;



        #endregion

        #region Form Stuff
        public MainForm()
        {

            InitializeComponent();
            Decompress.AllowDrop = true;
            Compress.AllowDrop = true;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShutDownServer();
        }

        private void ShutDownServer()
        {
            try
            {
                labelControl2.Text = "Stopping server...";
                timer1.Stop();
                ON = false;
                if (_httpListener == null)
                {

                }
                else
                {
                    _httpListener.Close();
                    _httpListener = null;
                }
                if (_responseThread == null)
                {

                }
                else
                {
                    _responseThread.Abort();
                    _responseThread = null;
                }
                context = null;

                labelControl2.Text = "Null...";
                TurnOffServer.Checked = false;
            }
            catch
            {

            }
        }
        #endregion

        #region FileDrop Functions
        private void Dropbox_DragEnter(object sender, DragEventArgs e)
        {
            // See if this is a copy and the data includes text.
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && (e.AllowedEffect & DragDropEffects.Copy) != 0)
            {
                //File Being Dropped
                string[] CurrentFile = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                //Sets FullPath Path
                CurrentFullPath = Path.GetFullPath(CurrentFile[0]);
                //Sets FullName Path
                CurrentFullName = Path.GetFileName(CurrentFile[0]);
                e.Effect = DragDropEffects.Copy;



            }
            else
            {
                // Don't allow any other drop.
                e.Effect = DragDropEffects.None;
            }
        }

        private void Dropbox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Data2 = File.ReadAllText(CurrentFullPath);
                Data = File.ReadAllBytes(CurrentFullPath);

            }
            catch
            {
            }
            if (sender.Equals(Compress))
            {
                // get the file attributes for file or directory
                FileAttributes attr = File.GetAttributes(CurrentFullPath);
                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo d = null;
                    string[] subfolders = Directory.GetDirectories(CurrentFullPath);
                    foreach (string i in subfolders)
                    {
                        d = new DirectoryInfo(i);
                        FileInfo[] files = d.GetFiles("*.nut");
                        foreach (FileInfo file in files)
                        {
                            try
                            {
                                CurrentFullPath = Path.GetFullPath(file.FullName);
                                Data = File.ReadAllBytes(CurrentFullPath);
                            }
                            catch (Exception)
                            {

                            }
                            if (!HappyTool.Helper.IsAlreadyCompressed(CurrentFullName))
                            {
                                HappyTool.Helper.ZlibCompression(HappyTool.Helper.Options.Zlib, CurrentFullName);
                            }
                        }
                    }
                }
                else
                {
                    if (!HappyTool.Helper.IsAlreadyCompressed(CurrentFullPath))
                    {
                        HappyTool.Helper.ZlibCompression(HappyTool.Helper.Options.Zlib, CurrentFullPath);
                    }

                }
            }
            else if (sender.Equals(Decompress))
            {

                // get the file attributes for file or directory
                FileAttributes attr = File.GetAttributes(CurrentFullPath);
                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo d = null;
                    //check if we have sub category's otherwise 
                    try
                    {
                        d = new DirectoryInfo(CurrentFullPath);
                        FileInfo[] files = d.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            try
                            {
                                CurrentFullPath = Path.GetFullPath(file.FullName);
                                Data = File.ReadAllBytes(CurrentFullPath);
                            }
                            catch
                            {

                            }
                            if (HappyTool.Helper.IsAlreadyCompressed(file.FullName))
                            {
                                HappyTool.Helper.ZlibCompression(HappyTool.Helper.Options.Zlib, CurrentFullPath);
                            }
                        }
                    }
                    catch
                    {

                    }
                    string[] subfolders = Directory.GetDirectories(CurrentFullPath);
                    foreach (string i in subfolders)
                    {
                        d = new DirectoryInfo(i);
                        FileInfo[] files = d.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            try
                            {
                                CurrentFullPath = Path.GetFullPath(file.FullName);
                                Data = File.ReadAllBytes(CurrentFullPath);
                            }
                            catch (Exception)
                            {

                            }
                            if (HappyTool.Helper.IsAlreadyCompressed(file.FullName))
                            {
                                HappyTool.Helper.ZlibCompression(HappyTool.Helper.Options.UnZlib, CurrentFullPath);
                            }
                        }
                    }
                    MessageBox.Show("Decompressed All Files In Folder...");
                }
                else
                {
                    if (HappyTool.Helper.IsAlreadyCompressed(CurrentFullPath))
                    {
                        HappyTool.Helper.ZlibCompression(HappyTool.Helper.Options.UnZlib, CurrentFullPath);
                        MessageBox.Show("Decompressed File..." + " \"" + CurrentFullName + " \"");
                    }

                }

            }
            if (sender.Equals(BlowfishEncryption))
            {
                HappyTool.Helper.BlowfishData(HappyTool.Helper.Options.BlowFishAndCompress, CurrentFullPath);
                MessageBox.Show("Adding BlowFishing Encryption..." + " \"" + CurrentFullName + " \"");
            }
            if (sender.Equals(BlowfishDecryption))
            {
                HappyTool.Helper.BlowfishData(HappyTool.Helper.Options.UndoBlowFishAndCompress, CurrentFullPath);
                MessageBox.Show("Removing BlowFishing Encryption..." + " \"" + CurrentFullName + " \"");
            }
            if (sender.Equals(Patchexe))
            {

            }

        }
        private void Patchexe_DragEnter(object sender, DragEventArgs e)
        {
            // See if this is a copy and the data includes text.
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && (e.AllowedEffect & DragDropEffects.Copy) != 0)
            {
                //File Being Dropped
                string[] CurrentFile = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                //Sets FullPath Path
                CurrentExeFullPath = Path.GetFullPath(CurrentFile[0]);
                //Sets FullName Path
                CurrentExeFullName = Path.GetFileName(CurrentFile[0]);
                e.Effect = DragDropEffects.Copy;
                ExeData = File.ReadAllBytes(CurrentExeFullPath);


            }
            else
            {
                // Don't allow any other drop.
                e.Effect = DragDropEffects.None;
            }
        }

        private void Patchexe_DragDrop(object sender, DragEventArgs e)
        {
            //Checks if User Has The Custom Patch Appylied Into The EXE 
            PatchMethod();
        }

        #endregion

        #region Server/Patching
        /// <summary>
        /// Patches The Exe For Client
        /// </summary>
        private void PatchMethod()
        {
            Helper options = new Helper();
            options.Address = Address.Text;
            options.Port = Port.Text;
            if (!options.IsExePatched(CurrentExeFullPath))
            {
                if (Path.GetExtension(CurrentExeFullPath) == ".exe")
                {
                    ReplaceData(CurrentExeFullPath, 0x1002A50, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                    ReplaceData(CurrentExeFullPath, 0x1002AC8, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                    ReplaceData(CurrentExeFullPath, 0x1002B0C, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                    ReplaceData(CurrentExeFullPath, 0x1002B58, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                    MessageBox.Show("Patched!");
                }
                else if (Path.GetExtension(CurrentExeFullPath) == ".dll")
                {
                    if (!options.IsDllPatched(CurrentExeFullPath))
                    {
                        //ReplaceData(CurrentExeFullPath, 0x0019FEC, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                        //ReplaceData(CurrentExeFullPath, 0x001A058, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                        //ReplaceData(CurrentExeFullPath, 0x001A13C, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                        //ReplaceData(CurrentExeFullPath, 0x1002B58, Encoding.ASCII.GetBytes("http://" + Address.Text + ":" + Port.Text + "/"));
                    }
                    throw new Exception("Not Yet Added!");
                }
            }
            else
            {
                MessageBox.Show("Already Patched!");
            }
        }
        private void TurnOffSever_CheckedChanged(object sender, EventArgs e)
        {
            ShutDownServer();
        }
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().Substring(0, 8).ToUpper();
        }

        public byte[] addByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }
        public static void ReplaceData(string filename, int position, byte[] data)
        {
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                stream.Position = position;
                stream.Write(data, 0, data.Length);
            }
        }
        private static void ResponseThread()
        {
            while (ON)
            {
                context = _httpListener.GetContext();
                context.Response.KeepAlive = false;
                context.Response.Close();
                DataReturned = string.Format("Client requested ( " + context.Request.RawUrl + " ) Status Code: " + context.Response.StatusCode + " " + Environment.NewLine,
context.Request.RawUrl/*, req, StartupDate.ToString("R")*/);
                FirstResponseHasHit = true;

                if (context.Request.RawUrl == "/TelemetryEntity/?name=recordingrestapiresult")
                {
                    Console.WriteLine("ContentType: {0}", context.Request.ContentType);
                    string body = new StreamReader(context.Request.InputStream).ReadToEnd();
                    var data = body.Split(',');
                    foreach (var obj in data)
                    {
                        var ss = obj.Split(':');
                        Console.WriteLine("var {0} = {1}", ss[0], ss[1]);
                    }
                }
                if (context.Request.RawUrl == "/HappyTicketSum")
                {
                    Console.WriteLine("ContentType: {0}", context.Request.ContentType);
                    string body = new StreamReader(context.Request.InputStream).ReadToEnd();
                    var data = body.Split(',');
                    foreach (var obj in data)
                    {
                        var ss = obj.Split(':');
                        Console.WriteLine("var {0} = {1}", ss[0], ss[1]);
                    }
                }
                Console.WriteLine("Response given to a request.");
            }
        }
        private void StartServer_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + "\\HappyWars.exe"))
            {
                //Sets FullPath Path
                CurrentExeFullPath = Path.GetFullPath(Application.StartupPath +"\\HappyWars.exe");
                //Sets FullName Path
                CurrentExeFullName = Path.GetFileName(CurrentExeFullPath);
                ExeData = File.ReadAllBytes(CurrentExeFullPath);
                PatchMethod();
            }
            if(_httpListener == null)
            {
                this.labelControl2.Text = "Starting server...";
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add(string.Concat(new string[] { "http://", this.Address.Text, ":", this.Port.Text, "/" }));
                _httpListener.Start();
                this.labelControl2.Text = "Server started.";
                this._responseThread = new Thread(new ThreadStart(ResponseThread));
                this._responseThread.Start();
            }
            else
            {
                _httpListener = null;
                _responseThread = null;

            }

        }

        #endregion

        #region Timer
        private void timer1_Tick(object sender, EventArgs e)
        {

            //we make sure the string has a check to ensure the fact we don't want issues when starting server
            if (DataReturned == null)
            {
                //return if empty
                return;
            }
            else
            {
                if (FirstResponseHasHit == true)
                {
                    //set this false to let the program know we started the server and we need first response otherwise send nothing..
                    FirstResponseHasHit = false;
                    //send first update
                    //richTextBox1.AppendText(DataReturned);

                }
                //if string changes then it provides update to the log
                if (DataReturned != DataReturned)
                {
                    //richTextBox1.AppendText(Environment.NewLine + "--");
                    //send update
                    //richTextBox1.AppendText(DataReturned);
                }
                else
                {
                    //otherwise return nothing...
                    return;
                }
            }
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            timer1.Start();
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
