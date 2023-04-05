using System;
using JetBrains.Annotations;

namespace Tools.Disposing
{
	public class DelegateDisposable : IDisposable
	{
		private Action action;

		public DelegateDisposable([NotNull] Action action)
		{
			this.action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public void Dispose()
		{
			if (action == null)
				return;
			action();
			action = null;
		}
	}
}