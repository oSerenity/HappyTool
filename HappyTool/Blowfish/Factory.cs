namespace Blowfish
{
  public class Factory
  {
    public static Cypher Create(Type type, string key)
    {
      Cypher cypher =  null;
      switch (type)
      {
        case Type.Native:
          cypher =  new Native(key);
          break;
        case Type.Safe:
          cypher =  new SafeCpp(key);
          break;
      }
      return cypher;
    }

    public enum Type
    {
      Native,
      Safe,
    }
  }
}
