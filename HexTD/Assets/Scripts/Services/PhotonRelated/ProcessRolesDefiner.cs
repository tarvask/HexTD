using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Tools;

namespace Services.PhotonRelated
{
    public class ProcessRolesDefiner : BaseDisposable
    {
        public struct Context
        {
            public bool IsMultiPlayerGame { get; }
            public bool HasAuthorityServer { get; }

            public Context(bool isMultiPlayerGame, bool hasAuthorityServer)
            {
                IsMultiPlayerGame = isMultiPlayerGame;
                HasAuthorityServer = hasAuthorityServer;
            }
        }

        private readonly Context _context;

        public ProcessRolesDefiner(Context context)
        {
            _context = context;
        }
        
        public Dictionary<ProcessRoles, int> GetRolesAndUsers()
        {
            if (_context.IsMultiPlayerGame)
                return GetRolesAndUsersMultiPlayer();
            else
                return GetRolesAndUsersSinglePlayer();
        }

        private Dictionary<ProcessRoles, int> GetRolesAndUsersMultiPlayer()
        {
            int serverId = PhotonNetwork.CurrentRoom.MasterClientId;
            int player1 = -1;
            int player2 = -1;

            if (_context.HasAuthorityServer)
            {
                foreach (var playerPair in PhotonNetwork.CurrentRoom.Players)
                {
                    if (playerPair.Key != serverId)
                    {
                        if (playerPair.Key != player1 && player1 < 0)
                            player1 = playerPair.Key;
                        else if (playerPair.Key != player2 && player2 < 0)
                            player2 = playerPair.Key;
                    }
                }
            }
            else
            {
                foreach (var playerPair in PhotonNetwork.CurrentRoom.Players)
                {
                    if (playerPair.Key != serverId)
                        player2 = playerPair.Key;
                    else
                        player1 = playerPair.Key;
                }
            }

            return new Dictionary<ProcessRoles, int>
                {{ProcessRoles.Server, serverId}, {ProcessRoles.Player1, player1}, {ProcessRoles.Player2, player2}};
        }

        private Dictionary<ProcessRoles, int> GetRolesAndUsersSinglePlayer()
        {
            int serverId;
            int player1;
            int player2;

            if (_context.HasAuthorityServer)
            {
                serverId = 0;
                player1 = 1;
                player2 = 2;
            }
            else
            {
                serverId = 0;
                player1 = 0;
                player2 = 1;
            }

            return new Dictionary<ProcessRoles, int>
                {{ProcessRoles.Server, serverId}, {ProcessRoles.Player1, player1}, {ProcessRoles.Player2, player2}};
        }

        public ProcessRoles GetCurrentProcessGameRole(Dictionary<ProcessRoles, int> rolesAndUserIds)
        {
            if (!_context.IsMultiPlayerGame)
                return ProcessRoles.Player1;
            
            ProcessRoles currentProcessRole = ProcessRoles.Player1;
            int currentUserId = PhotonNetwork.LocalPlayer.ActorNumber;

            foreach (var rolePair in rolesAndUserIds)
            {
                if (rolePair.Key == ProcessRoles.Server)
                    continue;
                
                if (rolePair.Value == currentUserId)
                {
                    currentProcessRole = rolePair.Key;
                    break;
                }
            }

            return currentProcessRole;
        }
        
        public NetworkRoles GetCurrentProcessNetworkRole(Dictionary<ProcessRoles, int> rolesAndUserIds)
        {
            if (!_context.IsMultiPlayerGame)
                return NetworkRoles.Server;
            
            NetworkRoles currentProcessRole = NetworkRoles.Client;
            int currentUserId = PhotonNetwork.LocalPlayer.ActorNumber;

            foreach (var rolePair in rolesAndUserIds)
            {
                if (rolePair.Value == currentUserId && rolePair.Key == ProcessRoles.Server)
                {
                    currentProcessRole = NetworkRoles.Server;
                    break;
                }
            }

            return currentProcessRole;
        }
    }
}