using System.Threading.Tasks;
using ExitGames.Client.Photon;

namespace Match.Commands
{
    public interface ICommandExecutor
    {
        Task Request(Hashtable commandParametersTable);
        Task Apply(Hashtable commandParametersTable);
    }
}