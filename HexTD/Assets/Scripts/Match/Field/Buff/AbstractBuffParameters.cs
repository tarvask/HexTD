using UniRx;

namespace Match.Field.Buff
{
    public abstract class AbstractBuffParameters
    {
        protected const byte UndefinedBuffSubtypeShift = 127;
        private readonly string _buffName;
        private readonly BuffedParameterType _buffedParameterType;
        private readonly BuffParameterComputingType _buffMathType;

        public string BuffName => _buffName;
        public BuffedParameterType BuffedParameterType => _buffedParameterType;
        public BuffParameterComputingType BuffMathType => _buffMathType;
        public abstract byte BuffSubType { get; }
        public abstract byte UndefinedBuffSubTypeIndex { get; }
        public abstract float BuffValue { get; }
        public abstract bool IsBuffPermanent { get; }
        public abstract float BuffDuration { get; }
        public abstract bool IsProgressive { get; }

        protected AbstractBuffParameters(string buffName,
            BuffedParameterType buffedParameterType,
            BuffParameterComputingType buffMathType)
        {
            _buffedParameterType = buffedParameterType;
            _buffName = buffName;
            _buffMathType = buffMathType;
        }

        public virtual void InitTimer(IReadOnlyReactiveProperty<float> timerReactiveProperty)
        {
            
        }

        public abstract string GetTextInfo();
    }
}