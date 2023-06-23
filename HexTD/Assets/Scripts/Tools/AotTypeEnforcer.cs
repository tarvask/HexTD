using HexSystem;

namespace Tools
{
    using Newtonsoft.Json.Utilities;
    using UnityEngine;
 
    public class AotTypeEnforcer : MonoBehaviour
    {
        public void Awake()
        {
            AotHelper.EnsureType<Hex2d>();
            AotHelper.EnsureList<Hex2d>();
            AotHelper.EnsureDictionary<Hex2d, int>();
            AotHelper.EnsureDictionary<Hex2d, Hex2d>();
        }
    }
}