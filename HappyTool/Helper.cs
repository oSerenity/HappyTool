
using Simias.Encryption;
//using Simias.Encryption;
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
        public static Blowfish bf = new Blowfish(Encoding.UTF8.GetBytes("04B915BA43FEB5B6"));
        public string Address { get; set; }
        #region Zlib Compress/Decompress
        public enum Options
        {
            Xbox,
            Steam,
            Zlib,
            UnZlib,
            Blowfish,
            CompressAndBlowfish,
            UndoBlowFishAndCompress
        }
        public static void Data(Options CompressionType, string Path)
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
                            stream.Write(Encoding.ASCII.GetBytes("ZRES"), 0, 4);
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
                                string japaneseTextFromFile = japaneseEncoding.GetString(File.ReadAllBytes(inFilePath));
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




        #region Blowfish Encryption/Decryption
        public static void BlowfishData(Options options, string path)
        {
            switch (options)
            {
                case Options.UndoBlowFishAndCompress://TODO:Work On Decryption
                    {//remove blowfish first then get rid of zlib

                        //File.WriteAllText(path, Decrypt(File.ReadAllBytes(path)));
                        break;
                    }
                case Options.CompressAndBlowfish:
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
                stream.Write(Encoding.ASCII.GetBytes("ZRES"), 0, 4);
                int num = Convert.ToInt32(new FileInfo(Path).Length);
                stream.WriteByte(Convert.ToByte(num & 0xff));
                stream.WriteByte(Convert.ToByte((num >> 8) & 0xff));
                stream.WriteByte(Convert.ToByte((num >> 0x10) & 0xff));
                stream.WriteByte(Convert.ToByte((num >> 0x18) & 0xff));
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
                stream = new FileStream(Path + ".temp", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                int length = (int)stream.Length;
                int size = 0x24 + length;
                byte[] buffer = new byte[xAlignSize(size)];
                stream.Read(buffer, 0x24, length);
                Array.Clear(buffer, size, buffer.Length - size);
                byte[] sourceArray = SHA256.Create().ComputeHash(buffer, 0x24, length);
                byte[] bytes = BitConverter.GetBytes(length);
                Array.Copy(bytes, buffer, bytes.Length);
                Array.Copy(sourceArray, 0, buffer, bytes.Length, sourceArray.Length);
                buffer = Encrypt(buffer);

                Encoding.GetEncoding("Shift_JIS");
                stream.Write(Encoding.ASCII.GetBytes("BFBR"), 0, 4);//writes the tag when file is closed
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
                if (File.Exists(Path))
                {

                    File.WriteAllBytes(Path, File.ReadAllBytes(Path + ".temp"));
                    File.Delete(Path + ".temp");
                    
                }
            }

        }
        private static byte[] CreateAlignBytes(byte[] source)
        {
            if (AlignSize(source.Length) != source.Length)
            {
                Array.Resize(ref source, source.Length);
                Array.Clear(source, source.Length, source.Length - source.Length);
            }
            return source;
        }
        private static int AlignSize(int size) => size + (8 - (size & 7));
        public static byte[] Encrypt(byte[] target)
        {
            byte[] i = CreateAlignBytes(target);
            bf.Encipher(i, i.Length);
            return new byte[i.Length];
        }
        public static int xAlignSize(int size)
        {
            int num = ((size & 7) != 0) ? 1 : 0;
            return (((size / 8) + num) * 8);
        }
        #endregion

    }
}
