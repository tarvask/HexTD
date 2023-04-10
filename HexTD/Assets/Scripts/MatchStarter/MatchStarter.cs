using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UI.MainMenuWindow;
using UI.No_Connection_Window;
using UnityEngine;
using WindowSystem;
using Zenject;
#if UNITY_EDITOR
#endif

namespace MatchStarter
{
	public class MatchStarter : MonoBehaviour
	{
		public bool IsLoaded { get; private set; }
		
		private List<Sprite> _loadedSprites = new List<Sprite>();
		private List<SpriteRenderer> _loadedSpriteRenderers = new List<SpriteRenderer>();

		private IMatchStarterLoader _matchStarterLoader;
		private IWindowsManager _windowsManager;
			
		[Inject]
		public void Construct(IMatchStarterLoader matchStarterLoader,
			IWindowsManager windowsManager)
		{
			_matchStarterLoader = matchStarterLoader;
			_windowsManager = windowsManager;
		}

		private async void Start()
		{
			await UniTask.DelayFrame(1);

			var (parallaxRoot, parallaxObjects) = await InstantiateAsync();

			IsLoaded = true;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				_windowsManager.OpenAsync<MainMenuWindowController>();
				_matchStarterLoader.DestroyAndRelease();
			}
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