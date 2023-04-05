using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tools
{
	[RequireComponent(typeof(Animator))]
	public class SimpleAnimation : MonoBehaviour
	{
		private Animator animator;
		private PlayableGraph playableGraph;
		private AnimationPlayableOutput playableOutput;
		private bool initialized;
		private CancellationToken destroyCT;

		private bool isAnimatorEnabled;

		private void Awake()
		{
			destroyCT = this.GetCancellationTokenOnDestroy();
			animator = GetComponent<Animator>();
			isAnimatorEnabled = animator.enabled;
			animator.enabled = false;
		}

		public async UniTask PlayAsync(AnimationClip clip, bool ignoreTimeScale = false)
		{
			Initialize();
			animator.updateMode = ignoreTimeScale ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
			animator.enabled = true;
			playableGraph.SetTimeUpdateMode(ignoreTimeScale
				? DirectorUpdateMode.UnscaledGameTime
				: DirectorUpdateMode.GameTime);
			var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
			playableOutput.SetSourcePlayable(clipPlayable);
			playableGraph.Play();
			await UniTask.WaitUntil(() => clipPlayable.GetTime() > clip.length, cancellationToken: destroyCT)
				.SuppressCancellationThrow();
			if (destroyCT.IsCancellationRequested) return;

			playableGraph.DestroyPlayable(clipPlayable);
			animator.enabled = isAnimatorEnabled;
		}

		private void Initialize()
		{
			if (initialized) return;

			playableGraph = PlayableGraph.Create();
			playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
			initialized = true;
		}
	}
}