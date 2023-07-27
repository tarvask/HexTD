using System;
using ExitGames.Client.Photon;

namespace Match.Serialization
{
    /// <summary>
    /// serialization field|property like int, byte, etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeToNetwork : Attribute
    {
        public string Key { get; }
        public SerializeToNetwork(string key) => Key = key;
    }
    
    /// <summary>
    /// serialization field | property for complex type which includes SerializeToNetwork fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializableToNetwork : Attribute
    {
        public string Key { get; }
        public SerializableToNetwork(string key) => Key = key;
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializableArrayToNetwork : Attribute
    {
        public string Key { get; }
        public Type TypeOfElements { get; }

        public SerializableArrayToNetwork(string key, Type typeOfElements)
        {
            Key = key;
            TypeOfElements = typeOfElements;
        }
    }
    
    /// <summary>
    /// class which take Hashtable and return objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SerializableFromNetwork : Attribute
    {
        public Action<Hashtable, object> RestoreObject { get; }

        public SerializableFromNetwork(Action<Hashtable, object> restoreObject) => RestoreObject = restoreObject;
    }
}