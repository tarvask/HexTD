using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Services.PhotonRelated
{
	public class PhotonConnectionService : PhotonConnectionServiceBase
	{
        /// <summary>Used as PhotonNetwork.GameVersion.</summary>
        private readonly byte _version = 1;

		/// <summary>Max number of players allowed in room. Once full, a new room will be created by the next connection attemping to join.</summary>
		[Tooltip("The max number of players allowed in room. Once full, a new room will be created by the next connection attemping to join.")]
        private readonly byte _maxPlayers = 2;

        private readonly int _playerTTL = -1;

        public bool IsConnectedToRoom { get; private set; }= false;

        public PhotonConnectionService ConnectNow()
        {
            Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");

            
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this._version + "." + SceneManagerHelper.ActiveSceneBuildIndex;

            return this;
        }


        // below, we implement some callbacks of the Photon Realtime API.
        // Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN. This client is now connected to Master Server in region [" + PhotonNetwork.CloudRegion +
                "] and can join a room. Calling: PhotonNetwork.JoinRandomRoom();");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRandomFailed() was called by PUN. No random room available in region [" + PhotonNetwork.CloudRegion + "]," +
                      "so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = {" + _maxPlayers + "}}, null);");

            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = this._maxPlayers };
            if (_playerTTL >= 0)
                roomOptions.PlayerTtl = _playerTTL;
            
            roomOptions.IsOpen = true; // to let others join till its open to join
            roomOptions.IsVisible = true; // to list room created room in lobby for callback function

            PhotonNetwork.CreateRoom("GreenRoom", roomOptions, null);
        }
        
        public override void OnCreatedRoom()
        {
            Debug.Log("OnCreatedRoom()");
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("OnCreateRoomFailed() with code" + returnCode + ", " + message);
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room in region [" + PhotonNetwork.CloudRegion + "]. Game is now running.");

            // Only AutoSpawn if we are a new ActorId. Rejoining should reproduce the objects by server instantiation.
            // additional check for rejoin by room name
            if (PhotonNetwork.LocalPlayer.HasRejoined )
            {
                Debug.Log("Was rejoined");
                return;
            }

            IsConnectedToRoom = true;
        }

        // the following methods are implemented to give you some context. re-implement them as needed.
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected(" + cause + ")");
        }
    }
}