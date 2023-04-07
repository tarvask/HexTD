using System.Collections.Generic;
using UnityEngine;

namespace UI.UIElement
{
	[CreateAssetMenu(fileName = "UIElementAnimations", menuName = "UIElementAnimations")]
	public class UIElementAnimations : ScriptableObject
	{
		[SerializeField] private AnimationClip appear = default;
		[SerializeField] private AnimationClip disappear = default;

		public AnimationClip Appear => appear;
		public AnimationClip Disappear => disappear;

		public List<AnimationClip> AllAnimations()
		{
			var animations = new List<AnimationClip>();

			if (appear != default)
			{
				animations.Add(appear);
			}

			if (disappear != default)
			{
				animations.Add(disappear);
			}

			return animations;
		}
	}
}