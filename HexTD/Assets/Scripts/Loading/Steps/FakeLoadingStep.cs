using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Loading.Steps
{
	[CreateAssetMenu(menuName = "Game/Loading/FakeLoadingStep")]
	public class FakeLoadingStep : GameLoadingStep
	{
		public override int StepWeight => 1;

		[Inject]
		private void Construct()
		{
		}

		public override UniTask LoadStep() => UniTask.Delay(500);
	}
}