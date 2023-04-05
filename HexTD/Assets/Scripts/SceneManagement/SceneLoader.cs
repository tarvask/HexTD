using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace SceneManagement
{
	public class SceneLoader : ISceneLoader
	{
		private readonly ZenjectSceneLoader zenjectSceneLoader;

		public SceneLoader(ZenjectSceneLoader zenjectSceneLoader) =>
			this.zenjectSceneLoader = zenjectSceneLoader;

		void ISceneLoader.LoadScene(string sceneName) =>
			zenjectSceneLoader.LoadScene(sceneName);

		void ISceneLoader.LoadScene(string sceneName, LoadSceneMode loadSceneMode) =>
			zenjectSceneLoader.LoadScene(sceneName, loadSceneMode);

		void ISceneLoader.LoadScene(string sceneName, LoadSceneMode loadSceneMode, Action<DiContainer> extras) =>
			zenjectSceneLoader.LoadScene(sceneName, loadSceneMode, extras);

		async UniTask ISceneLoader.LoadSceneAsync(string sceneName)
		{
			await zenjectSceneLoader.LoadSceneAsync(sceneName);
			await InternalWaitSceneLoading(sceneName);
		}

		async UniTask ISceneLoader.LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
		{
			await zenjectSceneLoader.LoadSceneAsync(sceneName, loadSceneMode);
			await InternalWaitSceneLoading(sceneName);
		}

		async UniTask ISceneLoader.LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode,
			Action<DiContainer> extras)
		{
			await zenjectSceneLoader.LoadSceneAsync(sceneName, loadSceneMode, extras);
			await InternalWaitSceneLoading(sceneName);
		}

		UniTask ISceneLoader.UnloadSceneAsync(string sceneName) =>
			SceneManager.UnloadSceneAsync(sceneName).ToUniTask();

		bool ISceneLoader.TryChangeSceneActive(string sceneName)
		{
			var targetScene = SceneManager.GetSceneByName(sceneName);
			var currentScene = SceneManager.GetActiveScene();
			if (string.Equals(targetScene.name, currentScene.name)) return false;
			SceneManager.SetActiveScene(targetScene);
			return true;
		}

		private static async UniTask InternalWaitSceneLoading(string sceneName)
		{
			var scene = SceneManager.GetSceneByName(sceneName);
			await UniTask.WaitUntil(() => scene.isLoaded);
		}
	}
}