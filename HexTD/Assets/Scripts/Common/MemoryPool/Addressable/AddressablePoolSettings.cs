using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common.MemoryPool.Addressable
{
    [CreateAssetMenu(menuName = "Game/Pool/Addressable Pool Settings")]
    public class AddressablePoolSettings : ScriptableObject
    {
        public string poolName;
        [Range(1, 50)] public int maxSize;
        [Range(1, 10)] public int perFrameAllocation = 1;
        public AssetReferenceGameObject reference;
    }
}