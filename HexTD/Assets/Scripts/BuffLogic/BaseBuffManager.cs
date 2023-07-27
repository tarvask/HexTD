using System.Collections.Generic;
using ExitGames.Client.Photon;
using Tools;
using Tools.Interfaces;

namespace BuffLogic
{
    public interface ITypedBuffManager : IOuterLogicUpdatable, ISerializableToNetwork
    {
        void AddBuff(IBuffableValue targetValue, IBuff buff);
        bool IsBuffs(IBuffableValue targetValue);
    }
    
    public class TypedBuffManager : BaseDisposable, ITypedBuffManager
    {
        private readonly Dictionary<IBuffableValue, PrioritizedBuffLinkedList> _buffs;

        public TypedBuffManager()
        {
            _buffs = new Dictionary<IBuffableValue, PrioritizedBuffLinkedList>(5);
        }

        public void AddBuff(IBuffableValue targetValue, IBuff buff)
        {
            if (!_buffs.TryGetValue(targetValue, out var buffList))
            {
                buffList = new PrioritizedBuffLinkedList(targetValue);
                _buffs.Add(targetValue, buffList);
            }
            
            buffList.AddBuff(buff);
        }

        public bool IsBuffs(IBuffableValue targetValue)
        {
            var target = (IBuffableValue)targetValue;

            return _buffs.ContainsKey(target);
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
            
            hashtable.Add(PhotonEventsConstants.SyncState.PlayerState.Buffs.BuffSizeParam, _buffs.Count);
            
            int i = 0;
            foreach (var buffPair in _buffs)
            {
                hashtable.Add($"{PhotonEventsConstants.SyncState.PlayerState.Buffs.TargetId}{i}", buffPair.Key.TargetId);
                hashtable.Add($"{PhotonEventsConstants.SyncState.PlayerState.Buffs.EntityBuffableValueType}{i}", (byte)buffPair.Key.EntityBuffableValueType);
                hashtable.Add($"{PhotonEventsConstants.SyncState.PlayerState.Buffs.BuffValueParam}{i}", 
                    Match.Serialization.SerializerToNetwork.EnumerableToNetwork(buffPair.Value, buffPair.Value.Count));
                i++;
            }

            return hashtable;
        }
    }
}