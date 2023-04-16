using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>Simple component to call ConnectUsingSettings and to get into a PUN room easily.</summary>
    /// <remarks>A custom inspector provides a button to connect in PlayMode, should AutoConnect be false.</remarks>
    public class ConnectAndJoinCachedOrRandom : ConnectAndJoinRandom
    {
        private bool _triedConnectingToCachedRoom;
        // below, we implement some callbacks of the Photon Realtime API.
        // Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN. This client is now connected to Master Server in region [" + PhotonNetwork.CloudRegion +
                "] and can join a room. Calling: ConnectCachedOrRandomRoom();");
            //ConnectCachedOrRandomRoom();
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: ConnectCachedOrRandomRoom();");
            ConnectCachedOrRandomRoom();
        }

        private void ConnectCachedOrRandomRoom()
        {
            string cachedRoom = PhotonRoomNameSaver.LastGameRoomName;

            if (string.IsNullOrEmpty(cachedRoom))
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                _triedConnectingToCachedRoom = true;
                
                if (!PhotonNetwork.JoinRoom(cachedRoom))
                    PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (_triedConnectingToCachedRoom)
            {
                _triedConnectingToCachedRoom = false;
                Debug.LogError(
                    "OnJoinRoomFailed() was called by PUN. Cached room is not available, so we try to join random one. Calling: PhotonNetwork.JoinRandomRoom();");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            _triedConnectingToCachedRoom = false;
        }
    }
}
