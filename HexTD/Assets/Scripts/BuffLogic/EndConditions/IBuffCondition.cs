using System;

namespace BuffLogic
{
    public interface IBuffCondition : IDisposable, ISerializableToNetwork
    {
        bool Invoke();
    }
}