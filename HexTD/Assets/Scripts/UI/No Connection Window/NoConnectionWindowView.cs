using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.No_Connection_Window
{
	public class NoConnectionWindowView : WindowViewBase
	{
		[SerializeField] private Button reconnectButton;
		[SerializeField] private Image backgroundImage;

		public IObservable<Unit> CloseClick => reconnectButton
			.OnClickAsObservable()
			.Merge(backgroundImage
				.OnPointerClickAsObservable()
				.AsUnitObservable())
			.WhereAppeared(this);
	}
}