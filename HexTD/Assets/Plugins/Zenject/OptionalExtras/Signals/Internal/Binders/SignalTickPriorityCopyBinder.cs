using Plugins.Zenject.OptionalExtras.Signals.Internal.Binders.DeclareSignal;
using Zenject;

namespace Plugins.Zenject.OptionalExtras.Signals.Internal.Binders
{
    [NoReflectionBaking]
    public class SignalTickPriorityCopyBinder : SignalCopyBinder
    {
        public SignalTickPriorityCopyBinder(
            SignalDeclarationBindInfo signalBindInfo)
        {
            SignalBindInfo = signalBindInfo;
        }

        protected SignalDeclarationBindInfo SignalBindInfo
        {
            get; private set;
        }

        public SignalCopyBinder WithTickPriority(int priority)
        {
            SignalBindInfo.TickPriority = priority;
            SignalBindInfo.RunAsync = true;
            return this;
        }
    }
}

