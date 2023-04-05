using WindowSystem.Controller;

namespace WindowSystem
{
	public interface IWindowControllerOwner
	{
		void AddOwnership(IWindowController windowController);
		void RemoveOwnership(IWindowController windowController);
	}
}