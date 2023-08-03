using System;
using Match.Serialization;

namespace BuffLogic
{
    public interface IBuffCondition : IDisposable, ISerializableToNetwork
    {
        bool Invoke();
    }
}