using System;
using System.Collections.Generic;
using Tools.Interfaces;
using Tools.PriorityTools;
using UnityEngine;

namespace BuffLogic
{
    public class PrioritizedBuffLinkedList : PrioritizeLinkedList<IBuff>, IOuterLogicUpdatable
    {
        private readonly List<IBuff> _removedBuffs;
        private readonly IBuffableValue _buffableValueTarget;

        public bool IsAlive => ValueList.Count > 0;

        public PrioritizedBuffLinkedList(IBuffableValue buffableValue)
        {
            _removedBuffs = new List<IBuff>();
            _buffableValueTarget = buffableValue;
        }

        public void AddBuff(IBuff newBuff)
        {
            foreach (var buff in ValueList)
            {
                if (buff.GetType() == newBuff.GetType())
                {
                    buff.MergeBuffs(newBuff);
                    _buffableValueTarget.UpdateAddBuff(this, buff);
                    return;
                }
            }
            
            Add(newBuff);
            _buffableValueTarget.UpdateAddBuff(this, newBuff);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _removedBuffs.Clear();
            bool wasUpdated = false;

            for (var valueNode = ValueList.First; valueNode != null;)
            {
                TryToUpdateBuff(frameLength, valueNode.Value);
                if (valueNode.Value.IsEndConditionDone)
                {
                    wasUpdated = true;

                    var nextNode = valueNode.Next;
                    _removedBuffs.Add(valueNode.Value);
                    
                    ValueList.Remove(valueNode);
                    valueNode = nextNode;
                }
                else
                    valueNode = valueNode.Next;
            }

            if (wasUpdated)
                _buffableValueTarget.UpdateRemoveBuffs(this, _removedBuffs);

            foreach (var removedBuff in _removedBuffs)
                removedBuff.Dispose();
        }

        private void TryToUpdateBuff(float frameLength, IBuff buff)
        {
            try
            {
                buff.OuterLogicUpdate(frameLength);
            }
            catch (Exception e)
            {
                Debug.LogError("Buff's update failed with exception: " + e.Message);
            }
        }
    }
}