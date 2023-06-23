namespace Tools
{
    using Newtonsoft.Json.Utilities;
    using UnityEngine;
 
    public class AotTypeEnforcer : MonoBehaviour
    {
        public void Awake()
        {
            AotHelper.EnsureList<int>();
        }
    }
}