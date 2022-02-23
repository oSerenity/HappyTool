using DevExpress.XtraEditors;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HappyTool
{
    public partial class MainForm : XtraForm
    {

        #region PlaceHolders
        private static byte[] decrypted { get; set; }
        public string CurrentFullPath { get; private set; }
        public string CurrentFullName { get; private set; }
        public string ExeVersion { get; private set; }
        public static string Server { get; private set; }
        public static int TicketSum { get; private set; }
        public string FileType { get; private set; }
        public bool Decrypt { get; private set; }
        public bool Decompress { get; private set; }

        private static string DecryptedLocation = Application.StartupPath + @"\Steam Files\Decrypted\";
        private string EncryptedLocation = Application.StartupPath + @"\Steam Files\Encrypted\";
        private string DecompressedLocation = Application.StartupPath + @"\Xbox Files\Decompressed\";
        private string CompressedLocation = Application.StartupPath + @"\Xbox Files\Compressed\";
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
        #endregion

        #region Compression/Encryption

        public void LoadDirectory(string Dir)
        {
            foreach (string s in Directory.GetDirectories(Dir, "**", SearchOption.AllDirectories))
            {
                string folders = string.Empty;
                if (FileType == "Steam")
                {
                    switch (Decrypt)
                    {
                        case true:
                            folders = s.Substring(s.LastIndexOf("\\Data\\"));
                            if (!Directory.Exists(DecryptedLocation + folders))
                            {

                                Directory.CreateDirectory(DecryptedLocation + folders);
                            }
                            break;
                        case false:
                            folders = s.Substring(s.LastIndexOf("\\Data\\"));
                            if (!Directory.Exists(EncryptedLocation + folders))
                            {

                                Directory.CreateDirectory(EncryptedLocation + folders);
                            }
                            break;
                    }
                }
                else
                {
                    switch (Decompress)
                    {
                        case true:
                            folders = s.Substring(s.LastIndexOf("\\media\\"));
                            if (!Directory.Exists(DecompressedLocation + folders))
                            {

                                Directory.CreateDirectory(DecompressedLocation + folders);
                            }
                            break;
                        case false:
                            folders = s.Substring(s.LastIndexOf("\\media\\"));
                            if (!Directory.Exists(CompressedLocation + folders))
                            {

                                Directory.CreateDirectory(CompressedLocation + folders);
                            }
                            break;
                    }
                }
                
            }
            
            foreach (string result in Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories))
            {
            
                switch (Decrypt)
                {
                    case true:
                        DecryptAndUncompress(result);
                        break;
                    case false:

                        CompressAndEncrypt(result);
                        break;
                }
                if (FileType == "Xbox")
                {
                    switch (Decompress)
                    {
                        case true:
                            Uncompress(result);
                            break;
                        case false:
                            Compress(result);
                            break;
                    }
                }
            }
            MessageBox.Show("Done Writing Data...\n\rFor " + FileType);
        }

        private void Compress(string fullName)
        {
            BRES.Resource[] array = new BRES.Resource[]
            {
                new BRES.Resource()
                {
                Name = "CM:mission.json",
                Command = "campaign",
                Content = new BRES.DataSource( File.ReadAllBytes(@"C:\Users\Serenity\Documents\GitHub\HappyTool\HappyTool\bin\Debug\Steam Files\data\campaign\default\cam_COOP_Massive\CM_mission.json"))
                },
                new BRES.Resource()
                {
                Name = "CM:mission.nut",
                Command = "campaign",
                Content = new BRES.DataSource( File.ReadAllBytes(@"C:\Users\Serenity\Documents\GitHub\HappyTool\HappyTool\bin\Debug\Steam Files\data\campaign\default\cam_COOP_Massive\CM_mission00.nut"))
                }
            };
            BRES s = new BRES()
            {
                Name = "cam_COOP_Massive",
                Platform = BRES.GamePlatform.Xbox360,
                Resources = array
            };
            File.WriteAllBytes(@"C:\Users\Serenity\Documents\cam_COOP_Massive.bres", s.ToArray());
        }

        private void CompressAndEncrypt(string Path)
        {
            byte[] Source = File.ReadAllBytes(Path);
            string CurrentFolder = Path.Substring(Path.IndexOf("\\Data\\") + "\\Data\\".Length).Substring(0, Path.Substring(Path.IndexOf("\\Data\\") + "\\Data\\".Length).IndexOf("\\"));
            FileInfo fileInfo = new FileInfo(Path);
            switch (fileInfo.Extension)
            {

                //case ".zson":
                //    File.WriteAllBytes(EncryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), ZRES.Compress(Source, 0, Source.Length));
                //    break;
                case ".nut":
                    if(CurrentFolder == "nut" /*|| CurrentFolder == "json"*/)
                    {
                        byte[] zres = ZRES.Compress(Source, 0, Source.Length);
                        File.WriteAllBytes(EncryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), BFBR.Encrypt(zres, 0, zres.Length));
                    }
                    break;
                //case ".json":
                //    byte[] zres2 = ZRES.Compress(Source, 0, Source.Length);
                //    File.WriteAllBytes(EncryptedLocation  + Path.Substring(Path.LastIndexOf("\\Data\\")), BFBR.Encrypt(zres2, 0, zres2.Length));
                //    break;
                //case ".jpg":
                //    byte[] zres3 = ZRES.Compress(Source, 0, Source.Length);
                //    File.WriteAllBytes(EncryptedLocation  + Path.Substring(Path.LastIndexOf("\\Data\\")), BFBR.Encrypt(zres3, 0, zres3.Length));
                //    break;
            }
        }

        private static void BresFile()
        {
            BRES.Resource[] array = new BRES.Resource[]
{
                new BRES.Resource()
                {
                Name = "CM:mission.json",
                Command = "campaign",
                Content = new BRES.DataSource( File.ReadAllBytes(@"C:\Users\Serenity\Documents\GitHub\HappyTool\HappyTool\bin\Debug\Steam Files\data\campaign\default\cam_COOP_Massive\CM_mission.json"))
                },
                new BRES.Resource()
                {
                Name = "CM:mission.nut",
                Command = "campaign",
                Content = new BRES.DataSource( File.ReadAllBytes(@"C:\Users\Serenity\Documents\GitHub\HappyTool\HappyTool\bin\Debug\Steam Files\data\campaign\default\cam_COOP_Massive\CM_mission00.nut"))
                }
};
            BRES s = new BRES()
            {
                Name = "cam_COOP_Massive",
                Platform = BRES.GamePlatform.Steam,
                Resources = array
            };
            File.WriteAllBytes(@"C:\Users\Serenity\Documents\cam_COOP_Massive.bres", s.ToArray());
        }

        private void Uncompress(string Path)
        {
            byte[] Source = File.ReadAllBytes(Path);
                switch (new FileInfo(Path).Extension)
                {
                    case ".bres":


                        System.Threading.Tasks.Task.Run(() =>
                        {
                            byte[] decompressed = ZRES.Decompress(Source, 0, Source.Length);
                            BRES bres = new BRES(decompressed, 0, decompressed.Length);
                            foreach (var resource in bres.Resources)
                            {
                                if (!Directory.Exists(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")).Replace(".bres", "") + @"\"))
                                {
                                    Directory.CreateDirectory(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")).Replace(".bres", "") + @"\");
                                }
                                File.WriteAllBytes(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")).Replace(".bres", "") + @"\" + new Regex("[\\<\\>\\:\\\" \\/\\|\\? \\*]").Replace(resource.Name, "_"), resource.Content.GetArraySegment().ToArray());

                            }
                        });
                        break;
                    case ".zson":
                        byte[] decompressed2 = ZRES.Decompress(Source, 0, Source.Length);
                        File.WriteAllBytes(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")), decompressed2);
                        break;
                    case ".nut":
                        byte[] decompressed3 = ZRES.Decompress(Source, 0, Source.Length);
                        File.WriteAllBytes(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")), decompressed3);
                        break;
                    case ".json":
                        byte[] decompressed4 = ZRES.Decompress(Source, 0, Source.Length);
                        File.WriteAllBytes(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")), decompressed4);
                        break;
                    case ".jpg":
                        byte[] decompressed5 = ZRES.Decompress(Source, 0, Source.Length);
                        File.WriteAllBytes(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")), decompressed5);
                        break;
                }
        }

        private void DecryptAndUncompress(string Path)
        {
            byte[] Source = File.ReadAllBytes(Path);
            FileInfo fileInfo  = new FileInfo(Path);
            switch (fileInfo.Extension)
                {
                    case ".bres":
                        decrypted = BFBR.Decrypt(Source, 0, Source.Length);

                        byte[] decompressed = ZRES.Decompress(decrypted, 0, decrypted.Length);

                    System.Threading.Tasks.Task.Run(() =>
                        {
                            BRES bres = new BRES(decompressed, 0, decompressed.Length);
                            foreach (var resource in bres.Resources)
                            {
                                if (!Directory.Exists(DecryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")).Replace(".bres", "") + @"\"))
                                {
                                    Directory.CreateDirectory(DecryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")).Replace(".bres", "") + @"\");
                                }
                                else
                                {
                                    File.WriteAllBytes(BresFile(Path, resource), resource.Content.GetArraySegment().ToArray());
                                }
                                if (!Directory.Exists(DecryptedLocation + "Decrypted_Data"))
                                {
                                    Directory.CreateDirectory(DecryptedLocation + "Decrypted_Data");
                                }
                                else
                                {
                                    //string FileName = Application.StartupPath + @"\" + @"Steam Files\Decrypted_Data\List.Data";


                                    //// Consider File Operation 1
                                    //    FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate);
                                    //    StreamWriter str = new StreamWriter(fs);
                                    //    str.BaseStream.Seek(0, SeekOrigin.End);
                                    //    str.Write(Path.Substring(Path.LastIndexOf(@"\Data\")) + "\n" + bres.Name + "\n" + resource.Command + "\n" + resource.Name + "\n" + "========================================================" + Environment.NewLine);
                                    //    //str.WriteLine(DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString());
                                    //    str.Flush();
                                    //    str.Close();
                                    //    fs.Close();
                                    //    // Close the Stream then Individually you can access the file.
                                    

                                    ////File.AppendAllText(FileName, Path.Substring(Path.LastIndexOf(@"\Data\")) + "\n" + bres.Name + "\n" + resource.Command + "\n" + resource.Name + "\n" + "========================================================" + Environment.NewLine);
                                   
                                }
                            }
                        });
                        break;
                    case ".zson":
                        File.WriteAllBytes(DecryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), ZRES.Decompress(Source, 0, Source.Length));
                        break;
                    case ".nut":
                        decrypted = BFBR.Decrypt(Source, 0, Source.Length);
                        File.WriteAllBytes(DecryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), ZRES.Decompress(decrypted, 0, decrypted.Length));
                        break;
                    case ".json":
                        decrypted = BFBR.Decrypt(Source, 0, Source.Length);
                        File.WriteAllBytes(DecryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), ZRES.Decompress(decrypted, 0, decrypted.Length));
                        break;
                    case ".jpg":
                        decrypted = BFBR.Decrypt(Source, 0, Source.Length);
                        File.WriteAllBytes(DecryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), ZRES.Decompress(decrypted, 0, decrypted.Length));
                        break;
                }
        }

        private static string BresFile(string Path, BRES.Resource resource)
        {
            return DecryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")).Replace(".bres", "") + @"\" + new Regex("[\\<\\>\\:\\\" \\/\\|\\? \\*]").Replace(resource.Name, "_");
        }

        private void xtraTabControl1_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            if (e.Page.Text == "Steam Files")
            {
                FileType = "Steam";
            }
            if (e.Page.Text == "Xbox Files")
            {
                FileType = "Xbox";
            }
        }

        private void FolderOptions(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            switch(FileType)
            {
                case "Xbox":
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if (folderBrowserDialog1.SelectedPath.EndsWith(@"\media"))
                        {
                            Decompress = true;
                            LoadDirectory(folderBrowserDialog1.SelectedPath);
                        }
                        else
                        {
                            MessageBox.Show("Select media Directory!!");

                        }
                    }
                    break;
                case "Steam":
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if (folderBrowserDialog1.SelectedPath.EndsWith(@"\Data"))
                        {
                            Decrypt = true;
                            LoadDirectory(folderBrowserDialog1.SelectedPath);
                        }
                        else
                        {
                            MessageBox.Show("Select Data Directory!!");

                        }
                    }
                    break;
            }

        }


        private void FolderOptions2(object sender, EventArgs e)
        {
            switch (FileType)
            {
                case "Xbox":
                    if (Directory.Exists(CompressedLocation + "media"))
                    {
                        Decompress = false;
                        LoadDirectory(CompressedLocation + "media");
                    }
                    else
                    {
                        MessageBox.Show(CompressedLocation + "media" + "\nwas Not Found!");
                    }
                    break;

                case "Steam":
                    if(Directory.Exists(DecryptedLocation + "Data"))
                    {
                        Decrypt = false;
                        LoadDirectory(DecryptedLocation + "Data");

                        MessageBox.Show(EncryptedLocation + "Data" + "\nwas Made!");
                    }
                    else
                    {
                        MessageBox.Show(DecryptedLocation + "Data" + "\nwas Not Found!");
                    }
                    break;
            }
        }
        #endregion
    }
}
