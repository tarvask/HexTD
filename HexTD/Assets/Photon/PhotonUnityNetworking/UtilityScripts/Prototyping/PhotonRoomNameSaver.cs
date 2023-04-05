using UnityEngine;

namespace Photon.PhotonUnityNetworking.UtilityScripts.Prototyping
{
    public static class PhotonRoomNameSaver
    {
        private const string LastGameRoomNameParam = "Room";
        public static string LastGameRoomName { get; private set; }
        public static bool HasCachedGame => !string.IsNullOrEmpty(LastGameRoomName);

        static PhotonRoomNameSaver()
        {
            LastGameRoomName = PlayerPrefs.GetString(LastGameRoomNameParam, "");
        }

        public static void DropLastGameRoomName()
        {
            LastGameRoomName = "";
            PlayerPrefs.SetString(LastGameRoomNameParam, LastGameRoomName);
        }

        public static void SetLastGameRoomName(string roomName)
        {
            LastGameRoomName = roomName;
            PlayerPrefs.SetString(LastGameRoomNameParam, LastGameRoomName);
        }
    }
}