using ExitGames.Client.Photon;
using Tools;

namespace Match.EventBus
{
    public abstract class AbstractEventBus : BaseDisposable, IEventBus
    {
        public abstract void RaiseEvent(byte eventCode, object content);

        public abstract void ProcessEvent(byte eventCode, object content, int senderId);

        public abstract void OnEvent(EventData photonEvent);
    }
}