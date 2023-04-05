using DG.Tweening;
using UnityEngine;

namespace UI.Custom
{
	public class AnimatedLoadingBar : LoadingBar
	{
		[SerializeField] private float animationTime = 0.5f;
		[SerializeField] private AnimationCurve animationCurve;

		public bool InProgress { get; private set; }

		public void AnimateProgress(float progress01)
		{
			InProgress = true;
			DOTween.Kill(this);
			DOTween.To(() => Progress, SetProgress, progress01, animationTime)
				.SetEase(animationCurve)
				.SetId(this)
				.OnComplete(() => InProgress = false);
		}
	}
}