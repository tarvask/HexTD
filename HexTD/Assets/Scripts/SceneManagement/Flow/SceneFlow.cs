using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace SceneManagement.Flow
{
	public abstract class SceneFlow : ScriptableObject
	{
		[SerializeField, Range(1, 100)] private int flowWeight = 1;
		[SerializeField] private bool transitionScene = false;

		protected ISceneLoader sceneLoader;

		public float Weight => flowWeight;
		public bool TransitionScene => transitionScene;

		[Inject]
		private void Construct(ISceneLoader sceneLoader)
		{
			this.sceneLoader = sceneLoader;
		}

		public abstract UniTask RunSceneFlow();
		public abstract UniTask StopSceneFlow();
	}
}