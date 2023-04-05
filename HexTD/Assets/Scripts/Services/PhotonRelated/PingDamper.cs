using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Match;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using Tools;
using UniRx;
using UnityEngine;

namespace Services.PhotonRelated
{
    public class PingDamper : BaseDisposable
    {
        private const int PingDamperFramesMin = 5;
        private const float CheckPingTimeout = 3f;
        
        private float _currentCheckPingTimeout;
        
        private readonly ReactiveProperty<int> _pingDamperFramesDeltaReactiveProperty;

        public IReadOnlyReactiveProperty<int> PingDamperFramesDeltaReactiveProperty => _pingDamperFramesDeltaReactiveProperty;

        public PingDamper()
        {
            _pingDamperFramesDeltaReactiveProperty = AddDisposable(new ReactiveProperty<int>());
        }
        
        public void UpdatePingTimeout(List<Player> players)
        {
            _currentCheckPingTimeout += Time.deltaTime;

            if (_currentCheckPingTimeout < CheckPingTimeout || players == null)
                return;
            
            _currentCheckPingTimeout = 0;
            UpdatePing(players);
        }

        private void UpdatePing(List<Player> players)
        {
            int maxCachedPing = 0;
            foreach (Player player in players)
            {
                if (player.CustomProperties.TryGetValue("Ping", out int playerPing))
                    maxCachedPing = Mathf.Max(maxCachedPing, playerPing);
            }

            // reduce ping by steps, if needed
            int playerCurrentPing = PhotonNetwork.GetPing();
            int generalPing = playerCurrentPing >= maxCachedPing ?
                playerCurrentPing
                : (playerCurrentPing + maxCachedPing) / 2 + 1;
            Hashtable playerProperties = new Hashtable(){ {"Ping", generalPing}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            
            // update ping damper
            int newPingDamper = Mathf.CeilToInt(maxCachedPing * 0.001f / TestMatchEngine.FrameLength);
            _pingDamperFramesDeltaReactiveProperty.Value = Mathf.Max(PingDamperFramesMin, newPingDamper);
        }
    }
}