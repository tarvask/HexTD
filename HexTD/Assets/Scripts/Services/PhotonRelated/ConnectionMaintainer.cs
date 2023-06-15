using System;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UniRx;
using UnityEngine;

namespace Services.PhotonRelated
{
    public class ConnectionMaintainer : MonoBehaviourPunCallbacks
    {
        private bool _isInited;
        private bool _rejoinCalled;
        private bool _reconnectCalled;
        private bool _inRoom;
        private string _roomName;
        private DisconnectCause _previousDisconnectCause;
        private ReactiveProperty<bool> _isConnectedReactiveProperty;
        private Coroutine _backToMatch;

        public IReadOnlyReactiveProperty<bool> IsConnectedReactiveProperty => _isConnectedReactiveProperty;
        public event Action OnRequestStateEvent;
        public event Action OnRollbackStateEvent;
        public event Action<int> OnRoomReceivedPlayer;
        public event Action<int> OnRoomLostPlayer;

        public void Init()
        {
            _isInited = true;
            _isConnectedReactiveProperty = new ReactiveProperty<bool>(true);
            _roomName = PhotonNetwork.CurrentRoom?.Name;
            PhotonRoomNameSaver.SetLastGameRoomName(_roomName);
        }

        public void Clear()
        {
            _isInited = false;
            _isConnectedReactiveProperty.Dispose();
            _roomName = "";
            PhotonRoomNameSaver.DropLastGameRoomName();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // if (CanRecoverFromDisconnect(cause))
            // {
            //     Recover();
            // }
            Debug.LogError($"Disconnected, because of {cause}");
            StartCoroutine(ConnectBack(cause));
        }

        public override void OnConnected()
        {
            if (!_isInited)
                return;
            
            base.OnConnected();
            Debug.LogError("Connected back");

            if (_backToMatch == null)
                _backToMatch = StartCoroutine(BackToMatch());
        }

        public override void OnConnectedToMaster()
        {
            if (!_isInited)
                return;
            
            base.OnConnectedToMaster();
            Debug.LogError("Connected to master");
            
            if (_reconnectCalled)
            {
                Debug.LogError("Reconnect successful");
                _reconnectCalled = false;
            }
            
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            if (!_isInited)
                return;
            
            base.OnJoinedLobby();
            Debug.LogError("Joined lobby");

            PhotonNetwork.JoinRoom(_roomName);
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (_rejoinCalled)
            {
                Debug.LogErrorFormat("Quick rejoin failed with error code: {0} & error message: {1}", returnCode, message);
                _rejoinCalled = false;
            }
        }

        public override void OnJoinedRoom()
        {
            if (!_isInited)
                return;
            
            base.OnJoinedRoom();
            
            _inRoom = true;
            if (_rejoinCalled)
            {
                Debug.LogError("Rejoin successful");
                _rejoinCalled = false;
            }

            Debug.LogError("Back to match, joined room");
            _isConnectedReactiveProperty.Value = true;
            OnRoomReceivedPlayer?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber);

            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                RequestMatchState();
            else
                RollbackMatchState();
        }

        public override void OnLeftRoom()
        {
            if (!_isInited)
                return;
            
            base.OnLeftRoom();
            Debug.LogError("Left room");
            _inRoom = false;
            
            _isConnectedReactiveProperty.Value = false;
            OnRoomLostPlayer?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (!_isInited)
                return;
            
            base.OnPlayerEnteredRoom(newPlayer);
            
            OnRoomReceivedPlayer?.Invoke(newPlayer.ActorNumber);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!_isInited)
                return;
            
            base.OnPlayerLeftRoom(otherPlayer);

