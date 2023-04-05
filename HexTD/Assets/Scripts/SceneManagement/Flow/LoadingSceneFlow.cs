using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement.Flow
{
	[CreateAssetMenu(menuName = "Game/Loading/Flow/Loading Scene Flow", fileName = "Loading Scene Flow")]
	public class LoadingSceneFlow : SceneFlow
	{
		public override UniTask RunSceneFlow() =>
			sceneLoader.LoadSceneAsync(SceneNames.Loading, LoadSceneMode.Additive);

		public override UniTask StopSceneFlow()
		{
			sceneLoader.UnloadSceneAsync(SceneNames.Loading);
			return UniTask.CompletedTask;
		}
	}
}