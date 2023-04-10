using UnityEngine;
using Zenject;

namespace Tools
{
    public class SrDebugActivator : MonoBehaviour
    {
        [Inject]
        void Construct()
        {
        }

        private void Awake()
        {
#if DEBUG
             SRDebug.Init();
             SRDebug.Instance.IsTriggerEnabled = true;
#endif
            
            Destroy(gameObject);
        }
    }
}