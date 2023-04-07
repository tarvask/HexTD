using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
#endif

namespace Locations.Loading
{
	public class Location : MonoBehaviour
	{
		[SerializeField] private string id;

		private List<Sprite> _loadedSprites = new List<Sprite>();
		private List<SpriteRenderer> _loadedSpriteRenderers = new List<SpriteRenderer>();

		public bool IsLoaded { get; private set; }

		public string Id => id;

		public Transform ParallaxRoot { get; private set; }

		[Inject]
		public void Construct()
		{
		}

		private async void Start()
		{
			await UniTask.DelayFrame(1);

			var (parallaxRoot, parallaxObjects) = await InstantiateAsync();

			IsLoaded = true;
		}

		private void OnDestroy()
		{
			RemoveLoadedSprites();
		}

		private void RemoveLoadedSprites()
		{
			foreach (SpriteRenderer loadedSprite in _loadedSpriteRenderers)
			{
				if (loadedSprite != null)
				{
					Destroy(loadedSprite.gameObject);
				}
			}

			foreach (Sprite loadedSprite in _loadedSprites)
			{
				if (loadedSprite != null)
				{
					Resources.UnloadAsset(loadedSprite.texture);
				}
			}

			_loadedSprites.Clear();
			_loadedSpriteRenderers.Clear();
		}

		private async UniTask<(Transform root, List<GameObject> objects)> InstantiateAsync(
			bool fromAtlas = true)
		{
			var parallaxObjects = new List<GameObject>();

			return (null, parallaxObjects);
		}

		public async UniTaskVoid LoadLocation()
		{
			var (parallaxRoot, parallaxObjects) = await InstantiateAsync(false);
		}
	}
}