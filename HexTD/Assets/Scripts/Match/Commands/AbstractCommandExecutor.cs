using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Match.EventBus;
using UniRx;

namespace Match.Commands
{
    public abstract class AbstractCommandExecutor : ICommandExecutor
    {
        public struct Context
        {
            public TestMatchEngine MatchEngine { get; }
            public IEventBus EventBus { get; }
            public IReadOnlyReactiveProperty<int> MinEngineFrameToProcessReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> PingDamperFramesDeltaReactiveProperty { get; }
            public IReadOnlyReactiveProperty<NetworkRoles> CurrentProcessRoleReactiveProperty { get; }

            public Context(TestMatchEngine matchEngine, IEventBus eventBus,
                IReadOnlyReactiveProperty<int> minEngineFrameToProcessReactiveProperty,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty,
                IReadOnlyReactiveProperty<int> pingDamperFramesDeltaReactiveProperty,
                IReadOnlyReactiveProperty<NetworkRoles> currentProcessRoleReactiveProperty)
            {
                MatchEngine = matchEngine;
                EventBus = eventBus;
                MinEngineFrameToProcessReactiveProperty = minEngineFrameToProcessReactiveProperty;
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
                PingDamperFramesDeltaReactiveProperty = pingDamperFramesDeltaReactiveProperty;
                CurrentProcessRoleReactiveProperty = currentProcessRoleReactiveProperty;
            }
        }

        private readonly ICommandImplementation _serverImplementation;
        private readonly ICommandImplementation _clientImplementation;
        private ICommandImplementation _currentImplementation;

        protected AbstractCommandExecutor(Context context)
        {
            _serverImplementation = CreateServerImplementation(context);
            _clientImplementation = CreateClientImplementation(context);
            UpdateRole(context.CurrentProcessRoleReactiveProperty.Value);

            context.CurrentProcessRoleReactiveProperty.Subscribe(UpdateRole);
        }

        protected abstract ICommandImplementation CreateServerImplementation(Context context);
        protected abstract ICommandImplementation CreateClientImplementation(Context context);

        public async Task Request(Hashtable parametersTable)
        {
            await _currentImplementation.Request(parametersTable);
        }

        public async Task Apply(Hashtable parametersTable)
        {
            await _currentImplementation.Apply(parametersTable);
        }

        private void UpdateRole(NetworkRoles newRole)
        {
            _currentImplementation = newRole == NetworkRoles.Server ?
                _serverImplementation
                : _clientImplementation;
        }
    }
}