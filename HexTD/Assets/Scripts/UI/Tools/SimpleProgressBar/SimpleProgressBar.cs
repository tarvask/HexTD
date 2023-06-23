using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tools.SimpleProgressBar
{
	public class SimpleProgressBar : BaseMonoBehaviour
	{
		[SerializeField] private Image progressImage;
		
		private RectTransform _progressRectTransform;
		
		private void Awake()
		{
			_progressRectTransform = progressImage.GetComponent<RectTransform>();
		}

		public void SetProgress(float progress)
		{
			_progressRectTransform.anchorMax = new Vector2(progress, 1.0f);
		}
	}
}