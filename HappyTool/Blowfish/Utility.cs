using System;

namespace Blowfish
{
  public class Utility
  {
    public static int AlignSize(int size)
    {
      int num = (size & 7) != 0 ? 1 : 0;
      return (size / 8 + num) * 8;
    }

    public static int PaddingSize(int size) => Utility.AlignSize(size) - size;

    private byte[] CreateAlignBytes(byte[] source)
    {
      byte[] array = source;
      if (Utility.AlignSize(source.Length) != source.Length)
      {
        Array.Resize<byte>(ref array, source.Length);
        Array.Clear((Array) array, source.Length, array.Length - source.Length);
      }
      return array;
    }
  }
}
