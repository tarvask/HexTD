using System;
using Cysharp.Threading.Tasks;
using UI.UIElement;
using Zenject;

namespace WindowSystem.Controller
{
	public interface IWindowController : IInitializable, IDisposable
	{
		UIElementState State { get; }
		bool CanOverlapScreen { get; }
		UniTask ShowWindowAsync(bool animated);
		UniTask HideWindowAsync(bool animated);
	}
}