using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Addressables
{
    public sealed class AddressableParallaxPool
    {
        private const char Separator = '^';
        
        private readonly IDictionary<AssetReference, AsyncOperationHandle<GameObject>> handlerPool;
        private readonly IDictionary<string, Queue<GameObject>> objectPool;

        private readonly DiContainer container;
        private readonly List<IAddressableAssetsReferencesProvider> assetsReferencesProviders;
        private readonly Transform parallaxStorage;

        public AddressableParallaxPool(DiContainer container, List<IAddressableAssetsReferencesProvider> assetsReferencesProviders)
        {
            handlerPool = new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();
            objectPool = new Dictionary<string, Queue<GameObject>>();

            parallaxStorage = container.CreateEmptyGameObject("ParallaxTilePool").transform;
            this.container = container;
            this.assetsReferencesProviders = assetsReferencesProviders;
        }

        public async UniTask InitializeAsync(List<AssetReferenceGameObject> objectsReferences)
        {
            await LoadLayerFromAssetReferences(objectsReferences);
        }

        public async UniTask InitializeMiniEventLayersAsync(List<string> miniEventIDs)
        {
            var objectsReferences = new List<AssetReferenceGameObject>();
            
            foreach (var provider in assetsReferencesProviders)
            {
                objectsReferences.AddRange(provider.GetAssetsReferences(miniEventIDs)); 
            }
            
            await LoadLayerFromAssetReferences(objectsReferences);
        }
        
        public void Reset(bool clearHandlerPool = false)
        {
            foreach (var objects in objectPool.Values)
            {
                while (objects.Count > 0)
                {
                    Object.Destroy(objects.Dequeue());
                }
            }
            objectPool.Clear();

            if (!clearHandlerPool) return;
            
            foreach(var handler in handlerPool)
            {
                UnityEngine.AddressableAssets.Addressables.Release(handler.Value);
            }

            handlerPool.Clear();
        }
        
        public GameObject Spawn(AssetReference reference)
        {
            GameObject resultObject;

            if (objectPool.TryGetValue(reference.ToString(), out var currentQueue) && currentQueue.Count > 0)
            {
                resultObject = currentQueue.Dequeue();
            }
            else
            {
                resultObject = container.InstantiatePrefab(GetParallaxPrefab(reference));
                resultObject.name = resultObject.name + Separator + reference.ToString();
            }

            resultObject.SetActive(false);
            return resultObject;
        }

        public void Despawn(GameObject inputObject)
        {
            var refString = inputObject.name.Split(Separator)[1];
            if (!objectPool.TryGetValue(refString, out var currentObject))
            {
                currentObject = new Queue<GameObject>();
                objectPool.Add(refString, currentObject);
            }

            currentObject.Enqueue(inputObject);
            inputObject.transform.position = new Vector3(0,0,inputObject.transform.position.z);

            inputObject.transform.SetParent(parallaxStorage);
            inputObject.SetActive(false);
        }
        
        private async UniTask LoadLayerFromAssetReferences(List<AssetReferenceGameObject> objectsReferences)
        {
            var tasks = new List<UniTask<GameObject>>();
            foreach (var reference in objectsReferences)
            {
                var handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(reference);
                handler.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                        handlerPool[reference] = op;
                };

                tasks.Add(handler.ToUniTask());
            }
            await UniTask.WhenAll(tasks);
        }

        private GameObject GetParallaxPrefab(AssetReference reference)
        {
            if (!handlerPool.TryGetValue(reference, out var result))
            {
                result = handlerPool.First().Value;
#if UNITY_EDITOR
                Debug.LogError($"Not found Prefab with reference - {reference.editorAsset.name} Load default parallax tile!");
#endif
            }

            return result.Result;
        }
    }
}