using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Match;
using Photon.Pun;
using Photon.Realtime;
using Tools;
using UniRx;
using UnityEngine;

namespace Services.PhotonRelated
{
    public class PingDamper : BaseDisposable
    {
        private const int PingDamperFramesMin = TestMatchEngine.LogicFramesPerSecond / 2;
        private const float CheckPingTimeout = 2f;
        private const float PingLerpCoefficient = 0.5f;
        
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

            UpdatePingDamper(maxCachedPing);
        }

        private void UpdatePingDamper(int maxCachedPing)
        {
            int newDesiredPingDamper = Mathf.CeilToInt(maxCachedPing * 0.001f / TestMatchEngine.FrameLength);
            
            if (newDesiredPingDamper < _pingDamperFramesDeltaReactiveProperty.Value)
                _pingDamperFramesDeltaReactiveProperty.Value = Mathf.CeilToInt( 
                    Mathf.Lerp(
                        _pingDamperFramesDeltaReactiveProperty.Value,
                        Mathf.Max(PingDamperFramesMin, newDesiredPingDamper),
                        PingLerpCoefficient));
            else
                _pingDamperFramesDeltaReactiveProperty.Value = newDesiredPingDamper;
        }
    }
}