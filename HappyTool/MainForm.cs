using DevExpress.XtraEditors;
using System;
using System.Diagnostics;
using System.IO;
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
        public static int TicketSum { get; private set; }
        public static int patchAddress1 { get; private set; }
        public static int patchAddress2 { get; private set; }
        public static int patchAddress3 { get; private set; }
        public static int patchAddress4 { get; private set; }
        public static bool ON { get; private set; } = true;
        private Thread _responseThread;
        public static HttpListenerContext context;
        static HttpListener _httpListener;
        public static bool FirstResponseHasHit = false;



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
                //Smarter Patching System To Prevent Dumbasses From Patching File Incorrectly
                VersionCheckAndPatch("http://localhost:12345/");

            }
            else if (dialogResult == DialogResult.No)
            {//server
                //Smarter Patching System To Prevent Dumbasses From Patching File Incorrectly
                VersionCheckAndPatch("http://hwgamers.com/");
            }
        }





        #endregion

        #region Networking/Patching
        private void VersionCheckAndPatch(string server)
        {
            if (FileVersionInfo.GetVersionInfo(CurrentFullPath).FileVersion.Equals("0.3.0.0"))
            {

                if (Path.GetFileName(CurrentFullPath).Equals("HappyWars.exe") && !Path.GetExtension(CurrentFullPath).Equals("LauncherDll.dll"))
                {
                    patchAddress1 = 0x1002A50;
                    patchAddress2 = 0x1002AC8;
                    patchAddress3 = 0x1002B0C;
                    patchAddress4 = 0x1002B58;
                    PatchServer(CurrentFullPath, 0x1002A50, Encoding.ASCII.GetBytes(server));
                    PatchServer(CurrentFullPath, 0x1002AC8, Encoding.ASCII.GetBytes(server));
                    PatchServer(CurrentFullPath, 0x1002B0C, Encoding.ASCII.GetBytes(server));
                    PatchServer(CurrentFullPath, 0x1002B58, Encoding.ASCII.GetBytes(server));
                }
                else if (Path.GetExtension(CurrentFullPath).Equals("LauncherDll.dll"))
                {
                    MessageBox.Show("Must Drag Exe Not The Launcher.dll And Files Must Be named Like The Original");
                }
            }
            else if (!FileVersionInfo.GetVersionInfo(CurrentFullPath).FileVersion.Equals("0.3.0.0"))//patch dll instead if it is not 0.3.0
            {
                if (FileVersionInfo.GetVersionInfo(CurrentFullPath).FileVersion.Equals("0.5.2.0"))//patch dll instead if it is not 0.3.0
                {
                    if (!Path.GetFileName(CurrentFullPath).Equals("HappyWars.exe") && Path.GetExtension(CurrentFullPath).Equals("LauncherDll.dll"))
                    {
                        patchAddress1 = 0x9CC0;
                        patchAddress2 = 0x9CF4;
                        patchAddress3 = 0x9D2C;
                        patchAddress4 = 0x9E38;
                        PatchServer(CurrentFullPath, 0x9CC0, Encoding.ASCII.GetBytes(server));
                        PatchServer(CurrentFullPath, 0x9CF4, Encoding.ASCII.GetBytes(server));
                        PatchServer(CurrentFullPath, 0x9D2C, Encoding.ASCII.GetBytes(server));
                        PatchServer(CurrentFullPath, 0x9E38, Encoding.ASCII.GetBytes(server));
                    }
                    else if (Path.GetFileName(CurrentFullPath).Equals("HappyWars.exe"))
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
        public static void PatchServer(string filepath, int position, byte[] data)
        {
            using (Stream stream = File.Open(filepath, FileMode.Open))
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
                string DataReturned;
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
            try
            {
                if (CheckIfPatchesAreMade(CurrentFullPath).Equals(true))
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
                else
                {
                    MessageBox.Show("Must Patch File.");
                }
            }
            finally
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

        }

        private bool CheckIfPatchesAreMade(string FilePath)
        {
            //we convert from byte to hex then finally string in one go, we also make sure that the length is the same as user's input.
            string s1 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002A50, 81)))).Replace("0\\", string.Empty);
            string s2 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002AC8, 81)))).Replace("0\\", string.Empty);
            string s3 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002B0C, 81)))).Replace("0\\", string.Empty);
            string s4 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(FilePath, 0x1002B58, 81)))).Replace("0\\", string.Empty);
            string local = "http://" + "localhost:1234" + "/";
            if (s1.Contains(local) && s2.Contains(local) && s3.Contains(local) && s4.Contains(local))
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


        private void MainForm_Load(object sender, EventArgs e)
        {
        }


        private void simpleButton2_Click(object sender, EventArgs e)
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
    }
}
