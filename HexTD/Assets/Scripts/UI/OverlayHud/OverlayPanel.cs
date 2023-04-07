using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extensions;
using Tools.Disposing;
using UI.UIElement;
using UniRx;
using UnityEngine;
using WindowSystem;
using Zenject;

namespace UI.OverlayHud
{
	[RequireComponent(typeof(CanvasGroup))]
	public class OverlayPanel : Panel
	{
		private CancellationToken destroyCt;

		private int blockerCount;

		private IWindowsManager windowsManager;
		private CanvasGroup canvasGroup;

		private readonly CompositeDisposable disposable = new CompositeDisposable();

		[Inject]
		private void Construct(
			IWindowsManager windowsManager)
		{
			this.windowsManager = windowsManager;
		}

		protected override bool SuppressReleaseAfterDisappear => true;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			destroyCt = gameObject.GetCancellationTokenOnDestroy();
			DisposableOwnerExtensions.AddTo(windowsManager.IsScreenOverlapped
					.Subscribe(isScreenOverlapped => isScreenOverlapped ? DisappearAsync() : AppearAsync()),
				(IDisposableOwner)this);

			Run();
		}

		private void OnDestroy()
		{
			disposable.Dispose();
		}

		private void Run()
		{
		}

		public void Block()
		{
			blockerCount++;
			canvasGroup.blocksRaycasts = false;
		}

		public void Unblock()
		{
			blockerCount = Math.Max(blockerCount - 1, 0);
			canvasGroup.blocksRaycasts = blockerCount == 0;
		}

		public void ForceUnblock()
		{
			blockerCount = 0;
			canvasGroup.blocksRaycasts = true;
		}

		public IDisposable BlockOverlayScoped()
		{
			Block();
			return new DelegateDisposable(Unblock);
		}

		public IDisposable HideOverlayPanelScoped()
		{
			DisappearAsync().Forget();
			return new DelegateDisposable(() => AppearAsync().Forget());
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Unblock();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Block();
		}

		public class Factory : UIFactory<OverlayPanel>
		{
		}
	}
}