using System;
using System.Collections.Generic;
using BuffLogic;
using ExitGames.Client.Photon;
using Match.Field;
using Match.Field.Castle;
using Match.Field.Mob;
using Match.Field.Tower;
using Services;

namespace Match.Serialization
{
    public class SerializerToNetwork
    {
        private struct SerializableData
        {
            public string FullName { get; }
            public Type Type { get; }
            public Func<Hashtable, object> DeserializeObject { get; }
            
            public SerializableData(Type type, Func<Hashtable, object> deserializeObject)
            {
                FullName = type.FullName;
                Type = type;    
                DeserializeObject = deserializeObject;
            }
        }
        
        public const string SerializedType = "SerializedType";
        public const string EnumerableElement = "EnumerableElement";
        public const string EnumerableSize = "EnumerableSize";
        
        private static Dictionary<Type, SerializableData> _serializableDatas;

        public static void InitSerializableData(ConfigsRetriever configsRetriever, FieldFactory factory)
        {
            if (_serializableDatas == null)
                _serializableDatas = new Dictionary<Type, SerializableData>();
            
            _serializableDatas.Clear();
            
            _serializableDatas.Add(typeof(CastleController), new SerializableData(typeof(CastleController), CastleController.FromNetwork));
            
            _serializableDatas.Add(typeof(MobsContainer), new SerializableData(typeof(MobsContainer), MobsContainer.FromNetwork));
            _serializableDatas.Add(typeof(MobController), new SerializableData(typeof(MobController), 
                hashtable => MobController.FromNetwork(hashtable, configsRetriever, factory)));
            
            _serializableDatas.Add(typeof(TowerController), new SerializableData(typeof(TowerController), 
                hashtable => TowerController.FromNetwork(hashtable, configsRetriever, factory)));
            _serializableDatas.Add(typeof(TowerContainer), new SerializableData(typeof(TowerContainer), 
                hashtable => TowerContainer.FromNetwork(hashtable, configsRetriever)));
            
            _serializableDatas.Add(typeof(HealBuff), new SerializableData(typeof(HealBuff), HealBuff.FromNetwork));
            _serializableDatas.Add(typeof(PoisonBuff), new SerializableData(typeof(PoisonBuff), PoisonBuff.FromNetwork));
            _serializableDatas.Add(typeof(MultiFloatValueBuff), new SerializableData(typeof(MultiFloatValueBuff), MultiFloatValueBuff.FromNetwork));
            
            _serializableDatas.Add(typeof(BuffConditionOnceCollection), new SerializableData(typeof(BuffConditionOnceCollection), BuffConditionOnceCollection.FromNetwork));
        }

        public static void PartlyReinitSerializableData(ConfigsRetriever configsRetriever, FieldFactory factory)
        {
            _serializableDatas[typeof(MobController)] = new SerializableData(typeof(MobController), 
                hashtable => MobController.FromNetwork(hashtable, configsRetriever, factory));
            _serializableDatas[typeof(TowerController)] = new SerializableData(typeof(TowerController), 
                hashtable => TowerController.FromNetwork(hashtable, configsRetriever, factory));
            _serializableDatas[typeof(TowerContainer)] = new SerializableData(typeof(TowerContainer), 
                hashtable => TowerContainer.FromNetwork(hashtable, configsRetriever));
        }

        public static Hashtable EnumerableToNetwork<T>(IEnumerable<T> enumerable, int size) where T : ISerializableToNetwork
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add(EnumerableSize, size);
            
            int i = 0;
            foreach (var serializableToNetwork in enumerable)
            {
                AddToHashTable(serializableToNetwork, hashtable, i.ToString());
            }

            return hashtable;
        }
        
        public static void AddToHashTable<T>(T element, Hashtable targetHashtable, string key) where T : ISerializableToNetwork
        {
            Hashtable hashtable = element.ToNetwork();
            hashtable.Add($"{SerializedType}{key}", _serializableDatas[typeof(T)].FullName);
            targetHashtable.Add(key, hashtable);
        }

        public static IEnumerable<(Hashtable, Type)> IterateSerializedEnumerable(Hashtable hashtable)
        {
            int size = (int)hashtable[EnumerableSize];
            for(int i = 0; i < size; i++)
            {
                Hashtable elementHashtable = (Hashtable)hashtable[i.ToString()];
                if(!TryGetType(elementHashtable, out var type))
                    continue;
                
                yield return ((Hashtable)hashtable[$"{EnumerableElement}{i}"], type);
            }
        }
        
        public static object FromNetwork((Hashtable, Type) hashtableTypePair)
        {
            return _serializableDatas[hashtableTypePair.Item2].DeserializeObject.Invoke(hashtableTypePair.Item1);
        }
        
        public static object FromNetwork(string key, Hashtable hashtable)
        {
            Hashtable typedHashtable = (Hashtable)hashtable[key];
            if (!TryGetType(typedHashtable, out var type))
                return null;
            
            return _serializableDatas[type].DeserializeObject.Invoke(typedHashtable);
        }

        public static bool TryGetType(Hashtable hashtable, string key, out Type type)
        {
            if (!hashtable.TryGetValue(key, out object value))
            {
                type = null;
                return false;
            }
                    
            string serializedTypeName = hashtable[SerializedType] as string;
            type = Type.GetType(serializedTypeName);

            return true;
        }

        private static bool TryGetType(Hashtable hashtable, out Type type)
        {
            return TryGetType(hashtable, SerializedType, out type);
        }
    }
}