using Blowfish;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using zlib;

namespace HappyTool
{
    public class Helper
    {
        public string Address { get; set; }
        public string Port { get; set; }

        [DllImport("blowfish_cpp.dll")]
        private static extern uint get_tag_length();

        [DllImport("blowfish_cpp.dll")]
        private static extern void get_tag(byte[] o);
        #region Zlib Compress/Decompress
        public enum ZlibOptions
        {
            Compress,
            Decompress
        }
        public enum BlowfishOptions
        {
            Encrypt,
            Decrypt,
            EncryptBlowfishAndCompress,
            DecryptBlowfishAndCompress
        }
        public void ZlibCompression(ZlibOptions CompressionType, string Path)
        {
            //uses the same Path To Output
            string outFile = Path + "." + CompressionType.ToString();
            switch (CompressionType)
            {
                case ZlibOptions.Compress:
                    {
                        FileStream stream = new FileStream(outFile, FileMode.Create);
                        ZOutputStream output = new ZOutputStream(stream, -1);
                        FileStream input = new FileStream(Path, FileMode.Open);
                        try
                        {
                            stream.WriteByte(Convert.ToByte('Z'));
                            stream.WriteByte(Convert.ToByte('R'));
                            stream.WriteByte(Convert.ToByte('E'));
                            stream.WriteByte(Convert.ToByte('S'));
                            int num = Convert.ToInt32(new FileInfo(Path).Length);
                            stream.WriteByte(Convert.ToByte((int)(num & 0xff)));
                            stream.WriteByte(Convert.ToByte((int)((num >> 8) & 0xff)));
                            stream.WriteByte(Convert.ToByte((int)((num >> 0x10) & 0xff)));
                            stream.WriteByte(Convert.ToByte((int)((num >> 0x18) & 0xff)));
                            try
                            {
                                CopyStream(input, output);
                            }
                            catch (Exception)
                            {
                                output.Close();
                                stream.Close();
                                input.Close();
                                return;
                            }
                        }
                        finally
                        {
                            output.Close();
                            stream.Close();
                            input.Close();
                        }
                        break;
                    }
                case ZlibOptions.Decompress:
                    {
                        if (!Path.Contains(".temp"))
                        {
                            //We Localize All Ways To Get The File For Smoother Transition.
                            string inFilePath = System.IO.Path.GetFullPath(Path);
                            byte[] inFileData = File.ReadAllBytes(inFilePath);
                            //we remove 8 bytes from the beginning to get rid of the header
                            byte[] newArray = new byte[inFileData.Length - 8];
                            Array.Copy(inFileData, 8, newArray, 0, newArray.Length);
                            //We Make A Temp File For The Program To Use..
                            File.WriteAllBytes(MainForm.CurrentFullName + ".temp", newArray);
                            //We Proceed to Use That File
                            System.IO.FileStream outFileStream = new System.IO.FileStream(outFile, System.IO.FileMode.Create);
                            zlib.ZOutputStream outZStream = new zlib.ZOutputStream(outFileStream);
                            System.IO.FileStream inFileStream = new System.IO.FileStream(MainForm.CurrentFullName + ".temp", System.IO.FileMode.Open);
                            try
                            {

                                CopyStream(inFileStream, outZStream);
                            }
                            finally
                            {
                                outZStream.Close();
                                outFileStream.Close();
                                inFileStream.Close();

                                //we Shift The Byte's For Language.
                                var japaneseEncoding = Encoding.GetEncoding(932);
                                // Reed From file bytes
                                string japaneseTextFromFile = japaneseEncoding.GetString(File.ReadAllBytes(inFilePath + ".Decompress"));
                                //We Write New FIle With Shifted Bytes
                                File.WriteAllText(inFilePath, japaneseTextFromFile);
                                //We Clean Up
                                if (File.Exists(inFilePath + ".Decompress"))
                                {
                                    File.Delete(inFilePath + ".Decompress");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid Entry");
                        }
                        break;
                    }
                default: break;
            }
        }
        public static void CopyStream(Stream input, Stream output)
        {
            int num;
            byte[] buffer = new byte[2000];
            while ((num = input.Read(buffer, 0, 2000)) > 0)
            {
                try
                {
                    output.Write(buffer, 0, num);
                }
                catch
                {
                    Console.WriteLine("Error " + num);
                }
            }
            output.Flush();
        }

        public static bool IsAlreadyCompressed(string srcFile)
        {
            bool flag = false;
            FileStream stream = new FileStream(srcFile, FileMode.Open);
            try
            {
                flag = ((stream.ReadByte() == Convert.ToByte('Z')) && ((stream.ReadByte() == Convert.ToByte('R')) && (stream.ReadByte() == Convert.ToByte('E')))) && (stream.ReadByte() == Convert.ToByte('S'));
            }
            finally
            {
                stream.Close();
            }
            return flag;
        }
        private static bool IsFileNonUpdate(string distFile, string srcFile) => 0 < new FileInfo(distFile).LastWriteTime.CompareTo(new FileInfo(srcFile).LastWriteTime);
        #endregion

        #region Blowfish Encryption/Decryption
        public static void BlowfishData(BlowfishOptions options, string path)
        {
            FileStream fileStream;
            switch (options)
            {
                case BlowfishOptions.Encrypt:
                    {
                        using (fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            int length = (int)fileStream.Length;
                            int num = 36 + length;
                            MainForm.Data = new byte[Utility.AlignSize(num)];
                            fileStream.Read(MainForm.Data, 36, length);
                            Array.Clear(MainForm.Data, num, MainForm.Data.Length - num);

                            //we need use the original hash Form Previous file
                            byte[] hash = SHA256.Create().ComputeHash(MainForm.Data, 36, length);
                            byte[] bytes = BitConverter.GetBytes(length);
                            Array.Copy((Array)bytes, (Array)MainForm.Data, bytes.Length);
                            Array.Copy((Array)hash, 0, (Array)MainForm.Data, bytes.Length, hash.Length);
                            MainForm.Data = Factory.Create(Factory.Type.Safe, "04B915BA43FEB5B6").Encrypt(MainForm.Data);
                            using (fileStream = new FileStream(MainForm.CurrentFullPath + ".encrypt", FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                Encoding.GetEncoding("Shift_JIS");
                                byte[] numArray2 = new byte[get_tag_length()];
                                get_tag(numArray2);
                                fileStream.Write(numArray2, 0, numArray2.Length);
                                fileStream.Write(MainForm.Data, 0, MainForm.Data.Length);
                            }
                        }
                        break;
                    }
                case BlowfishOptions.Decrypt:
                    {
                        using (fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            int length = (int)fileStream.Length;
                            int num = 36 + length;
                            MainForm.Data = new byte[Utility.AlignSize(num)];
                            fileStream.Read(MainForm.Data, 36, length);
                            Array.Clear(MainForm.Data, num, MainForm.Data.Length - num);

                            //we need use the original hash Form Previous file
                            byte[] hash = SHA256.Create().ComputeHash(MainForm.Data, 36, length);
                            byte[] bytes = BitConverter.GetBytes(length);
                            Array.Copy((Array)bytes, (Array)MainForm.Data, bytes.Length);
                            Array.Copy((Array)hash, 0, (Array)MainForm.Data, bytes.Length, hash.Length);
                            MainForm.Data = Factory.Create(Factory.Type.Safe, "04B915BA43FEB5B6").Decrypt(MainForm.Data);
                            using (fileStream = new FileStream(MainForm.CurrentFullPath + ".encrypt", FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                Encoding.GetEncoding("Shift_JIS");
                                byte[] numArray2 = new byte[get_tag_length()];
                                get_tag(numArray2);
                                //fileStream.Write(numArray2, 0, numArray2.Length);
                                fileStream.Write(MainForm.Data, 0, MainForm.Data.Length);
                            }
                        }
                        break;
                    }
                case BlowfishOptions.EncryptBlowfishAndCompress:
                    {
                        break;
                    }
                case BlowfishOptions.DecryptBlowfishAndCompress:
                    {

                        break;
                    }
            }


        }
        #endregion
        #region NetWorking
        public bool IsDllPatched(string path)
        {
            //we convert from byte to hex then finally string in one go, we also make sure that the length is the same as user's input.
            string s1 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002A50, Address.Length + Port.Length + 1))));
            string s2 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002AC8, Address.Length + Port.Length + 1))));
            string s3 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002B0C, Address.Length + Port.Length + 1))));
            string s4 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002B58, Address.Length + Port.Length + 1))));
            if (s1 == "http://" + Address + ":" + Port + "/" && s2 == "http://" + Address + ":" + Port + "/" && s3 == "http://" + Address + ":" + Port + "/" && s4 == "http://" + Address + ":" + Port + "/")
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
        /// <summary>
        /// Reads if File Has Been Patched To The User's Input
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public bool IsExePatched(string path)
        {
            //we convert from byte to hex then finally string in one go, we also make sure that the length is the same as user's input.
            string s1 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002A50, 81)))).Replace("0\\", string.Empty);
            string s2 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002AC8, 81)))).Replace("0\\", string.Empty);
            string s3 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002B0C, 81)))).Replace("0\\", string.Empty);
            string s4 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002B58, 81)))).Replace("0\\", string.Empty);
            string local = "http://" + Address + ":" + Port + "/";
            if (s1.Contains(local) && s2.Contains(local) && s3.Contains(local) && s4.Contains(local))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //    // string idk = "http://www.idk17.xyz";
        //    byte[] bytes1 = Encoding.ASCII.GetBytes("https://gameapi.happywarspc.msgamestudios.com/api");
        //    byte[] bytes2 = Encoding.ASCII.GetBytes("https://sts.happywarspc.msgamestudios.com/api/RefreshXstsToken?targetType=Product");
        //    byte[] bytes3 = Encoding.ASCII.GetBytes("https://telemetry.happywarspc.msgamestudios.com/api");
        //    byte[] bytes4 = Encoding.ASCII.GetBytes("https://purchasing.happywarspc.msgamestudios.com/api");
        #endregion
    }
}
