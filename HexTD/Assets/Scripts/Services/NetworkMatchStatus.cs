using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Services.PhotonRelated;
using Tools;
using UniRx;
using Debug = UnityEngine.Debug;

namespace Services
{
    public class NetworkMatchStatus : BaseDisposable
    {
        public struct Context
        {
            public ProcessRolesDefiner ProcessRolesDefiner { get; }
            public Dictionary<ProcessRoles, int> RolesAndUserIds { get; }

            public Context(ProcessRolesDefiner processRolesDefiner, Dictionary<ProcessRoles, int> rolesAndUserIds)
            {
                ProcessRolesDefiner = processRolesDefiner;
                RolesAndUserIds = rolesAndUserIds;
            }
        }

        private readonly Context _context;
        
        private Dictionary<ProcessRoles, int> _rolesAndUserIds;
        private Dictionary<int, ProcessRoles> _userIdsAndRoles;
        private readonly ReactiveProperty<ProcessRoles> _currentProcessGameRoleReactiveProperty;
        private readonly ReactiveProperty<NetworkRoles> _currentProcessNetworkRoleReactiveProperty;
        private Dictionary<byte, int> _rolesAndUserIdsNetwork;
        
        public Dictionary<byte, int> RolesAndUserIdsNetwork => _rolesAndUserIdsNetwork;
        public IReadOnlyReactiveProperty<ProcessRoles> CurrentProcessGameRoleReactiveProperty => _currentProcessGameRoleReactiveProperty;
        public IReadOnlyReactiveProperty<NetworkRoles> CurrentProcessNetworkRoleReactiveProperty => _currentProcessNetworkRoleReactiveProperty;

        public NetworkMatchStatus(Context context)
        {
            _context = context;
            
            _rolesAndUserIds = new Dictionary<ProcessRoles, int>(_context.RolesAndUserIds.Count);;
            _userIdsAndRoles = new Dictionary<int, ProcessRoles>(_context.RolesAndUserIds.Count);
            _rolesAndUserIdsNetwork = new Dictionary<byte, int>(_context.RolesAndUserIds.Count);
            
            UpdateUsersAndRoles(_context.RolesAndUserIds);
            ProcessRoles currentProcessRole = _context.ProcessRolesDefiner.GetCurrentProcessGameRole(_context.RolesAndUserIds);
            NetworkRoles currentNetworkRole = _context.ProcessRolesDefiner.GetCurrentProcessNetworkRole(_context.RolesAndUserIds);
            _currentProcessGameRoleReactiveProperty = AddDisposable(new ReactiveProperty<ProcessRoles>(currentProcessRole));
            _currentProcessNetworkRoleReactiveProperty = AddDisposable(new ReactiveProperty<NetworkRoles>(currentNetworkRole));
        }

        public void AddUser(int userId)
        {
            Debug.LogError($"Added user {userId}");
            // 4 cases are possible:
            // 1) current player reconnected to populated room (another player in)
            // 2) current player reconnected to empty room
            // 3) another player reconnected to populated room (current player in)
            // 4) another player reconnected to empty room
            // case #4 is impossible to detect, as current player must be disconnected at that time,
            // so deal with #1-3
            
            // #1
            // room is full and there is no role record for current player
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers
                && !_rolesAndUserIds.ContainsKey(_currentProcessGameRoleReactiveProperty.Value))
            {
                Debug.LogError($"Added current player to full room");
                // clear, because current player was disconnected and old data is irrelevant now
                _rolesAndUserIds.Clear();
                _userIdsAndRoles.Clear();
                // get up-to-date info
                UpdateUsersAndRoles(_context.ProcessRolesDefiner.GetRolesAndUsers());
                
                // add server role for another player, if needed
                /*if (!_rolesAndUserIds.ContainsKey(ProcessRoles.Server))
                {
                    Debug.LogError($"Made another player a master");
                    ProcessRoles otherPlayerRole = (_currentProcessGameRoleReactiveProperty.Value == ProcessRoles.Player1) ?
                        ProcessRoles.Player2 :
                        ProcessRoles.Player1;
                    _rolesAndUserIds.Add(ProcessRoles.Server, _rolesAndUserIds[otherPlayerRole]);
                }*/

                LogRoles();
                return;
            }
            
            // #2
            // first player in room and it can only be detected for current player
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers - 1)
            {
                Debug.LogError($"Added current player to an empty room, made him a master");
                // clear, because room was empty and old data is irrelevant now
                _rolesAndUserIds.Clear();
                _userIdsAndRoles.Clear();
                // get up-to-date info
                UpdateUsersAndRoles(_context.ProcessRolesDefiner.GetRolesAndUsers());
                
                // become server
                //_rolesAndUserIds.Add(ProcessRoles.Server, userId);
                _currentProcessNetworkRoleReactiveProperty.Value = NetworkRoles.Server;
                LogRoles();
                return;
            }

