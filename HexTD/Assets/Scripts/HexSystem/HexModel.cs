using System.Collections.Generic;
using Configs.Constants;
using ExitGames.Client.Photon;
using Newtonsoft.Json;

namespace HexSystem
{
    public class HexModel
    {
        [JsonProperty("Position")] public Hex2d Position;
        [JsonProperty("Height")] public int Height;
        [JsonProperty("Data")] public Dictionary<string, string> Data;

        [JsonIgnore] public int Q => Position.Q;
        [JsonIgnore] public int R => Position.R;
        [JsonIgnore] public string HexType => Data[HexParamsNameConstants.HexTypeParam];

        [JsonConstructor]
        public HexModel([JsonProperty("Position")] Hex2d position, 
            [JsonProperty("Height")] int height, 
            [JsonProperty("Data")] Dictionary<string, string> parameters)
        {
            Position = position;
            Height = height;
            Data = new Dictionary<string, string>(parameters);
        }

        public HexModel(Hex2d position, int height, List<(string, string)> parameters)
        {
            Position = position;
            Height = height;
            Data = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                Data.Add(parameter.Item1, parameter.Item2);
            }
        }

        public HexModel(Hex2d position, int height)
        {
            Position = position;
            Height = height;
            Data = new Dictionary<string, string>();
        }

        public HexModel(HexModel hexModel)
        {
            Position = hexModel.Position;
            Height = hexModel.Height;
            Data = new Dictionary<string, string>(hexModel.Data);
        }

        public static explicit operator Hex2d(HexModel hexModel) =>
            new Hex2d(hexModel.Position.Q, hexModel.Position.R);

        public static explicit operator Hex3d(HexModel hexModel) =>
            new Hex3d(hexModel.Position.Q, hexModel.Position.R, hexModel.Height);

        public Hashtable ToNetwork()
        {
            Hashtable hexNetwork = new Hashtable{
                {PhotonEventsConstants.SyncMatch.HexStateParam.Q, Position.Q},
                {PhotonEventsConstants.SyncMatch.HexStateParam.R, Position.R},
                {PhotonEventsConstants.SyncMatch.HexStateParam.H, Height},
                {PhotonEventsConstants.SyncMatch.HexStateParam.DataLength, (byte)Data.Count}
            };

            int i = 0;
            foreach (var hexProperty in Data)
            {
                hexNetwork.Add($"{PhotonEventsConstants.SyncMatch.HexStateParam.DataKey}{i}", 
                    hexProperty.Key);
                hexNetwork.Add($"{PhotonEventsConstants.SyncMatch.HexStateParam.DataValue}{i}", 
                    hexProperty.Value);
                i++;
            }

            return hexNetwork;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}