using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Tools;
using Tools.Disposing;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.UIElement
{
	public abstract class UIElement : MonoBehaviour, IDisposableOwner
	{
		private readonly CompositeDisposable disposables = new CompositeDisposable();

		[SerializeField] private UIElementAnimations animations = default;

//        [SerializeField] private Sound appearSound;
//        [SerializeField] private Sound disappearSound;

//        protected IAudioService audioService;
		private UIElementAnimations defaultAnimations;
		private SimpleAnimation simpleAnimation;

		public UIElementState State { get; private set; }
		public UIElementAnimations Animations => animations;

		protected virtual AnimationClip AppearAnimation =>
			animations != null ? animations.Appear : defaultAnimations.Appear;

		protected virtual AnimationClip DisappearAnimation =>
			animations != null ? animations.Disappear : defaultAnimations.Disappear;

		[Inject]
		private void Construct(
//            IAudioService audioService,
			UIElementAnimations defaultAnimations)
		{
//            this.audioService = audioService;
			this.defaultAnimations = defaultAnimations;
			if (Application.isEditor)
			{
				AssertNoLegacyAnimationEvents();
			}
		}

		private void OnDestroy()
		{
			disposables.Dispose();
		}

		public async UniTask AppearAsync(bool withAnimation = true)
		{
			if (State == UIElementState.Appearing || State == UIElementState.Appeared)
			{
				return;
			}

			if (State == UIElementState.Disappearing)
			{
				await UniTask.WaitUntil(() => State == UIElementState.Disappeared);
			}

			gameObject.SetActive(true);
			OnAppearing();

//            if (appearSound != null) 
//                audioService.PlaySoundEffect(appearSound);

			if (withAnimation) await AppearInternalAsync();
			OnAppeared();
		}

		public async UniTask DisappearAsync(bool withAnimation = true)
		{
			if (State == UIElementState.Disappearing || State == UIElementState.Disappeared)
			{
				return;
			}

			if (State == UIElementState.Appearing)
			{
				await UniTask.WaitUntil(() => State == UIElementState.Appeared);
			}

			OnDisappearing();

//            if (disappearSound != null)
//                audioService.PlaySoundEffect(disappearSound);

			if (withAnimation) await DisappearInternalAsync();
			gameObject.SetActive(false);
			OnDisappeared();
		}

		public UniTask WaitUntilAppearedAsync(CancellationToken ct = default)
		{
			return UniTask.WaitUntil(() => State == UIElementState.Appeared, cancellationToken: ct)
				.SuppressCancellationThrow();
		}

		public UniTask WaitUntilDisappearedAsync(CancellationToken ct = default)
		{
			return UniTask.WaitUntil(() => State == UIElementState.Disappeared, cancellationToken: ct)
				.SuppressCancellationThrow();
		}

		void IDisposableOwner.AddOwnership(IDisposable disposable)
		{
			if (!disposables.Contains(disposable))
				disposables.Add(disposable);
		}

		void IDisposableOwner.RemoveOwnership(IDisposable disposable)
		{
			disposables.Remove(disposable);
		}

		protected virtual void OnAppearing() => State = UIElementState.Appearing;

		protected virtual UniTask AppearInternalAsync() => PlayAnimationAsync(AppearAnimation);

		protected virtual void OnAppeared() => State = UIElementState.Appeared;

		protected virtual void OnDisappearing() => State = UIElementState.Disappearing;

		protected virtual UniTask DisappearInternalAsync() => PlayAnimationAsync(DisappearAnimation);

		protected virtual void OnDisappeared() => State = UIElementState.Disappeared;

		private UniTask PlayAnimationAsync(AnimationClip clip)
		{
			if (clip == null) return UniTask.CompletedTask;

			simpleAnimation = simpleAnimation ? simpleAnimation : gameObject.AddComponent<SimpleAnimation>();
			return simpleAnimation.PlayAsync(clip, true);
		}

		private void AssertNoLegacyAnimationEvents()
		{
			AssertNoLegacyAnimationEvents(AppearAnimation);
			AssertNoLegacyAnimationEvents(DisappearAnimation);
		}

		private void AssertNoLegacyAnimationEvents(AnimationClip animationClip)
		{
			if (animationClip == null) return;
			var legacyEventNames = new List<string>
			{
				nameof(OnAppearing),
				nameof(OnAppeared),
				nameof(OnDisappearing),
				nameof(OnDisappeared)
			};
			var legacyEvent = animationClip.events
				.FirstOrDefault(clipEvent => legacyEventNames.Contains(clipEvent.functionName));
			if (legacyEvent != null)
			{
				throw new Exception(
					$"Animation has legacy event {legacyEvent.functionName}. Game Object \"{gameObject.name}\".");
			}
		}
	}

	public enum UIElementState
	{
		None,

		Disappeared,
		Appearing,
		Appeared,
		Disappearing,
	}
}