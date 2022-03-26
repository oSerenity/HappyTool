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
        private void SteamFileDecrypt(object sender, EventArgs e)
        {
            SteamFiles(HappyFile.Decrypt);
        }

        private void SteamFileEncrypt(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult dialogResult =  MessageBox.Show("Are You Encypting A Bres File?", "Bres File?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SteamBresFiles(HappyFile.Encrypt);
            }
            else
            {
                SteamFiles(HappyFile.Encrypt);
            }
        }

        private void SteamBresFiles(HappyFile encrypt)
        {
            throw new NotImplementedException();
        }

        private static void SteamFiles(HappyFile Option)
        {
            if (Option == HappyFile.Decrypt)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string Path = openFileDialog.FileName;
                openFileDialog.Filter = "JSON|*.json|NUT|*.nut|JPG|*.jpg|BRES|*.bres";
                openFileDialog.Title = "File Decryption";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] Source = File.ReadAllBytes(openFileDialog.FileName);
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JSON|*.json|NUT|*.nut|JPG|*.jpg|BRES|*.bres";
                    saveFileDialog.Title = openFileDialog.Title = "Save Decryption File";
                    saveFileDialog.FileName = openFileDialog.FileName;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (new FileInfo(openFileDialog.FileName).Extension.Equals(".bres"))
                        {
                            SteamBresFiles(Source, saveFileDialog.FileName.Replace(".bres", string.Empty));
                        }
                        else
                        {
                            decrypted = BFBR.Decrypt(Source, 0, Source.Length);
                            File.WriteAllBytes(saveFileDialog.FileName, ZRES.Decompress(decrypted, 0, decrypted.Length));
                        }
                        MessageBox.Show("Decrypted File Saved!");
                    }
                }
            }
            else if (Option == HappyFile.Encrypt)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string Path = openFileDialog.FileName;
                openFileDialog.Filter = "JSON|*.json|NUT|*.nut|JPG|*.jpg|BRES|*.bres";
                openFileDialog.Title = "File Encryption";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] Source = File.ReadAllBytes(openFileDialog.FileName);
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JSON|*.json|NUT|*.nut|JPG|*.jpg|BRES|*.bres";
                    saveFileDialog.Title = openFileDialog.Title = "Save Encryption File";
                    saveFileDialog.FileName = openFileDialog.FileName;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (new FileInfo(openFileDialog.FileName).Extension.Equals(".bres"))
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
                        else
                        {
                            byte[] zres3 = ZRES.Compress(Source, 0, Source.Length);
                            File.WriteAllBytes(saveFileDialog.FileName, BFBR.Encrypt(zres3, 0, zres3.Length));
                        }
                        MessageBox.Show("Encrypted File Saved!");
                    }
                }
            }
        }

        private static void SteamBresFiles(byte[] Source, string FileName)
        {
            decrypted = BFBR.Decrypt(Source, 0, Source.Length);

            byte[] decompressed = ZRES.Decompress(decrypted, 0, decrypted.Length);

            BRES bres = new BRES(decompressed, 0, decompressed.Length);
            foreach (var resource in bres.Resources)
            {
                if (!Directory.Exists(FileName))
                {
                    Directory.CreateDirectory(FileName);
                }
                File.WriteAllBytes(FileName + @"\" + new Regex("[\\<\\>\\:\\\" \\/\\|\\? \\*]").Replace(resource.Name, "_"), resource.Content.GetArraySegment().ToArray());
            }
        }

        public enum HappyFile
        {
            Encrypt,
            Decrypt,
            Compress,
            Decompress
        }

        #region PlaceHolders
        private static byte[] decrypted { get; set; }
        public string CurrentFullPath { get; private set; }
        public string CurrentFullName { get; private set; }
        public string ExeVersion { get; private set; }
        public static string Server { get; private set; }
        public static int TicketSum { get; private set; }
        public string FileType { get; private set; }

        private static string currentfoldertype { get; set; }

        private static string FileLocation { get; set; }
        private static string EncryptedLocation { get; set; } = Application.StartupPath + @"\Steam Files\Encrypted\";
        private static string DecompressedLocation { get; set; } = Application.StartupPath + @"\Xbox Files\Decompressed\";
        private static string CompressedLocation { get; set; } = Application.StartupPath + @"\Xbox Files\Compressed\media";
        private Thread _responseThread;
        public static HttpListenerContext context;
        static HttpListener _httpListener;
        private int inc = 0;



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
            hex = hex.Replace("-", string.Empty);
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

        public void LoadDirectory(string Dir, HappyFile Option)
        {

            foreach (string FileOrgin in Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FileOrgin).Parent;
                string ProgramLocation = FileLocation + directoryInfo.Parent +@"\" +directoryInfo.Name;//FullName.Substring(directoryInfo.FullName.IndexOf(@"\Data"));
                if(ProgramLocation.Contains(@"\Decrypted\Data\"))
                {
                    if (!Directory.Exists(ProgramLocation))
                    {
                        Directory.CreateDirectory(ProgramLocation);
                    }
                }

                progressBarControl2.EditValue = inc;
                switch (Option)
                { 
                    case HappyFile.Decrypt:
                        DecryptAndUncompress(FileOrgin, ProgramLocation);

                        break;
                    case HappyFile.Encrypt:

                        CompressAndEncrypt(FileOrgin);
                        break;
                    case HappyFile.Compress:
                        Compress(FileOrgin);
                        break;
                    case HappyFile.Decompress:
                        Uncompress(FileOrgin);
                        break;
                }
                inc++;
            }
            progressBarControl2.EditValue = 100;
            MessageBox.Show("Task Has Been Finished...\n\rFor " + FileType);
        }

        private static void CreateParentFolder(HappyFile happyFile)
        {
            switch (happyFile)
            {
                case HappyFile.Encrypt:
                    if (!Directory.Exists(EncryptedLocation))
                    {
                        Directory.CreateDirectory(EncryptedLocation);
                    }
                    break;
                case HappyFile.Decrypt:
                    
                    if (!Directory.Exists(FileLocation))
                    {
                        Directory.CreateDirectory(FileLocation);
                    }
                    break;
                case HappyFile.Compress:
                    if (!Directory.Exists(CompressedLocation))
                    {
                        Directory.CreateDirectory(CompressedLocation);
                    }
                    break;
                case HappyFile.Decompress:
                    if (!Directory.Exists(DecompressedLocation))
                    {
                        Directory.CreateDirectory(DecompressedLocation);
                    }
                    break;
            }
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

                case ".zson":
                    File.WriteAllBytes(EncryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), ZRES.Compress(Source, 0, Source.Length));
                    break;
                case ".nut":
                    if (CurrentFolder == "nut" /*|| CurrentFolder == "json"*/)
                    {
                        byte[] zres = ZRES.Compress(Source, 0, Source.Length);
                        File.WriteAllBytes(EncryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), BFBR.Encrypt(zres, 0, zres.Length));
                    }
                    break;
                case ".json":
                    if (CurrentFolder == "json")
                    {
                        byte[] zres2 = ZRES.Compress(Source, 0, Source.Length);
                        File.WriteAllBytes(EncryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), BFBR.Encrypt(zres2, 0, zres2.Length));
                    }
                    else
                    {
                        break;
                    }
                    break;
                case ".jpg":
                    byte[] zres3 = ZRES.Compress(Source, 0, Source.Length);
                    File.WriteAllBytes(EncryptedLocation + Path.Substring(Path.LastIndexOf("\\Data\\")), BFBR.Encrypt(zres3, 0, zres3.Length));
                    break;
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
                            if (!Directory.Exists(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")).Replace(".bres", string.Empty) + @"\"))
                            {
                                Directory.CreateDirectory(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")).Replace(".bres", string.Empty) + @"\");
                            }
                            File.WriteAllBytes(DecompressedLocation + Path.Substring(Path.LastIndexOf("\\media\\")).Replace(".bres", string.Empty) + @"\" + new Regex("[\\<\\>\\:\\\" \\/\\|\\? \\*]").Replace(resource.Name, "_"), resource.Content.GetArraySegment().ToArray());

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
        /// <summary>
        /// REDO Directory Creation 
        /// </summary>
        /// <param name="OriginalPath"></param>
        /// <param name="NewPath"></param>
        private void DecryptAndUncompress(string OriginalPath , string NewPath)
        {
            byte[] Source = File.ReadAllBytes(OriginalPath);
            FileInfo fileInfo = new FileInfo(OriginalPath);
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
                            if (!Directory.Exists(FileLocation + OriginalPath.Substring(OriginalPath.LastIndexOf("\\Data\\")).Replace(".bres", string.Empty) + @"\"))
                            {
                                Directory.CreateDirectory(FileLocation + OriginalPath.Substring(OriginalPath.LastIndexOf("\\Data\\")).Replace(".bres", string.Empty) + @"\");
                            }
                            else
                            {
                                File.WriteAllBytes(BresFile(OriginalPath, resource), resource.Content.GetArraySegment().ToArray());
                            }
                            if (!Directory.Exists(FileLocation + "Decrypted_Data"))
                            {
                                Directory.CreateDirectory(FileLocation + "Decrypted_Data");
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
                    File.WriteAllBytes(FileLocation + OriginalPath.Substring(OriginalPath.LastIndexOf("\\Data\\")), ZRES.Decompress(Source, 0, Source.Length));
                    break;
                case ".nut":
                    decrypted = BFBR.Decrypt(Source, 0, Source.Length);
                    File.WriteAllBytes(FileLocation + OriginalPath.Substring(OriginalPath.LastIndexOf("\\Data\\")), ZRES.Decompress(decrypted, 0, decrypted.Length));
                    break;
                case ".json":
                    decrypted = BFBR.Decrypt(Source, 0, Source.Length);

                    File.WriteAllBytes(FileLocation + OriginalPath.Substring(OriginalPath.LastIndexOf("\\Data\\")), ZRES.Decompress(decrypted, 0, decrypted.Length));
                    break;
                case ".jpg":
                    decrypted = BFBR.Decrypt(Source, 0, Source.Length);
                    File.WriteAllBytes(FileLocation + OriginalPath.Substring(OriginalPath.LastIndexOf("\\Data\\")), ZRES.Decompress(decrypted, 0, decrypted.Length));
                    break;
            }
        }

        private static string BresFile(string Path, BRES.Resource resource)
        {
            return FileLocation + Path.Substring(Path.LastIndexOf("\\Data\\")).Replace(".bres", string.Empty) + @"\" + new Regex("[\\<\\>\\:\\\" \\/\\|\\? \\*]").Replace(resource.Name, "_");
        }

        private void xtraTabControl1_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            if (e.Page.Text == "Steam Files")
            {
                FileType = "Steam";
                currentfoldertype = "\\Data\\";
            }
            if (e.Page.Text == "Xbox Files")
            {
                FileType = "Xbox";
                currentfoldertype = "\\media\\";
            }
        }

        private void HappyFolderEncryptionOptions(object sender, EventArgs e)
        {
            progressBarControl2.EditValue = 0;
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {

                switch (FileType)
                {
                    case "Xbox":
                        if (folderBrowserDialog1.SelectedPath.EndsWith(@"\media"))
                        {
                            progressBarControl2.EditValue = 5;
                            CreateParentFolder(HappyFile.Decompress);
                            LoadDirectory(folderBrowserDialog1.SelectedPath, HappyFile.Decompress);
                        }
                        else
                        {
                            MessageBox.Show("Select media Directory!!");

                        }
                        break;
                    case "Steam":
                        if (folderBrowserDialog1.SelectedPath.EndsWith(@"\Data"))
                        {
                            progressBarControl2.EditValue = 5;
                            FileLocation = Application.StartupPath + @"\Steam Files\Decrypted\";
                            CreateParentFolder(HappyFile.Decrypt);
                            LoadDirectory(folderBrowserDialog1.SelectedPath, HappyFile.Decrypt);
                           
                        }
                        else
                        {
                            MessageBox.Show("Select Data Directory!!");

                        }
                        break;
                }
            }

        }


        private void HappyDeadManFile(object sender, EventArgs e)
        {
            switch (FileType)
            {
                case "Xbox":
                    if (Directory.Exists(CompressedLocation))
                    {
                        CreateParentFolder(HappyFile.Decompress);
                        LoadDirectory(CompressedLocation, HappyFile.Decompress);
                    }
                    else
                    {
                        MessageBox.Show(CompressedLocation + "\nwas Not Found!");
                    }
                    break;

                case "Steam":
                    if (Directory.Exists(FileLocation + "Data"))
                    {
                        CreateParentFolder(HappyFile.Decrypt);
                        LoadDirectory(FileLocation + "Data", HappyFile.Decrypt);

                        MessageBox.Show(EncryptedLocation + "Data" + "\nwas Made!");
                    }
                    else
                    {
                        MessageBox.Show(FileLocation + "Data" + "\nwas Not Found!");
                    }
                    break;
            }
        }
        #endregion

        private void simpleButton9_Click(object sender, EventArgs e)
        {

        }

        private void progressBarControl2_EditValueChanged(object sender, EventArgs e)
        {
            if (inc > 100)
            {
                inc = 0;
            }
            else
            {
                progressBarControl2.EditValue = inc;

            }
        }
    }
}
