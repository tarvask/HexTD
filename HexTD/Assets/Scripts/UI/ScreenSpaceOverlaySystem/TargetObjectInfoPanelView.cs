using System;
using BuffLogic;
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
			public readonly FloatImpactableBuffableValue Health;
			public readonly Transform Pivot;
			public readonly bool IsShowHealthBarWhenFullHealth;

			public Context(
				FloatImpactableBuffableValue health,
				Transform pivot,
				bool isShowHealthBarWhenFullHealth)
			{
				Health = health;
				Pivot = pivot;
				IsShowHealthBarWhenFullHealth = isShowHealthBarWhenFullHealth;
			}
		}

		public void OnSpawned(Context context, IMemoryPool pool)
		{
			_pool = pool;
			_context = context;

			context.Health.SubscribeOnSetValue(health =>
				healthProgressBar.SetProgress(health / context.Health.Value));

			if (context.IsShowHealthBarWhenFullHealth)
			{
				healthProgressBar.gameObject.SetActive(true);
			}
			else
			{
				context.Health.SubscribeOnSetValue(health =>
					healthProgressBar.gameObject.SetActive(health < context.Health.Value));
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
		}

		public class Factory : PlaceholderFactory<Context, TargetObjectInfoPanelView>
		{
		}
	}
}