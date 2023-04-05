namespace Match.Field.Buff
{
    public abstract class ProgressiveBuffParameters : AbstractBuffParameters
    {
        protected readonly float _buffValue;
        private readonly bool _isBuffPermanent;
        private readonly float _buffDuration;

        //public override float BuffValue => _buffValue;
        public override bool IsBuffPermanent => _isBuffPermanent;
        public override float BuffDuration => _buffDuration;
        public override bool IsProgressive => true;
        
        protected ProgressiveBuffParameters(string buffName,
            BuffedParameterType buffedParameterType,
            BuffParameterComputingType buffMathType,
            float buffValue,
            bool isBuffPermanent,
            float buffDuration) : base(buffName, buffedParameterType, buffMathType)
        {
            _buffValue = buffValue;
            _isBuffPermanent = isBuffPermanent;
            _buffDuration = buffDuration;
        }
    }
}