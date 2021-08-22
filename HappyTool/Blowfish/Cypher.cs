namespace Blowfish
{
  public interface Cypher
  {
    byte[] Encrypt(byte[] target);

    string Encrypt(string target);

    byte[] Decrypt(byte[] target);

    string Decrypt(string target);
  }
}
