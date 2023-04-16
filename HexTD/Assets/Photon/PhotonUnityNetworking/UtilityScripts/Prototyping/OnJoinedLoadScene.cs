using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Photon.Pun.UtilityScripts
{
    public class OnJoinedLoadScene : MonoBehaviour, IMatchmakingCallbacks
    {
        #region Inspector Items
        [SerializeField] private string sceneName;
        #endregion

        private string _roomName;
        
        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        
        public virtual void OnJoinedRoom()
        {
            // Only AutoSpawn if we are a new ActorId. Rejoining should reproduce the objects by server instantiation.
            // additional check for rejoin by room name
            if (!PhotonNetwork.LocalPlayer.HasRejoined && string.IsNullOrEmpty(_roomName))
            {
                _roomName = PhotonNetwork.CurrentRoom.Name;
                LoadScene();
            }
        }

        public virtual void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public virtual void OnCreatedRoom() { }
        public virtual void OnCreateRoomFailed(short returnCode, string message) { }
        public virtual void OnJoinRoomFailed(short returnCode, string message) { }
        public virtual void OnJoinRandomFailed(short returnCode, string message) { }
        public virtual void OnLeftRoom() { }

        private void LoadScene()
        {
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}