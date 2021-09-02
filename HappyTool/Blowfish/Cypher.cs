namespace Blowfish
{
    using System;

    public interface Cypher
    {
        byte[] Decrypt(byte[] target);
        string Decrypt(string target);
        byte[] Encrypt(byte[] target);
        string Encrypt(string target);
    }
}

