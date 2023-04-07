using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WindowSystem.View;

namespace UI.Components
{
	public class WindowViewAnimation : MonoBehaviour, IWindowViewComponent
	{
		[SerializeField] private string appearAnimation = "Default Window Appear";
		[SerializeField] private string disappearAnimation = "Default Window Disappear";
		[SerializeField] private bool useClipTimings;
		[SerializeField] private float appearTime = 0f;
		[SerializeField] private float disappearTime = 0f;
		[SerializeField] private Animator[] viewAnimators;

		private static readonly int Appear = Animator.StringToHash("appear");
		private static readonly int Disappear = Animator.StringToHash("disappear");

		private void OnValidate()
		{
			if (useClipTimings)
				UpdateTimings();
		}

		private void UpdateTimings()
		{
			appearTime = 0;
			disappearTime = 0;

			foreach (var anim in viewAnimators)
			{
				var clips = anim.runtimeAnimatorController.animationClips;
				foreach (var clip in clips)
				{
					if (string.Equals(clip.name, appearAnimation))
						appearTime = Mathf.Max(appearTime, clip.length);

					if (string.Equals(clip.name, disappearAnimation))
						disappearTime = Mathf.Max(disappearTime, clip.length);
				}
			}
		}

		public UniTask AppearAsync(bool animated = true)
		{
			if (!animated)
				return UniTask.CompletedTask;

			foreach (var animator in viewAnimators)
				animator.SetTrigger(Appear);

			return UniTask.Delay(TimeSpan.FromSeconds(appearTime));
		}

		public void Appeared()
		{
		}

		public UniTask DisappearAsync(bool animated = true)
		{
			if (!animated)
				return UniTask.CompletedTask;

			foreach (var animator in viewAnimators)
				animator.SetTrigger(Disappear);

			return UniTask.Delay(TimeSpan.FromSeconds(disappearTime));
		}

		public void Disappeared()
		{
		}
	}
}