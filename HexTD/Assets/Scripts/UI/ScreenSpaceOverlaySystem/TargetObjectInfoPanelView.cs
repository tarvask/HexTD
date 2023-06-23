using System;
using Tools;
using Tools.Interfaces;
using UI.Tools.SimpleProgressBar;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.ScreenSpaceOverlaySystem
{
	public class TargetObjectInfoPanelView : BaseMonoBehaviour,
		IPoolable<TargetObjectInfoPanelView.Context, IMemoryPool>,
		IDisposable, IOuterViewUpdatable
	{
		[SerializeField] private SimpleProgressBar healthProgressBar;

		private IMemoryPool _pool;
		private Context _context;

		private CompositeDisposable _compositeDisposable;

		private Camera _ourFieldCamera;

		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		[Inject]
		public void Constructor([Inject(Id = "OurFieldCamera")] Camera ourFieldCamera)
		{
			_ourFieldCamera = ourFieldCamera;
		}

		public class Context
		{
			public readonly IReadOnlyReactiveProperty<float> MaxHealth;
			public readonly IReadOnlyReactiveProperty<float> CurrentHealth;
			public readonly Transform Pivot;
			public readonly bool IsShowHealthBarWhenFullHealth;

			public Context(
				IReadOnlyReactiveProperty<float> maxHealth,
				IReadOnlyReactiveProperty<float> currentHealth,
				Transform pivot,
				bool isShowHealthBarWhenFullHealth)
			{
				MaxHealth = maxHealth;
				CurrentHealth = currentHealth;
				Pivot = pivot;
				IsShowHealthBarWhenFullHealth = isShowHealthBarWhenFullHealth;
			}
		}

		public void OnSpawned(Context context, IMemoryPool pool)
		{
			_pool = pool;
			_context = context;

			_compositeDisposable = new CompositeDisposable();

			context.CurrentHealth.Subscribe(health => healthProgressBar.SetProgress(health / context.MaxHealth.Value))
				.AddTo(_compositeDisposable);

			if (context.IsShowHealthBarWhenFullHealth)
			{
				healthProgressBar.gameObject.SetActive(true);
			}
			else
			{
				context.CurrentHealth.Subscribe(health =>
						healthProgressBar.gameObject.SetActive(health < context.MaxHealth.Value))
					.AddTo(_compositeDisposable);
			}
		}

		public void OuterViewUpdate(float frameLength)
		{
			if (_context.Pivot != null)
			{
				SetPosition(_context.Pivot.position);
			}
		}

		private void SetPosition(Vector3 worldPosition)
		{
			Vector2 screenPoint = _ourFieldCamera.WorldToScreenPoint(worldPosition);
			_rectTransform.position = screenPoint;
		}

		public void Dispose()
		{
			_pool.Despawn(this);
		}

		public void OnDespawned()
		{
			_pool = null;
			_context = null;

			_compositeDisposable.Dispose();
			_compositeDisposable = null;
		}

		public class Factory : PlaceholderFactory<Context, TargetObjectInfoPanelView>
		{
		}
	}
}