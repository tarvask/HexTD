using ExitGames.Client.Photon;

namespace BuffLogic
{
    public interface ISerializableFromNetwork : ISerializableToNetwork
    {
        object Restore(Hashtable hashtable);
    }

    public interface ISerializableToNetwork
    {
        Hashtable ToNetwork();
    }

    public interface ISerializableToNetwork<out T, TValue> : ISerializableToNetwork where T : TValue
    {
        T SerializeFromNetwork(Hashtable hashtable);
    }
}