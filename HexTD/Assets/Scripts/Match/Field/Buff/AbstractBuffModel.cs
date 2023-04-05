using Tools;
using Tools.Interfaces;
using UniRx;

namespace Match.Field.Buff
{
    public class AbstractBuffModel : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly AbstractBuffParameters _buffParameters;
        private readonly ReactiveProperty<float> _timerReactiveProperty; 

        public AbstractBuffParameters BuffParameters => _buffParameters;
        public IReadOnlyReactiveProperty<float> TimerReactiveProperty => _timerReactiveProperty;

        public bool HasRemainingDuration => _buffParameters.IsBuffPermanent || _timerReactiveProperty.Value < _buffParameters.BuffDuration;

        public AbstractBuffModel(AbstractBuffParameters buffParameters)
        {
            _timerReactiveProperty = AddDisposable(new ReactiveProperty<float>(0));
            _buffParameters = buffParameters;
            _buffParameters.InitTimer(_timerReactiveProperty);
        }

        public string GetTextInfo()
        {
            return _buffParameters.GetTextInfo();
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _timerReactiveProperty.Value += frameLength;
        }
    }
}