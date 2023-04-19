using System;
using System.Collections.Generic;
using Tools.Interfaces;
using Tools.PriorityTools;
using UnityEngine;

namespace BuffLogic
{
    public class PrioritizedBuffLinkedList<TValue> : PrioritizeLinkedList<IBuff<TValue>>, IOuterLogicUpdatable
    {
        private readonly List<IBuff<TValue>> _removedBuffs;
        private readonly IBuffableValue<TValue> _buffableValueTarget;

        public bool IsAlive => ValueList.Count > 0;

        public PrioritizedBuffLinkedList(IBuffableValue<TValue> buffableValue)
        {
            _removedBuffs = new List<IBuff<TValue>>();
            _buffableValueTarget = buffableValue;
        }

        public void AddBuff(IBuff<TValue> buff)
        {
            Add(buff);
            _buffableValueTarget.UpdateAddBuff(this, buff);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _removedBuffs.Clear();
            bool wasUpdated = false;

            for (var valueNode = ValueList.First; valueNode != null;)
            {
                TryToUpdateBuff(valueNode.Value);
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

        }

        private void TryToUpdateBuff(IBuff<TValue> buff)
        {
            try
            {
                buff.Update();
            }
            catch (Exception e)
            {
                Debug.LogError("Buff's update failed with exception: " + e.Message);
            }
        }
    }
}