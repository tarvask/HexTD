using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Tools;
using UniRx;
using UnityEngine;
using WindowSystem;
using Zenject;
#if UNITY_EDITOR
#endif

namespace MatchStarter
{
	public class MatchStarter : BaseMonoBehaviour
	{
		public IObservable<Unit> OnQuitMatch => _onQuitMatch;
		private Subject<Unit> _onQuitMatch = new Subject<Unit>();
		
		public bool IsLoaded { get; private set; }

		private List<Sprite> _loadedSprites = new List<Sprite>();
		private List<SpriteRenderer> _loadedSpriteRenderers = new List<SpriteRenderer>();

		private IMatchStarterLoader _matchStarterLoader;
//		private IWindowsManager _windowsManager;

		private IDisposable _battleDisposable;
		
		private bool _isDisposed;
			
		[Inject]
		public void Construct(IMatchStarterLoader matchStarterLoader
//			,
//			IWindowsManager windowsManager
			)
		{
			_matchStarterLoader = matchStarterLoader;
//			_windowsManager = windowsManager;
		}

		private void Awake()
		{
			_battleDisposable = FindObjectOfType<PhotonMatchBridge>().OnQuitMatch.Subscribe(_onQuitMatch);
		}

		private async void Start()
		{
			await UniTask.DelayFrame(1);

			var (parallaxRoot, parallaxObjects) = await InstantiateAsync();

			IsLoaded = true;
		}

		protected override void OnDestroy()
		{
			if (!_isDisposed)
				Dispose();
			
			base.OnDestroy();
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

		private void Dispose()
		{
			RemoveLoadedSprites();
			
			_battleDisposable?.Dispose();
			_battleDisposable = null;
		}
	}
}