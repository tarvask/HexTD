using System.Threading.Tasks;
using ExitGames.Client.Photon;

namespace Match.Commands
{
    public abstract class AbstractCommandImplementation : ICommandImplementation
    {
        protected readonly AbstractCommandExecutor.Context Context;
        
        protected AbstractCommandImplementation(AbstractCommandExecutor.Context context)
        {
            Context = context;
        }
            
        public abstract Task Request(Hashtable commandParametersTable);

        public abstract Task Apply(Hashtable commandParametersTable);
        
        protected async Task WaitForTargetFrame(int frameNumber)
        {
            while (Context.CurrentEngineFrameReactiveProperty.Value < frameNumber)
                await Task.Delay(2);
        }
    }
}