using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Custom
{
	public class LoadingBar : MonoBehaviour
	{
		[SerializeField] private GameObject loaderRoot;
		[SerializeField] private Image loadingBar;
		[SerializeField] private TextMeshProUGUI loadingProgress;

		public float Progress => loadingBar.fillAmount;

		public void SetActive(bool isActive) => loaderRoot.SetActive(isActive);

		public void SetProgress(float progress01)
		{
			loadingBar.fillAmount = progress01;
			loadingProgress.text = $"{Math.Floor(progress01 * 100)}%";
		}
	}
}