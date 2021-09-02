using Blowfish;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Simias.Encryption;
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
        public static Simias.Encryption.Blowfish bf = new Simias.Encryption.Blowfish(Encoding.UTF8.GetBytes("04B915BA43FEB5B6"));
        public string Address { get; set; }
        [DllImport("blowfish_cpp.dll")]
        private static extern void get_tag(byte[] o);
        [DllImport("blowfish_cpp.dll")]
        private static extern uint get_tag_length();
        #region Zlib Compress/Decompress
        public enum Options
        {
            Xbox,
            Steam,
            Zlib,
            UnZlib,
            Blowfish,
            BlowFishAndCompress,
            UndoBlowFishAndCompress
        }
        public static void ZlibCompression(Options CompressionType, string Path)
        {
            //uses the same Path To Output
            string outFile = Path + "." + CompressionType.ToString();
            switch (CompressionType)
            {
                case Options.Zlib:
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
                case Options.UnZlib:
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
        private static void Swap(byte[] buff, int pos1, int pos2)
        {
            byte b = buff[pos1];
            buff[pos1] = buff[pos2];
            buff[pos2] = b;
        }

        private static void EndianSwap(byte[] buff)
        {
            for (int i = 0; i < buff.Length / 4; i++)
            {
                Swap(buff, i * 4, i * 4 + 3);
                Swap(buff, i * 4 + 1, i * 4 + 2);
            }
        }

        private static uint ReadU32(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            return BitConverter.ToUInt32(buff, 0);
        }

        private static byte[] Decompress(Stream s)
        {
            MemoryStream m = new MemoryStream();
            using (InflaterInputStream inf = new InflaterInputStream(s))
            {
                inf.CopyTo(m);
            }
            return m.ToArray();
        }

        public static string Decrypt(byte[] data)
        {
            MemoryStream m;
            uint check = ReadU32(m = new MemoryStream(data));
            if (check != 0x52424642)
            {
                return "Not a valid file!";
            }
            int size = data.Length - 4;
            if ((size % 8) != 0)
            {
                return "Not a valid file!";
            }
            byte[] buff = new byte[size];
            m.Read(buff, 0, size);
            EndianSwap(buff);
            bf.Decipher(buff, size);
            EndianSwap(buff);
            m = new MemoryStream(buff);
            int pSize = (int)ReadU32(m);
            m.Seek(0x2C, SeekOrigin.Begin);
            buff = new byte[pSize - 8];
            m.Read(buff, 0, pSize - 8);
            m = new MemoryStream(buff);
            buff = Decompress(m);
            return Encoding.GetEncoding(932).GetString(buff);
        }
        #region Blowfish Encryption/Decryption
        public static void BlowfishData(Options options, string path)
        {
            switch (options)
            {
                case Options.UndoBlowFishAndCompress:
                    {
                        File.WriteAllText(path, Decrypt(File.ReadAllBytes(path)));
                        break;
                    }
                case Options.BlowFishAndCompress:
                    {
                        //compresses it fisrt then it blowfish encryption
                        //ZlibCompression(Options.Zlib, path);
                        Blowfish(path, path);
                        break;
                    }
            }


        }

        public static void Blowfish(string Path, string output)
        {

            FileStream stream = new FileStream(Path + ".temp", FileMode.Create);
            ZOutputStream xoutput = new ZOutputStream(stream, -1);
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
                    CopyStream(input, xoutput);
                }
                catch (Exception)
                {
                    xoutput.Close();
                    stream.Close();
                    input.Close();
                    return;
                }
            }
            finally
            {
                xoutput.Close();
                stream.Close();
                input.Close();
                byte[] buffer;
                using (FileStream BlowfishStream = new FileStream(Path + ".temp", FileMode.Open, FileAccess.Read))
                {

                    int length = (int)BlowfishStream.Length;
                    int size = 0x24 + length;
                    buffer = new byte[Utility.AlignSize(size)];
                    BlowfishStream.Read(buffer, 0x24, length);
                    Array.Clear(buffer, size, buffer.Length - size);
                    byte[] sourceArray = SHA256.Create().ComputeHash(buffer, 0x24, length);
                    byte[] bytes = BitConverter.GetBytes(length);
                    Array.Copy(bytes, buffer, bytes.Length);
                    Array.Copy(sourceArray, 0, buffer, bytes.Length, sourceArray.Length);
                    buffer = Factory.Create(Factory.Type.Safe, "04B915BA43FEB5B6").Encrypt(buffer);
                    if (!File.Exists(output))
                    {
                        if (output == string.Empty)
                        {
                            output = Path + ".encrypt";
                        }
                        else
                        {
                            string fullPath = System.IO.Path.GetFullPath(output);
                            Directory.CreateDirectory(fullPath);
                            output = fullPath + System.IO.Path.GetFileName(Path) + ".encrypt";
                        }
                    }
                }
                using (FileStream stream2 = new FileStream(Path + ".temp", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    Encoding.GetEncoding("Shift_JIS");
                    byte[] o = new byte[get_tag_length()];//4 bytes
                    get_tag(o);//grabs 4 bytes 
                    stream2.Write(o, 0, o.Length);
                    stream2.Write(buffer, 0, buffer.Length);

                }
                if (File.Exists(Path))
                {

                    File.WriteAllBytes(Path, File.ReadAllBytes(Path + ".temp"));
                    File.Delete(Path + ".temp");
                }
            }

        }
        //public static void get_tag(byte[] Destination)
        //{
        //    byte[] xDestination;
        //    //byte[] unk_10003128 = null;
        //    //Array.Copy(Destination, 4, &xDestination.Length, 4 , 4);
        //}
        #endregion
        #region NetWorking
        public bool IsDllPatched(string path)
        {
            //we convert from byte to hex then finally string in one go, we also make sure that the length is the same as user's input.
            string s1 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002A50, Address.Length + 1))));
            string s2 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002AC8, Address.Length + 1))));
            string s3 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002B0C, Address.Length + 1))));
            string s4 = Encoding.ASCII.GetString(FromHex(BitConverter.ToString(ReadBytes(path, 0x1002B58, Address.Length + 1))));
            if (s1 == "http://" + Address  +"/" && s2 == "http://" + Address + "/" && s3 == "http://" + Address + "/" && s4 == "http://" + Address + "/")
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
            string local = "http://" + Address + "/";
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
