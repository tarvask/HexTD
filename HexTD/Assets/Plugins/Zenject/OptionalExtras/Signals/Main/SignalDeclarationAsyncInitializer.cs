using System.Collections.Generic;
using Plugins.Zenject.OptionalExtras.Signals.Internal;
using Plugins.Zenject.Source.Injection;
using Plugins.Zenject.Source.Internal;
using Plugins.Zenject.Source.Runtime;
using Zenject;

namespace Plugins.Zenject.OptionalExtras.Signals.Main
{
    // This class just exists to solve a circular dependency that would otherwise happen if we
    // attempted to inject TickableManager into either SignalDeclaration or SignalBus
    // And we need to directly depend on TickableManager because we need each SignalDeclaration
    // to have a unique tick priority
    public class SignalDeclarationAsyncInitializer : IInitializable
    {
        readonly LazyInject<TickableManager> _tickManager;
        readonly List<SignalDeclaration> _declarations;

        public SignalDeclarationAsyncInitializer(
            [Inject(Source = InjectSources.Local)]
            List<SignalDeclaration> declarations,
            [Inject(Optional = true, Source = InjectSources.Local)]
            LazyInject<TickableManager> tickManager)
        {
            _declarations = declarations;
            _tickManager = tickManager;
        }

        public void Initialize()
        {
            for (int i = 0; i < _declarations.Count; i++)
            {
                var declaration = _declarations[i];

                if (declaration.IsAsync)
                {
                    Assert.IsNotNull(_tickManager.Value, "TickableManager is required when using asynchronous signals");
                    _tickManager.Value.Add(declaration, declaration.TickPriority);
                }
            }
        }
    }
}

