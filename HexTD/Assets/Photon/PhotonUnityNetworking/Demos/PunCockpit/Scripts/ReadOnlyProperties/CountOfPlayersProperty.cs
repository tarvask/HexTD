// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountOfPlayersProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine.UI;

namespace Photon.PhotonUnityNetworking.Demos.PunCockpit.Scripts.ReadOnlyProperties
{
    /// <summary>
    /// PhotonNetwork.CountOfPlayers UI property.
    /// </summary>
    public class CountOfPlayersProperty : PropertyListenerBase
    {
        public Text Text;

        int _cache = -1;

        void Update()
        {
            if (PhotonNetwork.NetworkingClient.Server == ServerConnection.MasterServer)
            {
                if (PhotonNetwork.CountOfPlayers != _cache)
                {
                    _cache = PhotonNetwork.CountOfPlayers;
                    Text.text = _cache.ToString();
                    this.OnValueChanged();
                }
            }
            else
            {
                if (_cache != -1)
                {
                    _cache = -1;
                    Text.text = "n/a";
                }
            }
        }
    }
}