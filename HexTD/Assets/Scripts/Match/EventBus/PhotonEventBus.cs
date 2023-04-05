using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Match.EventBus
{
    public class PhotonEventBus : AbstractEventBus
    {
        public struct Context
        {
            public PhotonMatchBridge Bridge { get; }

            public Context(PhotonMatchBridge bridge)
            {
                Bridge = bridge;
            }
        }

        private readonly Context _context;

        public PhotonEventBus(Context context)
        {
            _context = context;
            
            PhotonNetwork.AddCallbackTarget(this);
        }
        
        public override void RaiseEvent(byte eventCode, object content)
        {
            RaiseEventOptions eventOptions = RaiseEventOptions.Default;
            eventOptions.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(eventCode, content, eventOptions, SendOptions.SendReliable);
        }

        public override void ProcessEvent(byte eventCode, object content, int senderId)
        {
            _context.Bridge.ProcessEvent(eventCode, content, senderId);
        }

        public override void OnEvent(EventData photonEvent)
        {
            ProcessEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}