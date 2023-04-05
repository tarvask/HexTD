using System;
using UI.UIElement;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Tools
{
	[Obsolete]
	public class UIToolsInstaller : MonoInstaller
	{
		[SerializeField] private EventSystem eventSystem;
		[SerializeField] private Canvas uiCanvas;

		[Header("Animations")] [SerializeField]
		private UIElementAnimations widgetAnimations;

		[SerializeField] private UIElementAnimations panelAnimations;
		[SerializeField] private UIElementAnimations dialogAnimations;

		public override void InstallBindings()
		{
			Container.Bind<UICanvas>().AsSingle().WithArguments(uiCanvas);
			Container.Bind<EventSystem>().FromComponentInNewPrefab(eventSystem).AsSingle().NonLazy();

			//Animations
			Container.Bind<UIElementAnimations>().FromInstance(widgetAnimations).WhenInjectedInto<Widget>();
			Container.Bind<UIElementAnimations>().FromInstance(panelAnimations).WhenInjectedInto<Panel>();
			Container.Bind<UIElementAnimations>().FromInstance(dialogAnimations).WhenInjectedInto<Dialog>();
		}
	}
}