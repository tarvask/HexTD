using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Addressables
{
    public class AddressableSpriteLoader : IDisposable
    {
        private readonly Dictionary<string, AsyncOperationHandle<SpriteAtlas>> loadedAtlasHandles = new Dictionary<string, AsyncOperationHandle<SpriteAtlas>>();
        private readonly Dictionary<string, AsyncOperationHandle<Sprite>> loadedSpriteHandles = new Dictionary<string, AsyncOperationHandle<Sprite>>();

        public UniTask<SpriteAtlas> LoadSpriteAtlasAsync(string guid)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
                return UniTask.FromResult(asset);
            }
#endif
            if (!loadedAtlasHandles.TryGetValue(guid, out var currentAtlas))
            {
                currentAtlas = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<SpriteAtlas>(guid);
                loadedAtlasHandles.Add(guid, currentAtlas);
            }
            return currentAtlas.ToUniTask();
        }
        
        public UniTask<Sprite> LoadSpriteAsync(string guid)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            { 
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                return UniTask.FromResult(asset);
            }
#endif
            if (!loadedSpriteHandles.TryGetValue(guid, out var currentSprite))
            {
                currentSprite = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Sprite>(guid);
                loadedSpriteHandles.Add(guid, currentSprite);
            }
            
            return currentSprite.ToUniTask();
        }
        
        public void Dispose()
        {
            foreach (var loadedAtlasHandle in loadedAtlasHandles)
            {
                UnityEngine.AddressableAssets.Addressables.Release(loadedAtlasHandle.Value);
            }
            
            foreach (var loadedSpriteHandle in loadedSpriteHandles)
            {
                UnityEngine.AddressableAssets.Addressables.Release(loadedSpriteHandle.Value);
            }

            loadedAtlasHandles.Clear();
            loadedSpriteHandles.Clear();
        }
    }
}
