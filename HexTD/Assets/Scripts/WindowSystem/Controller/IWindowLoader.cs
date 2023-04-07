using Cysharp.Threading.Tasks;

namespace WindowSystem.Controller
{
	public interface IWindowLoader
	{
		UniTask<bool> LoadWindowAsync();
	}
}