using System;
using Match.Field.Shooting;

namespace BuffLogic.SerializableBuffs
{
    [Serializable]
    public abstract class BaseSerializableBuff
    {
        public abstract void ApplyBuff(ITarget target, BuffManager buffManager);
    }
}