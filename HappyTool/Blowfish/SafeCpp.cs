using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Blowfish
{
    internal class SafeCpp : Cypher
    {
        private const string dllName = "blowfish_cpp.dll";

        [DllImport("blowfish_cpp.dll")]
        private static extern bool set_key(string key, uint keySize);

        [DllImport("blowfish_cpp.dll")]
        private static extern bool encipher(byte[] output, uint size, byte[] input);

        [DllImport("blowfish_cpp.dll")]
        private static extern bool decipher(byte[] output, uint size, byte[] input);

        public SafeCpp(string key)
        {
            if (!set_key(key, (uint)Encoding.ASCII.GetByteCount(key)))
                throw new Exception(string.Format("Key: {0} is invalid. Up to 4 to 56 bytes", (object)key));
        }

        public byte[] Encrypt(byte[] target)
        {
            byte[] alignBytes = this.CreateAlignBytes(target);
            uint length = (uint)alignBytes.Length;
            byte[] o = new byte[length];
            if (!encipher(o, length, alignBytes))
                throw new Exception("The key is not set");
            return o;
        }

        public string Encrypt(string target) => this.ByteToHex(this.Encrypt(this.CreateAlignBytes(target)));

        public byte[] Decrypt(byte[] target)
        {
            uint length = (uint)target.Length;
            byte[] o = new byte[length];
            if (!decipher(o, length, target))
                throw new Exception("The key is not set");
            return o;
        }

        public string Decrypt(string target) => Encoding.ASCII.GetString(this.Decrypt(this.HexToByte(target))).Replace("\0", string.Empty);

        private string ByteToHex(byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in bytes)
                stringBuilder.Append(num.ToString("x2"));
            return stringBuilder.ToString();
        }

        private byte[] HexToByte(string hex)
        {
            byte[] numArray = new byte[hex.Length / 2];
            for (int index = 0; index < hex.Length - 1; index += 2)
            {
                byte hex1 = this.GetHex(hex[index]);
                byte hex2 = this.GetHex(hex[index + 1]);
                numArray[index / 2] = (byte)((uint)hex1 * 16U + (uint)hex2);
            }
            return numArray;
        }

        private byte GetHex(char x)
        {
            if (x <= '9' && x >= '0')
                return (byte)((uint)x - 48U);
            if (x <= 'z' && x >= 'a')
                return (byte)((int)x - 97 + 10);
            return x <= 'Z' && x >= 'A' ? (byte)((int)x - 65 + 10) : (byte)0;
        }

        private int AlignSize(int size) => size + (8 - (size & 7));

        private byte[] CreateAlignBytes(byte[] source)
        {
            byte[] array = source;
            if (this.AlignSize(source.Length) != source.Length)
            {
                Array.Resize<byte>(ref array, source.Length);
                Array.Clear((Array)array, source.Length, array.Length - source.Length);
            }
            return array;
        }

        private byte[] CreateAlignBytes(string source)
        {
            byte[] bytes = new byte[this.AlignSize(source.Length)];
            Encoding.ASCII.GetBytes(source, 0, source.Length, bytes, 0);
            return bytes;
        }
    }
}
