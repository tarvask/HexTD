using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace SceneManagement
{
	public interface ISceneLoader
	{
		void LoadScene(string sceneName);
		void LoadScene(string sceneName, LoadSceneMode loadSceneMode);
		void LoadScene(string sceneName, LoadSceneMode loadSceneMode, Action<DiContainer> extras);

		UniTask LoadSceneAsync(string sceneName);
		UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode);
		UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, Action<DiContainer> extras);

		UniTask UnloadSceneAsync(string sceneName);

		bool TryChangeSceneActive(string sceneName);
	}
}