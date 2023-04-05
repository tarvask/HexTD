using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement.Flow
{
	[CreateAssetMenu(menuName = "Game/Loading/Flow/Game Scene Flow", fileName = "Game Scene Flow")]
	public class GameSceneFlow : SceneFlow
	{
		public override async UniTask RunSceneFlow()
		{
			await sceneLoader.LoadSceneAsync(SceneNames.Game, LoadSceneMode.Additive);
			sceneLoader.TryChangeSceneActive(SceneNames.Game);
		}

		public override UniTask StopSceneFlow() =>
			sceneLoader.UnloadSceneAsync(SceneNames.Game);
	}
}