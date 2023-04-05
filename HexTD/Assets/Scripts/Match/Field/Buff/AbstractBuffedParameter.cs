using Tools;
using UniRx;

namespace Match.Field.Buff
{
    public abstract class AbstractBuffedParameter : BaseDisposable
    {
        protected BuffedParameterType BuffedParameterType;
        protected IReadOnlyReactiveProperty<float> BaseValue;
        protected float MinValue;
        protected float MaxValue;

        protected abstract BuffedParameterResultValue ResultValue { get; }

        public float ResultBuffedValue => ResultValue.Value;

        public abstract void AddBuff(int buffKey, AbstractBuffParameters buffParameters);

        public abstract void RemoveBuff(int buffKey);

        public abstract void ClearBuffs();

        public abstract bool TryGetBuffBySubtype(byte buffSubtypeByte, out int buffKey);
    }
}