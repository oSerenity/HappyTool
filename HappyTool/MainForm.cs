using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HappyTool
{
    public partial class MainForm : XtraForm
    {

        #region PlaceHolders
        public string CurrentFullPath { get; private set; }
        public string CurrentFullName { get; private set; }
        public string ExeVersion { get; private set; }
        public static string Server { get; private set; }
        public static int TicketSum { get; private set; }
        private Thread _responseThread;
        public static HttpListenerContext context;
        static HttpListener _httpListener;



        #endregion

        #region Form Stuff
        public MainForm()
        {

            InitializeComponent();
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

        private void Patchexe_DragEnter(object sender, DragEventArgs e)
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
                //get's file version number
                ExeVersion = FileVersionInfo.GetVersionInfo(CurrentFile[0]).FileVersion;

                e.Effect = DragDropEffects.Copy;


            }
            else
            {
                // Don't allow any other drop.
                e.Effect = DragDropEffects.None;
            }
        }

        private void Patchexe_DragDrop(object sender, DragEventArgs e)
        {
            //Checks if User Has The Custom Patch Applied Into The EXE 
            DialogResult dialogResult = MessageBox.Show("Would You Like To Patch To Local Server", "Patch To Webserver", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Server = "http://localhost:12345";//23

                //Smarter Patching System To Prevent Dumbasses From Patching File Incorrectly
                VersionCheckAndPatch(Server, CurrentFullName, ExeVersion);

            }
            else if (dialogResult == DialogResult.No)
            {
                Server = "http://hwgamers.com/api";//20
                //Smarter Patching System To Prevent Dumbasses From Patching File Incorrectly
                VersionCheckAndPatch(Server, CurrentFullName, ExeVersion);
            }
        }





        #endregion

        #region Networking/Patching
        private void VersionCheckAndPatch(string server, string FileName, string Version)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(server);
            //make sure padding is correct
            Array.Resize(ref bytes, 52);
            if (Version.Equals("0.3.0.0"))
            {

                if (FileName.Equals("HappyWars.exe") && !FileName.Equals("LauncherDll.dll"))
                {
                    PatchServer(CurrentFullPath, 0x1002A50, bytes);
                    PatchServer(CurrentFullPath, 0x1002AC8, bytes);
                    PatchServer(CurrentFullPath, 0x1002B0C, bytes);
                    PatchServer(CurrentFullPath, 0x1002B58, bytes);
                }
                else if (Path.GetExtension(CurrentFullPath).Equals("LauncherDll.dll"))
                {
                    MessageBox.Show("Must Drag Exe Not The Launcher.dll And Files Must Be named Like The Original");
                }
            }
            else if (!Version.Equals("0.3.0.0"))//patch dll instead if it is not 0.3.0
            {
                if (Version.Equals("0.5.2.0"))//patch dll instead if it is not 0.3.0
                {
                    if (!FileName.Equals("HappyWars.exe") && FileName.Equals("LauncherDll.dll"))
                    {
                        PatchServer(CurrentFullPath, 0x9CC0, Encoding.ASCII.GetBytes(server));
                        PatchServer(CurrentFullPath, 0x9CF4, Encoding.ASCII.GetBytes(server));
                        PatchServer(CurrentFullPath, 0x9D2C, Encoding.ASCII.GetBytes(server));
                        PatchServer(CurrentFullPath, 0x9E38, Encoding.ASCII.GetBytes(server));
                    }
                    else if (FileName.Equals("HappyWars.exe"))
                    {
                        MessageBox.Show("Must Drag Dll Not The Happywars.exe And Files Must Be named Like The Original");
                    }
                }
                else
                {
                    MessageBox.Show("Version Is Not A Supported Version");
                }
            }
        }

        private void TurnOffSever_CheckedChanged(object sender, EventArgs e)
        {
            Application.Restart();
        }


        public static void PatchServer(string filepath, int position, byte[] data)
        {
            using (Stream stream = File.Open(filepath, FileMode.Open))
            {
                stream.Position = position;
                stream.Write(new byte[52], 0, new byte[52].Length);//cleans with zeros
                stream.Write(data, 0, data.Length);//writes new data
            }
        }
        private static void ResponseThread()
        {
            while (true)
            {
                context = _httpListener.GetContext();
                context.Response.KeepAlive = false;
                context.Response.Close();
                string DataReturned;
                DataReturned = string.Format("Client requested ( " + context.Request.RawUrl + " ) Status Code: " + context.Response.StatusCode + " " + Environment.NewLine,
context.Request.RawUrl/*, req, StartupDate.ToString("R")*/);

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
                    string data = "\"{\"RadarJsonResultBoughtTicketNum\":{\"result\": \"OK\",\"response\":{\"happyticket\":" + TicketSum + "}}}\"";
                    context.Response.StatusCode = 200; //Send OK Response
                    context.Response.ContentType = "text/html";


                    // Write out to the response stream (asynchronously), then close it
                    context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(data), 0, data.Length);
                    context.Response.Close();

                }
                Console.WriteLine("Response given to a request.");
            }
        }
        //starts a local server
        private void StartServer_Click(object sender, EventArgs e)
        {
            if (_httpListener == null)
            {
                labelControl2.Text = "Starting server...";
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add(string.Concat(new string[] { "http://", "localhost:12345", "/" }));
                _httpListener.Start();
                labelControl2.Text = "Server started.";
                _responseThread = new Thread(new ThreadStart(ResponseThread));
                _responseThread.Start();
            }
            else
            {
                _httpListener = null;
                _responseThread = null;

            }

        }

        private bool CheckIfPatchesAreMade(string FilePath)
        {
            //we convert from byte to hex then finally string in one go, we also make sure that the length is the same as user's input.
            string s1 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002A50, 81)))).Replace("0\\", string.Empty);
            string s2 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002AC8, 81)))).Replace("0\\", string.Empty);
            string s3 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002B0C, 81)))).Replace("0\\", string.Empty);
            string s4 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002B58, 81)))).Replace("0\\", string.Empty);
            if (s1.Contains(Server) && s2.Contains(Server) && s3.Contains(Server) && s4.Contains(Server))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static byte[] ReadBytes(string path, int offset, int count)
        {
            using (var file = File.OpenRead(path))
            {
                file.Position = offset;
                offset = 0;
                byte[] buffer = new byte[count];
                int read;
                while (count > 0 && (read = file.Read(buffer, offset, count)) > 0)
                {
                    offset += read;
                    count -= read;
                }
                if (count < 0) throw new EndOfStreamException();
                return buffer;
            }
        }
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        #endregion




        private void SetTickets_Click(object sender, EventArgs e)
        {
            if (!textEdit1.Text.Equals(string.Empty) && char.IsDigit(char.Parse(textEdit1.Text)))
            {

                TicketSum = int.Parse(textEdit1.Text);
            }
            else
            {
                TicketSum = 0;
            }

        }

        private void DecryptingFolder_Click(object sender, EventArgs e)
        {
            //Local instance of Folder Dialog 
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            //Shows if user wants to make a new folder.
            folderBrowserDialog.ShowNewFolderButton = true;
            //remmeber previous folder selection...
            //folderBrowserDialog.SelectedPath = @"C:\Users\Serenity\Documents\Happy Wars Achrive\0.5.2.0\Data\";

            //User clicked the correct project solution
            //Shows ui.
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {

                string Foldername = Path.GetFullPath(folderBrowserDialog.SelectedPath).Substring(folderBrowserDialog.SelectedPath.LastIndexOf(@"\") + 1);
                //Checks all files in that Directory
                foreach (string FIleSource in Directory.GetFiles(folderBrowserDialog.SelectedPath))
                {
                    byte[] Source = File.ReadAllBytes(FIleSource);
                    FileInfo fileInfo = new FileInfo(FIleSource);
                    string File_Extention = fileInfo.Extension;
                    string FileName = FIleSource.Substring(FIleSource.LastIndexOf(@"\")).Replace(File_Extention, "");

                    if (!Directory.Exists(Application.StartupPath + @"\" + Foldername + @"\"))
                    {
                        System.IO.Directory.CreateDirectory(Application.StartupPath + @"\" + Foldername + @"\");
                    }
                    byte[] decrypted = BFBR.Decrypt(Source, 0, Source.Length);

                    byte[] decompressed = ZRES.Decompress(decrypted, 0, decrypted.Length);
                    if (File_Extention == ".bres")
                    {
                        BRES bres = new BRES(decompressed, 0, decompressed.Length);

                        if (!Directory.Exists(Application.StartupPath + @"\" + Foldername + @"\" + bres.Name))
                        {
                            System.IO.Directory.CreateDirectory(Application.StartupPath + @"\" + Foldername + @"\" + bres.Name);
                        }
                        File.WriteAllBytes(Application.StartupPath + @"\" + Foldername + @"\" + FileName + File_Extention, decompressed);

                        foreach (var resource in bres.Resources)
                        {
                            File.WriteAllBytes(Application.StartupPath + @"\" + Foldername + @"\" + bres.Name + @"\" + resource.Name, resource.Content.GetArraySegment().ToArray());

                        }
                    }
                    else
                    {
                        File.WriteAllBytes(Application.StartupPath + @"\" + Foldername + @"\" + FileName + File_Extention, decompressed);

                    }


                }


                MessageBox.Show(Application.StartupPath + @"\" + Foldername + @"\" + "Done!");
            }

        }

        private void XboxDecompressFolder_Click(object sender, EventArgs e)
        {
            //Local instance of Folder Dialog 
            FolderBrowserDialog XboxDecompress = new FolderBrowserDialog();
            //Shows if user wants to make a new folder.
            XboxDecompress.ShowNewFolderButton = true;
            //remmeber previous folder selection...
            //XboxDecompress.SelectedPath = @"C:\Users\Serenity\Documents\Happy Wars Achrive\0.5.2.0\Data\";

            //Shows ui.
            if (XboxDecompress.ShowDialog() == DialogResult.OK)
            {

                string Foldername = Path.GetFullPath(XboxDecompress.SelectedPath).Substring(XboxDecompress.SelectedPath.LastIndexOf(@"\") + 1);
                //Checks all files in that Directory
                foreach (string FIleSource in Directory.GetFiles(XboxDecompress.SelectedPath))
                {
                    byte[] Source = File.ReadAllBytes(FIleSource);
                    FileInfo fileInfo = new FileInfo(FIleSource);
                    string File_Extention = fileInfo.Extension;
                    string FileName = FIleSource.Substring(FIleSource.LastIndexOf(@"\")).Replace(File_Extention, "");

                    if (!Directory.Exists(Application.StartupPath + @"\" + Foldername + @"\"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + @"\" + Foldername + @"\");
                    }
                    byte[] decompressed = ZRES.Decompress(Source, 0, Source.Length);
                    if (File_Extention == ".bres")
                    {
                        BRES bres = new BRES(decompressed, 0, decompressed.Length);

                        if (!Directory.Exists(Application.StartupPath + @"\" + Foldername + @"\" + bres.Name))
                        {
                            Directory.CreateDirectory(Application.StartupPath + @"\" + Foldername + @"\" + bres.Name);
                        }
                        File.WriteAllBytes(Application.StartupPath + @"\" + Foldername + @"\" + FileName + File_Extention, decompressed);

                        foreach (var resource in bres.Resources)
                        {
                            File.WriteAllBytes(Application.StartupPath + @"\" + Foldername + @"\" + bres.Name + @"\" + resource.Name, resource.Content.GetArraySegment().ToArray());

                        }
                    }
                    else
                    {
                        File.WriteAllBytes(Application.StartupPath + @"\" + Foldername + @"\" + FileName + File_Extention, decompressed);

                    }


                }


                MessageBox.Show(Application.StartupPath + @"\" + Foldername + @"\" + "Done!");
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {

        }

        private void FolderEncypt_Click(object sender, EventArgs e)
        {
            //Local instance of Folder Dialog 
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            //Shows if user wants to make a new folder.
            folderBrowserDialog.ShowNewFolderButton = true;
            //remmeber previous folder selection...
            folderBrowserDialog.SelectedPath = @"C:\Users\Serenity\Documents\Happy Wars Achrive\0.5.2.0\Data\";

            //User clicked the correct project solution
            //Shows ui.
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {

                string Foldername = Path.GetFullPath(folderBrowserDialog.SelectedPath).Substring(folderBrowserDialog.SelectedPath.LastIndexOf(@"\") + 1);
                byte[] Source = File.ReadAllBytes(@"C:\Users\Serenity\Documents\Happy Wars Achrive\0.5.2.0\Data\Test\cam_COOP_Massive.bres");
                string FileName = "cam_COOP_Massive";
                byte[] decrypted = BFBR.Decrypt(Source, 0, Source.Length);
                byte[] decompressed = ZRES.Decompress(decrypted, 0, decrypted.Length);
                BRES bres = new BRES(decompressed, 0, decompressed.Length);


                MessageBox.Show(Application.StartupPath + @"\" + Foldername + @"\" + "Done!");
            }
        }
    }
}