            OnRoomLostPlayer?.Invoke(otherPlayer.ActorNumber);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
        }

        private bool CanRecoverFromDisconnect(DisconnectCause cause)
        {
            switch (cause)
            {
                // the list here may be non exhaustive and is subject to review
                case DisconnectCause.Exception:
                case DisconnectCause.ServerTimeout:
                case DisconnectCause.ClientTimeout:
                case DisconnectCause.DisconnectByServerLogic:
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    return true;
            }
            return false;
        }
        
        private void HandleDisconnect(DisconnectCause cause)
        {
            switch (cause)
            {
                // cases that we can recover from
                case DisconnectCause.ServerTimeout:
                case DisconnectCause.Exception:
                case DisconnectCause.ClientTimeout:
                case DisconnectCause.DisconnectByServerLogic:
                case DisconnectCause.AuthenticationTicketExpired:
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    if (_inRoom)
                    {
                        Debug.LogError("calling PhotonNetwork.ReconnectAndRejoin()");
                        _rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                        
                        if (!_rejoinCalled)
                        {
                            Debug.LogWarning("PhotonNetwork.ReconnectAndRejoin returned false, PhotonNetwork.Reconnect is called instead.");
                            _reconnectCalled = PhotonNetwork.Reconnect();
                        }
                    }
                    else
                    {
                        Debug.LogError("calling PhotonNetwork.Reconnect()");
                        _reconnectCalled = PhotonNetwork.Reconnect();
                    }
                    
                    if (!_rejoinCalled && !_reconnectCalled)
                    {
                        Debug.LogError("PhotonNetwork.ReconnectAndRejoin() or PhotonNetwork.Reconnect() returned false, client stays disconnected.");
                    }
                    break;
                case DisconnectCause.None:
                case DisconnectCause.OperationNotAllowedInCurrentState:
                case DisconnectCause.CustomAuthenticationFailed:
                case DisconnectCause.DisconnectByClientLogic:
                case DisconnectCause.InvalidAuthentication:
                case DisconnectCause.ExceptionOnConnect:
                case DisconnectCause.MaxCcuReached:
                case DisconnectCause.InvalidRegion:
                    Debug.LogErrorFormat("Disconnection we cannot automatically recover from, cause: {0}, report it if you think auto recovery is still possible", cause);
                    break;
            }
        }

        private void Recover()
        {
            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
                if (!PhotonNetwork.Reconnect())
                {
                    Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                    if (!PhotonNetwork.ConnectUsingSettings())
                    {
                        Debug.LogError("ConnectUsingSettings failed");
                    }
                }
            }
        }

        private void Recover2(DisconnectCause cause)
        {
            Debug.LogErrorFormat("OnDisconnected(cause={0}) ClientState={1} PeerState={2}",
                cause,
                PhotonNetwork.NetworkingClient.State,
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);
            if (_rejoinCalled)
            {
                Debug.LogErrorFormat("Rejoin failed, client disconnected, causes; prev.:{0} current:{1}", _previousDisconnectCause, cause);
                _rejoinCalled = false;
            }
            else if (_reconnectCalled)
            {
                Debug.LogErrorFormat("Reconnect failed, client disconnected, causes; prev.:{0} current:{1}", _previousDisconnectCause, cause);
                _reconnectCalled = false;
            }
            
            HandleDisconnect(cause); // add attempts counter? to avoid infinite retries?
            _inRoom = false;
            _previousDisconnectCause = cause;
        }

        private IEnumerator ConnectBack(DisconnectCause cause)
        {
            while (!PhotonNetwork.IsConnected)
            {
                Recover2(cause);
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator BackToMatch()
        {
            while (!PhotonNetwork.IsConnectedAndReady)
            {
                yield return new WaitForSeconds(1f);
            }

            if (!PhotonNetwork.InLobby)
            {
                Debug.LogError("Back to match, joining lobby");
                PhotonNetwork.JoinLobby();
                
                while (!PhotonNetwork.InLobby)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            
            Debug.LogError("Back to match, joining room");
            PhotonNetwork.JoinRoom(_roomName);
            
            _backToMatch = null;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                Debug.Log("Application went to tray, pause state started");
            else
                Debug.Log("Pause ended");
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Debug.Log("Application caught focus");
                
                if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                    RequestMatchState();
            }
            else
            {
                Debug.Log("Application lost focus");
            }
        }

        [ContextMenu("Request Sync")]
        private void RequestMatchState()
        {
            Debug.LogError("Back to match, requesting match state");
            OnRequestStateEvent?.Invoke();
        }

        private void RollbackMatchState()
        {
            Debug.LogError("Back to match, rolling back match state");
            OnRollbackStateEvent?.Invoke();
        }
    }
}