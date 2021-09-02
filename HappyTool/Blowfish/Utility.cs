namespace Blowfish
{
    using System;

    public class Utility
    {
        public static int AlignSize(int size)
        {
            int num = ((size & 7) != 0) ? 1 : 0;
            return (((size / 8) + num) * 8);
        }

        private byte[] CreateAlignBytes(byte[] source)
        {
            byte[] array = source;
            if (AlignSize(source.Length) != source.Length)
            {
                Array.Resize<byte>(ref array, source.Length);
                Array.Clear(array, source.Length, array.Length - source.Length);
            }
            return array;
        }

        public static int PaddingSize(int size) => 
            AlignSize(size) - size;
    }
}

