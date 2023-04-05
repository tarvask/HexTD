using UnityEngine.EventSystems;
using Zenject;

namespace UI.UIElement
{
	public class Dialog : RootUIElement
	{
		protected virtual bool SuppressStatistics => false;
		private EventSystem eventSystem;

		[Inject]
		private void Construct(EventSystem eventSystem) => this.eventSystem = eventSystem;

		protected override void OnAppearing()
		{
			base.OnAppearing();
			eventSystem.enabled = false;
			if (!SuppressStatistics)
			{
				var windowType = GetType().Name;
//                GameflowAnalytics.SendWindowOpenEvent(windowType);
			}
		}

		protected override void OnAppeared()
		{
			eventSystem.enabled = true;
			base.OnAppeared();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			eventSystem.enabled = false;
			if (!SuppressStatistics)
			{
				var windowType = GetType().Name;
//                GameflowAnalytics.SendWindowCloseEvent(windowType);
			}
		}

		protected override void OnDisappeared()
		{
			eventSystem.enabled = true;
			Destroy(gameObject);
			base.OnDisappeared();
		}
	}
}