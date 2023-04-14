using System;

namespace BuffLogic
{
    public interface IBuffCondition : IDisposable
    {
        bool Invoke();
    }
}