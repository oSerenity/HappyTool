namespace Blowfish
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class SafeCpp : Cypher
    {
        private const string dllName = "blowfish_cpp.dll";

        public SafeCpp(string key)
        {
            if (!set_key(key, (uint) Encoding.ASCII.GetByteCount(key)))
            {
                throw new Exception($"キー:{key}は不正です。4～56バイトまでです");
            }
        }

        private int AlignSize(int size) => 
            size + (8 - (size & 7));

        private string ByteToHex(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte num2 in bytes)
            {
                builder.Append(num2.ToString("x2"));
            }
            return builder.ToString();
        }

        private byte[] CreateAlignBytes(byte[] source)
        {
            byte[] array = source;
            if (this.AlignSize(source.Length) != source.Length)
            {
                Array.Resize<byte>(ref array, source.Length);
                Array.Clear(array, source.Length, array.Length - source.Length);
            }
            return array;
        }

        private byte[] CreateAlignBytes(string source)
        {
            byte[] bytes = new byte[this.AlignSize(source.Length)];
            Encoding.ASCII.GetBytes(source, 0, source.Length, bytes, 0);
            return bytes;
        }

        [DllImport("blowfish_cpp.dll")]
        private static extern bool decipher(byte[] o, uint size, byte[] i);
        public byte[] Decrypt(byte[] target)
        {
            uint length = (uint) target.Length;
            byte[] o = new byte[length];
            if (!decipher(o, length, target))
            {
                throw new Exception("キーが設定されていません");
            }
            return o;
        }

        public string Decrypt(string target) => 
            Encoding.ASCII.GetString(this.Decrypt(this.HexToByte(target))).Replace("\0", "");

        [DllImport("blowfish_cpp.dll")]
        private static extern bool encipher(byte[] o, uint size, byte[] i);
        public byte[] Encrypt(byte[] target)
        {
            byte[] i = this.CreateAlignBytes(target);
            uint length = (uint) i.Length;
            byte[] o = new byte[length];
            if (!encipher(o, length, i))
            {
                throw new Exception("キーが設定されていません");
            }
            return o;
        }

        public string Encrypt(string target) => 
            this.ByteToHex(this.Encrypt(this.CreateAlignBytes(target)));

        private byte GetHex(char x)
        {
            if (x <= '9' && x >= '0')
            {
                return (byte)(x - 48);
            }
            if (x <= 'z' && x >= 'a')
            {
                return (byte)(x - 97 + 10);
            }
            if (x > 'Z' || x < 'A')
            {
                return (byte)0;
            }
            return (byte)(x - 65 + 10);
        }
        private byte[] HexToByte(string hex)
        {
            byte[] buffer = new byte[hex.Length / 2];
            for (int i = 0; i < (hex.Length - 1); i += 2)
            {
                byte num2 = this.GetHex(hex[i]);
                byte num3 = this.GetHex(hex[i + 1]);
                buffer[i / 2] = (byte) ((num2 * 0x10) + num3);
            }
            return buffer;
        }

        [DllImport("blowfish_cpp.dll")]
        private static extern bool set_key(string key, uint keySize);
    }
}

