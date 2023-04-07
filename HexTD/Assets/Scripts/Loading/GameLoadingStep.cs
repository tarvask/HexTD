using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Loading
{
	public abstract class GameLoadingStep : ScriptableObject
	{
		public abstract int StepWeight { get; }
		public abstract UniTask LoadStep();
	}
}