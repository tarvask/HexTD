using ExitGames.Client.Photon;

namespace Match.EventBus
{
    public class LocalEventBus : AbstractEventBus
    {
        public struct Context
        {
            public PhotonMatchBridge Bridge { get; }
            public int PlayerId { get; }

            public Context(PhotonMatchBridge bridge, int playerId)
            {
                Bridge = bridge;
                PlayerId = playerId;
            }
        }

        private readonly Context _context;

        public LocalEventBus(Context context)
        {
            _context = context;
        }
        
        public override void RaiseEvent(byte eventCode, object content)
        {
            ProcessEvent(eventCode, content, _context.PlayerId);
        }

        public override void ProcessEvent(byte eventCode, object content, int senderId)
        {
            _context.Bridge.ProcessEvent(eventCode, content, senderId);
        }

        public override void OnEvent(EventData photonEvent)
        {
            ProcessEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
        }
    }
}