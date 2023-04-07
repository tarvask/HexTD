using System;
using System.Collections.Generic;
using System.Linq;
using UI.UIElement;
using WindowSystem.Controller;

namespace WindowSystem
{
	internal static class WindowsStorage
	{
		public static event Action<IWindowController> OnOpenedControllerAdded;
		public static event Action<IWindowController> OnOpenedControllerRemoved;

		private static readonly Dictionary<Type, IWindowController> OpenedWindows;
		private static readonly Dictionary<Type, IList<IWindowController>> OpenedModalWindows;

		static WindowsStorage()
		{
			OpenedWindows = new Dictionary<Type, IWindowController>();
			OpenedModalWindows = new Dictionary<Type, IList<IWindowController>>();
		}

		internal static void AddOpenedController(IWindowController controller)
		{
			OpenedWindows[controller.GetType()] = controller;
			OnOpenedControllerAdded?.Invoke(controller);
		}


		internal static bool HasOpenedController<TWindowController>()
			where TWindowController : class, IWindowController

		{
			return OpenedWindows.TryGetValue(typeof(TWindowController), out var windowController) &&
			       windowController.State != UIElementState.None;
		}

		internal static bool HasOpenedController(IWindowController windowController) =>
			OpenedWindows.ContainsKey(windowController.GetType());

		internal static bool HasOpenedWindowsThatOverlapScreen() =>
			OpenedWindows.Any(controller =>
				controller.Value.CanOverlapScreen && controller.Value.State != UIElementState.Disappearing);

		public static void RemoveOpenedController(IWindowController windowController)
		{
			OnOpenedControllerRemoved?.Invoke(OpenedWindows[windowController.GetType()]);
			OpenedWindows.Remove(windowController.GetType());
		}

		public static void AddOpenedModalController(IWindowController controller)
		{
			var type = controller.GetType();

			if (OpenedModalWindows.ContainsKey(type))
				OpenedModalWindows[type].Add(controller);
			else
				OpenedModalWindows[type] = new List<IWindowController> { controller };
		}

		public static void RemoveOpenedModalController(IWindowController windowController) =>
			OpenedModalWindows[windowController.GetType()].Remove(windowController);

		public static TWindowController GetOpenedController<TWindowController>()
			where TWindowController : class, IWindowController =>
			(TWindowController)OpenedWindows[typeof(TWindowController)];

		public static List<IWindowController> GetAllOpenedControllers()
		{
			return OpenedWindows.Values.ToList();
		}
	}
}