            // #3
            // room is full and current player is already recorded
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers
                && _rolesAndUserIds.ContainsKey(_currentProcessGameRoleReactiveProperty.Value))
            {
                Debug.LogError($"Added another player to a full room");
                ProcessRoles otherPlayerRole = (_currentProcessGameRoleReactiveProperty.Value == ProcessRoles.Player1) ?
                    ProcessRoles.Player2 :
                    ProcessRoles.Player1;
                // remove old data for another player role if needed
                if (_rolesAndUserIds.TryGetValue(otherPlayerRole, out int anotherPlayerOldId))
                {
                    _rolesAndUserIds.Remove(otherPlayerRole);
                    _userIdsAndRoles.Remove(anotherPlayerOldId);
                }
                
                // add role for other player
                _rolesAndUserIds.Add(otherPlayerRole, userId);
                _userIdsAndRoles.Add(userId, otherPlayerRole);
                LogRoles();
                return;
            }
        }

        public void RemoveUser(int userId)
        {
            Debug.LogError($"Removed user {userId}");
            
            if (!_userIdsAndRoles.ContainsKey(userId) && userId != -1)
            {
                Debug.LogError($"Unknown user {userId}");
                return;
            }
            // 2 cases are possible:
            // 1) current player disconnected from full room
            // 2) current player disconnected from empty room
            // 3) another player disconnected from full room
            // 4) another player disconnected from empty room
            // case #4 is impossible to detect, as current player must be disconnected at that time,
            // so deal with #1-3
            
            // #1, 2
            if (userId == -1)
            {
                // maybe just clear everything?
                
                int selfId = _rolesAndUserIds[_currentProcessGameRoleReactiveProperty.Value];
                _userIdsAndRoles.Remove(selfId);
                _rolesAndUserIds.Remove(_currentProcessGameRoleReactiveProperty.Value);
            
                // stop being server on disconnect
                if (_rolesAndUserIds.ContainsKey(ProcessRoles.Server) && _rolesAndUserIds[ProcessRoles.Server] == selfId)
                {
                    _rolesAndUserIds.Remove(ProcessRoles.Server);
                    _currentProcessNetworkRoleReactiveProperty.Value = NetworkRoles.Client;
                }
                
                LogRoles();
                return;
            }

            // #3
            if (_userIdsAndRoles[userId] != _currentProcessGameRoleReactiveProperty.Value)
            {
                ProcessRoles otherPlayerRole = _userIdsAndRoles[userId];
                _userIdsAndRoles.Remove(userId);
                _rolesAndUserIds.Remove(otherPlayerRole);

                // change server
                if (_rolesAndUserIds.ContainsKey(ProcessRoles.Server) &&
                    _rolesAndUserIds[ProcessRoles.Server] == userId)
                {
                    _rolesAndUserIds.Remove(ProcessRoles.Server);

                    // become server
                    _rolesAndUserIds.Add(ProcessRoles.Server, PhotonNetwork.LocalPlayer.ActorNumber);
                    _currentProcessNetworkRoleReactiveProperty.Value = NetworkRoles.Server;
                    PhotonNetwork.CurrentRoom.SetMasterClient(PhotonNetwork.LocalPlayer);
                }

                LogRoles();
            }
        }

        private void UpdateUsersAndRoles(Dictionary<ProcessRoles, int> rolesAndUserIds)
        {
            _rolesAndUserIds.Clear();
            _userIdsAndRoles.Clear();
            _rolesAndUserIdsNetwork.Clear();

            foreach (var roleUserPair in rolesAndUserIds)
            {
                // skip empty record
                if (roleUserPair.Value == -1)
                    continue;
                
                _rolesAndUserIds.Add(roleUserPair.Key, roleUserPair.Value);
                
                if (roleUserPair.Key != (byte)ProcessRoles.Server)
                    _userIdsAndRoles.Add(roleUserPair.Value, roleUserPair.Key);
                
                _rolesAndUserIdsNetwork.Add((byte)roleUserPair.Key, roleUserPair.Value);
            }
        }

        private void LogRoles()
        {
            foreach (var roleUserIdPair in _rolesAndUserIds)
            {
                Debug.LogError($"Role is {roleUserIdPair.Key}, user is {roleUserIdPair.Value}");
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _rolesAndUserIds.Clear();
            _userIdsAndRoles.Clear();
            _rolesAndUserIdsNetwork.Clear();
        }
    }
}