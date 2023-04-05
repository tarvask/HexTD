using Plugins.Zenject.OptionalExtras.Signals.Internal;
using Plugins.Zenject.OptionalExtras.Signals.Internal.Binders.DeclareSignal;
using Plugins.Zenject.Source.Install;
using Plugins.Zenject.Source.Internal;

namespace Plugins.Zenject.OptionalExtras.Signals.Main
{
    // Note that you only need to install this once
    public class SignalBusInstaller : Installer<SignalBusInstaller>
    {
        public override void InstallBindings()
        {
            Assert.That(!Container.HasBinding<SignalBus>(), "Detected multiple SignalBus bindings.  SignalBusInstaller should only be installed once");

            Container.BindInterfacesAndSelfTo<SignalBus>().AsSingle().CopyIntoAllSubContainers();

            Container.BindInterfacesTo<SignalDeclarationAsyncInitializer>().AsSingle().CopyIntoAllSubContainers();

            Container.BindMemoryPool<SignalSubscription, SignalSubscription.Pool>();

            // Dispose last to ensure that we don't remove SignalSubscription before the user does
            Container.BindLateDisposableExecutionOrder<SignalBus>(-999);

            Container.BindFactory<SignalDeclarationBindInfo, SignalDeclaration, SignalDeclaration.Factory>();
        }
    }
}
