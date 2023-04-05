using JetBrains.Annotations;
using UnityEngine;

namespace UI
{
	[UsedImplicitly]
	public class UICanvas
	{
		private readonly Canvas root;

		public UICanvas(Canvas rootPrefab)
		{
			root = Object.Instantiate(rootPrefab);
			Object.DontDestroyOnLoad(root);
		}

		public void Add(Component child)
		{
			child.transform.SetParent(root.transform, false);
		}
	}
}