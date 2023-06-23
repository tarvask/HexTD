using System.Collections;
using System.Collections.Generic;
using Configs.Constants;
using HexSystem;
using Newtonsoft.Json;

namespace MapEditor
{
    public class PropsModel
    {
        [JsonProperty("Position")] public readonly Hex2d Position;
        [JsonProperty("Height")] public int Height;
        [JsonProperty("Data")] public Dictionary<string, string> Data;

        [JsonIgnore] public int Q => Position.Q;
        [JsonIgnore] public int R => Position.R;
        [JsonIgnore] public string HexType => Data[PropsParamsNameConstants.Type];

        [JsonConstructor]
        public PropsModel([JsonProperty("Position")] Hex2d position,
            [JsonProperty("Height")] int height,
            [JsonProperty("Data")] Dictionary<string, string> parameters)
        {
            Position = position;
            Height = height;
            Data = new Dictionary<string, string>(parameters);
        }

        public PropsModel(Hex2d position, int height, List<(string, string)> parameters)
        {
            Position = position;
            Height = height;
            Data = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                Data.Add(parameter.Item1, parameter.Item2);
            }
        }

        public PropsModel(Hex2d position, int height)
        {
            Position = position;
            Height = height;
            Data = new Dictionary<string, string>();
        }

        public PropsModel(PropsModel hexModel)
        {
            Position = hexModel.Position;
            Height = hexModel.Height;
            Data = new Dictionary<string, string>(hexModel.Data);
        }

        public static explicit operator Hex2d(PropsModel propsModel) =>
            new Hex2d(propsModel.Position.Q, propsModel.Position.R);

        public static explicit operator Hex3d(PropsModel propsModel) =>
            new Hex3d(propsModel.Position.Q, propsModel.Position.R, propsModel.Height);

        public Hashtable ToNetwork()
        {
            Hashtable hexNetwork = new Hashtable
            {
                { PhotonEventsConstants.SyncMatch.PropsStateParam.Q, Position.Q },
                { PhotonEventsConstants.SyncMatch.PropsStateParam.R, Position.R },
                { PhotonEventsConstants.SyncMatch.PropsStateParam.H, Height },
                { PhotonEventsConstants.SyncMatch.PropsStateParam.DataLength, (byte)Data.Count }
            };

            int i = 0;
            foreach (var property in Data)
            {
                hexNetwork.Add($"{PhotonEventsConstants.SyncMatch.HexStateParam.DataKey}{i}",
                    property.Key);
                hexNetwork.Add($"{PhotonEventsConstants.SyncMatch.HexStateParam.DataValue}{i}",
                    property.Value);
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
