using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.MainMenuWindow
{
	public class MainMenuWindowView: WindowViewBase
	{
		[SerializeField] private Image testImage;
		[SerializeField] private Button battleRunButton;

		public IObservable<Unit> BattleRunClick => battleRunButton
			.OnClickAsObservable()
			.WhereAppeared(this);
		
		public void TestShowLoaded() => testImage.color = Color.green;

	}
}