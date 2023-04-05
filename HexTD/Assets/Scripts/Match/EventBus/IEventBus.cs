using Photon.Realtime;

namespace Match.EventBus
{
    public interface IEventBus : IOnEventCallback
    {
        void RaiseEvent(byte eventCode, object content);
        void ProcessEvent(byte eventCode, object content, int senderId);
    }
}