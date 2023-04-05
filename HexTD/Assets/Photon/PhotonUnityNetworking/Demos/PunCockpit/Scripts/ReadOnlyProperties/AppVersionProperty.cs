// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppVersionProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.PhotonUnityNetworking.Demos.PunCockpit.Scripts.ReadOnlyProperties
{
    /// <summary>
    /// PhotonNetwork.AppVersion UI property.
    /// </summary>
    public class AppVersionProperty : MonoBehaviour
    {

        public Text Text;

        string _cache;

        void Update()
        {
            if (PhotonNetwork.AppVersion != _cache)
            {
                _cache = PhotonNetwork.AppVersion;
                Text.text = _cache;
            }
        }
    }
}