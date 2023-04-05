using Plugins.Zenject.OptionalExtras.Signals.Internal.Binders.DeclareSignal;
using Plugins.Zenject.Source.Main;

namespace Plugins.Zenject.OptionalExtras.Signals.Internal.Binders.BindSignal
{
    public class BindSignalIdToBinder<TSignal> : BindSignalToBinder<TSignal>
    {
        public BindSignalIdToBinder(DiContainer container, SignalBindingBindInfo signalBindInfo)
            : base(container, signalBindInfo)
        {
        }

        public BindSignalToBinder<TSignal> WithId(object identifier)
        {
            SignalBindInfo.Identifier = identifier;
            return this;
        }
    }
}

