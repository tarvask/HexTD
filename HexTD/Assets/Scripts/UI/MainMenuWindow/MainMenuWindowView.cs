using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.MainMenuWindow
{
	public class MainMenuWindowView : WindowViewBase
	{
		//[SerializeField] private Image testImage;
		[SerializeField] private Button singlePlayerBattleRunButton;
		[SerializeField] private Button multiPlayerBattleRunButton;
		[SerializeField] private Button shopButton;
		[SerializeField] private Button inventoryButton;

		public IObservable<Unit> SinglePlayerBattleRunClick => singlePlayerBattleRunButton
			.OnClickAsObservable()
			.WhereAppeared(this);

		public IObservable<Unit> MultiPlayerBattleRunClick => multiPlayerBattleRunButton
			.OnClickAsObservable()
			.WhereAppeared(this);

		public IObservable<Unit> ShopButtonClick => shopButton
			.OnClickAsObservable()
			.WhereAppeared(this);

		public IObservable<Unit> InventoryButtonClick => inventoryButton
			.OnClickAsObservable()
			.WhereAppeared(this);

		//public void TestShowLoaded() => testImage.color = Color.green;
	}
}