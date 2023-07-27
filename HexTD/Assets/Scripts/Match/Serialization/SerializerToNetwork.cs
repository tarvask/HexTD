using System;
using System.Collections.Generic;
using System.Reflection;
using BuffLogic;
using ExitGames.Client.Photon;
using Match.Field.Shooting;
using Newtonsoft.Json;
using Unity.VisualScripting;

namespace Match.Serialization
{
    public static class SerializerToNetwork
    {
        public const string SerializedType = "SerializedType";
        private const string EnumerableElement = "EnumerableElement";
        private const string EnumerableSize = "EnumerableSize";

        public static Hashtable EnumerableToNetwork<T>(IEnumerable<T> enumerable, int size) where T : ISerializableToNetwork
        {
            Hashtable hashtable = new Hashtable();
            
            hashtable.Add(EnumerableSize, size);

            int i = 0;
            foreach (var buffCondition in enumerable)
            {
                hashtable.Add($"{EnumerableElement}{i++}", buffCondition.ToNetwork());
            }

            return hashtable;
        }

        public static Hashtable ToNetwork(object value)
        {
            var iSerializable = value as ISerializableToNetwork;
            if (iSerializable != null)
            {
                return iSerializable.ToNetwork();
            }
            
            Hashtable hashtable = new Hashtable();
            var type = value.GetType();

            FieldInfo[] fields = type.GetFields();
            hashtable.Add(SerializedType, type.FullName);
            foreach (var field in fields)
            {
                if (TryGetCustomAttribute<SerializeToNetwork>(type, out var serialize))
                {
                    if (TryGetCustomAttribute<SerializableToNetwork>(type, out var serializable))
                    {
                        hashtable.Add(serialize.Key, ToNetwork(serializable));
                    }
                    else
                    {
                        object fieldValue = field.GetValue(null);
                        hashtable.Add(serialize.Key, fieldValue);
                    }
                }
            }
            
            return hashtable;
        }

        public static void CreateBuffs(TargetContainer targetContainer, BuffManager buffManager, Hashtable hashtable)
        {
            int i = 0;
            foreach (var typedBuffManagerHashtable in IterateSerializedEnumerable(hashtable))
            {
                int targetId = (int)hashtable[$"{PhotonEventsConstants.SyncState.PlayerState.Buffs.TargetId}{i}"];
                EntityBuffableValueType entityBuffableValueType = (EntityBuffableValueType)hashtable[$"{PhotonEventsConstants.SyncState.PlayerState.Buffs.EntityBuffableValueType}{i}"];

                targetContainer.TryGetTargetById(targetId, out var target);
                target.BaseReactiveModel.TryGetBuffableValue(entityBuffableValueType, out var buffableTarget);

                Hashtable buffsHashtable = (Hashtable)hashtable[$"{PhotonEventsConstants.SyncState.PlayerState.Buffs.BuffValueParam}{i}"];

                foreach (var buffTable in IterateSerializedEnumerable(buffsHashtable))
                {
                    if (!TryGetType(buffTable, out Type type))
                        continue;

                    var buff = FromNetwork(buffTable, type) as IBuff;
                    buffManager.AddBuff(buffableTarget, buff, type);
                }
            }
        }

        public static IEnumerable<Hashtable> IterateSerializedEnumerable(Hashtable hashtable)
        {
            int size = (int)hashtable[EnumerableSize];
            for(int i = 0; i < size; i++)
            {
                yield return (Hashtable)hashtable[$"{EnumerableElement}{i}"];
            }
        }

        private static bool TryGetType(Hashtable hashtable, out Type type)
        {
            if (!hashtable.TryGetValue(SerializedType, out object value))
            {
                type = null;
                return false;
            }
                    
            string serializedTypeName = hashtable[SerializedType] as string;
            type = Type.GetType(serializedTypeName);

            return true;
        }
        
        public static object FromNetwork(Hashtable hashtable, Type type)
        {
            return type.GetConstructor(new Type[] { typeof(Hashtable) })?.Invoke(new object[] { hashtable });
        }

        private static bool TryGetCustomAttribute<T>(Type type, out T attribute) where T : Attribute
        {
            attribute = type.GetCustomAttribute<T>();
            return attribute != null;
        }
    }
}