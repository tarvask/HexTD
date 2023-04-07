using System;
using JetBrains.Annotations;

namespace Tools.Disposing
{
	public class DelegateDisposable : IDisposable
	{
		private Action _action;

		public DelegateDisposable([NotNull] Action action)
		{
			_action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public void Dispose()
		{
			if (_action == null)
				return;
			_action();
			_action = null;
		}
	}
}