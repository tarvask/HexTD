using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Match.Field.Shooting;
using Match.Serialization;
using Tools;
using Tools.Interfaces;

namespace BuffLogic
{
    public interface ITypedBuffManager : IOuterLogicUpdatable, ISerializableToNetwork
    {
        void AddBuff(IBuffableValue targetValue, IBuff buff);
        bool IsBuffs(IBuffableValue targetValue);
        void Clear();
    }
    
    public class TypedBuffManager : BaseDisposable, ITypedBuffManager
    {
        private const string BuffTargetType = "BuffTargetType";
        
        public Type Type { get; }
        private readonly Dictionary<IBuffableValue, PrioritizedBuffLinkedList> _buffs;

        public TypedBuffManager(Type type)
        {
            Type = type;
            _buffs = new Dictionary<IBuffableValue, PrioritizedBuffLinkedList>(5);
        }

        public void AddBuff(IBuffableValue targetValue, IBuff buff)
        {
            if (Type != buff.TargetType)
                return;
            
            if (!_buffs.TryGetValue(targetValue, out var buffList))
            {
                buffList = new PrioritizedBuffLinkedList(targetValue);
                targetValue.SubscribeOnDispose(RemoveBuffableValue);
                _buffs.Add(targetValue, buffList);
            }
            
            buffList.AddBuff(buff);
        }

        public bool IsBuffs(IBuffableValue targetValue)
        {
            return _buffs.ContainsKey(targetValue);
        }
        
        public void RemoveBuffableValue(IBuffableValue buffableValue)
        {
            _buffs[buffableValue].Dispose();
            _buffs.Remove(buffableValue);
        }

        public void Clear()
        {
            foreach (var buffPair in _buffs)
            {
                buffPair.Value.Dispose();
            }
            
            _buffs.Clear();
        }

        public void OuterLogicUpdate(float frameLength)
        {
            foreach (var buffValuePair in _buffs)
            {
                buffValuePair.Value.OuterLogicUpdate(frameLength);
            }
        }
        
        public Hashtable ToNetwork()
        {
            Hashtable hashtable = new Hashtable();
            
            hashtable.Add(BuffTargetType, Type.FullName);
            hashtable.Add(SerializerToNetwork.EnumerableSize, _buffs.Count);
            
            int i = 0;
            foreach (var buffPair in _buffs)
            {
                hashtable.Add($"{PhotonEventsConstants.SyncState.PlayerState.Buffs.TargetId}{i}", buffPair.Key.TargetId);
                hashtable.Add($"{PhotonEventsConstants.SyncState.PlayerState.Buffs.EntityBuffableValueType}{i}", 
                    (byte)buffPair.Key.EntityBuffableValueType);
                hashtable.Add($"{PhotonEventsConstants.SyncState.PlayerState.Buffs.BuffValueParam}{i}", 
                    SerializerToNetwork.EnumerableToNetwork(buffPair.Value, buffPair.Value.Count));
                i++;
            }

            return hashtable;
        }
        
        public static TypedBuffManager FromNetwork(TargetContainer targetContainer1, TargetContainer targetContainer2, Hashtable hashtable)
        {
            if (!SerializerToNetwork.TryGetType(hashtable, BuffTargetType, out Type type))
                return new TypedBuffManager(null);

            TypedBuffManager typedBuffManager = new TypedBuffManager(type);
            int size = (int)hashtable[SerializerToNetwork.EnumerableSize];
            for(int i = 0; i < size; i++)
            {
                int targetId = (int)hashtable[$"{PhotonEventsConstants.SyncState.PlayerState.Buffs.TargetId}{i}"];
                EntityBuffableValueType entityBuffableValueType = 
                    (EntityBuffableValueType)hashtable[$"{PhotonEventsConstants.SyncState.PlayerState.Buffs.EntityBuffableValueType}{i}"];

                if(!targetContainer1.TryGetTargetById(targetId, out var target) &&
                   !targetContainer2.TryGetTargetById(targetId, out target))
                    continue;
                target.BaseReactiveModel.TryGetBuffableValue(entityBuffableValueType, out var buffableTarget);

                var buffsHashtable = (Hashtable)hashtable[$"{PhotonEventsConstants.SyncState.PlayerState.Buffs.BuffValueParam}{i}"];
                foreach (var buffHashtable in SerializerToNetwork.IterateSerializedEnumerable(buffsHashtable))
                {
                    IBuff buff = SerializerToNetwork.FromNetwork(buffHashtable) as IBuff;
                    typedBuffManager.AddBuff(buffableTarget, buff);
                }
                
                i++;
            }

            return typedBuffManager;
        }
    }
}