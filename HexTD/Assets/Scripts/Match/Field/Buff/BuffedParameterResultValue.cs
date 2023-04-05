using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Match.Field.Buff
{
    public class BuffedParameterResultValue
    {
        protected readonly IReadOnlyReactiveProperty<float> _baseValue;
        
        protected float _baseSum;
        protected float _baseProduct;
        protected float _finalSum;
        protected float _finalProduct;

        public float Value { get; protected set; }

        protected BuffedParameterResultValue(IReadOnlyReactiveProperty<float> baseValue)
        {
            _baseValue = baseValue;
        }

        public virtual void RecomputeValue(Dictionary<int, AbstractBuffParameters> buffsOfType, float minValue, float maxValue)
        {
            _baseSum = 0;
            _baseProduct = 1;
            _finalSum = 0;
            _finalProduct = 1;

            foreach (KeyValuePair<int, AbstractBuffParameters> buffKeyValuePair in buffsOfType)
                HandleBuffParameter(buffKeyValuePair.Value.BuffMathType, buffKeyValuePair.Value.BuffValue);

            Value = Mathf.Clamp(
                (_baseValue.Value * _baseProduct + _baseSum) * _finalProduct + _finalSum,
                minValue, maxValue);
        }


        protected void HandleBuffParameter(BuffParameterComputingType buffComputingType, float buffValue)
        {
            switch (buffComputingType)
            {
                case BuffParameterComputingType.BaseAddendum:
                    _baseSum += buffValue;
                    break;
                case BuffParameterComputingType.BaseMultiplier:
                    _baseProduct += buffValue;
                    break;
                case BuffParameterComputingType.FinalAddendum:
                    _finalSum += buffValue;
                    break;
                case BuffParameterComputingType.FinalMultiplier:
                    _finalProduct += buffValue;
                    break;
            }
        }
    }
}