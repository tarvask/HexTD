using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Match.Field.Shooting;
using Match.Serialization;
using Tools;
using Tools.Interfaces;

namespace BuffLogic
{
    public sealed class BuffManager : BaseDisposable, IOuterLogicUpdatable, ISerializableToNetwork
    {
        private readonly Dictionary<Type, ITypedBuffManager> _buffManagers;

        public BuffManager()
        {
            _buffManagers = new Dictionary<Type, ITypedBuffManager>();
        }

        public void AddBuff(IBuffableValue targetValue, IBuff buff, Type type)
        {
            if (!_buffManagers.TryGetValue(type, out ITypedBuffManager buffManager))
            {
                buffManager = AddDisposable(new TypedBuffManager(type));
                _buffManagers.Add(type, buffManager);
            }
            
            buffManager.AddBuff(targetValue, buff);
        }
        
        public bool HasBuffs(IBuffableValue targetValue)
        {
            if (_buffManagers.TryGetValue(typeof(IBuffableValue), out ITypedBuffManager buffManager))
            {
                return buffManager.IsBuffs(targetValue);
            }

            return false;
        }

        public void AddBuff<TValue>(IBuffableValue targetValue, IBuff<TValue> buff)
        {
            AddBuff(targetValue, buff, typeof(TValue));
        }

        protected override void OnDispose()
        {
            Clear();
        }

        public void OuterLogicUpdate(float frameLength)
        {
            foreach (var buffManager in _buffManagers)
            {
                buffManager.Value.OuterLogicUpdate(frameLength);
            }
        }
        
        public void Clear()
        {
            foreach (var typedBuffManager in _buffManagers)
            {
                typedBuffManager.Value.Clear();
            }
            
            _buffManagers.Clear();
        }
        
        public Hashtable ToNetwork()
        {
            return SerializerToNetwork.EnumerableToNetwork(_buffManagers.Values,
                _buffManagers.Values.Count);
        }
        
        public void FromNetwork(TargetContainer targetContainer, Hashtable hashtable)
        {
            Clear();
            foreach (var elementHashtable in SerializerToNetwork.IterateSerializedEnumerable(hashtable))
            {
                var typedBuffManager = TypedBuffManager.FromNetwork(targetContainer, elementHashtable.Item1);
                _buffManagers.Add(typedBuffManager.Type, typedBuffManager);
            }
        }
    }
}