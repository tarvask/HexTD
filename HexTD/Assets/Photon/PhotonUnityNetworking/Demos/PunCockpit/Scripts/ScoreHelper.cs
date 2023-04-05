// <copyright file="ScoreHelper.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.PhotonUnityNetworking.Code;
using Photon.PhotonUnityNetworking.UtilityScripts.PhotonPlayer;
using UnityEngine;

namespace Photon.PhotonUnityNetworking.Demos.PunCockpit.Scripts
{

    public class ScoreHelper : MonoBehaviour
    {
        public int Score;

        int _currentScore;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {


            if (PhotonNetwork.LocalPlayer != null && Score != _currentScore)
            {
                _currentScore = Score;
                PhotonNetwork.LocalPlayer.SetScore(Score);
            }

        }
    }

}