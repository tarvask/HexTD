using System;
using Cysharp.Threading.Tasks;
using Tools.Disposing;
using UI.UIElement;
using UniRx;
using UnityEngine;
using WindowSystem.View;
using Zenject;

namespace WindowSystem.Controller
{
	public abstract class WindowControllerBase<TWindowView> : IWindowController, IDisposableOwner
		where TWindowView : WindowViewBase
	{
		private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
		private IInstantiator instantiator;

		public UIElementState State => View.State;
		public virtual bool CanOverlapScreen => false;
		protected TWindowView View { get; private set; }
		protected IWindowsManager WindowsManager { get; private set; }

		[Inject]
		private void InjectDependencies(
			IInstantiator instantiator,
			IWindowsManager windowsManager)
		{
			this.instantiator = instantiator;
			WindowsManager = windowsManager;
		}

		public void Initialize() => DoInitialize();
		public void AddOwnership(IDisposable disposable) => compositeDisposable.Add(disposable);
		public void RemoveOwnership(IDisposable disposable) => compositeDisposable.Remove(disposable);

		protected virtual void DoInitialize()
		{
		}

		protected virtual void DoDispose()
		{
		}

		protected virtual UniTask WillAppearAsync(bool animated = true) => UniTask.CompletedTask;
		protected virtual UniTask DoShowAsync(bool animated = true) => UniTask.CompletedTask;
		protected virtual UniTask DoHideAsync(bool animated = true) => UniTask.CompletedTask;

		protected virtual void OnViewDidShown()
		{
		}

		protected virtual void OnViewDidHidden()
		{
		}

		protected void SetView(TWindowView view) => View = view;

		protected TController CreateSubController<TController>(params object[] args)
			where TController : class, IInitializable, IDisposable
		{
			var controller = instantiator.Instantiate<TController>(args);
			AddOwnership(controller);
			return controller;
		}

		async UniTask IWindowController.ShowWindowAsync(bool animated)
		{
			await WillAppearAsync(animated);
			if (View == null)
			{
//                Log.Warn(LogTag.UI, $"View for has been destroyed", this);
				Debug.LogError($"View for has been destroyed");
				return;
			}

			await UniTask.WhenAll(View.AppearAsync(animated), DoShowAsync(animated))
				.ContinueWith(OnViewDidShown);
		}

		UniTask IWindowController.HideWindowAsync(bool animated) =>
			UniTask.WhenAll(View.DisappearAsync(animated), DoHideAsync(animated))
				.ContinueWith(OnViewDidHidden);

		void IDisposable.Dispose()
		{
			compositeDisposable.Dispose();
			DoDispose();
		}
	}
}