using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UI.UIElement;
using UnityEngine;

namespace WindowSystem.View
{
	public abstract class WindowViewBase : MonoBehaviour
	{
		private readonly List<IWindowViewComponent> _additionalComponents = new List<IWindowViewComponent>();
		private UniTask[] _viewTasks = Array.Empty<UniTask>();

		public UIElementState State { get; protected set; }

		protected void Awake()
		{
			GetComponents(_additionalComponents);
			_viewTasks = new UniTask[_additionalComponents.Count];
			DoAwake();
		}

		protected virtual void DoAwake()
		{
		}

		public virtual UniTask AppearAsync(bool animated = true)
		{
			Appearing(animated);
			return UniTask.WhenAll(_viewTasks).ContinueWith(DidAppeared);
		}

		public virtual UniTask DisappearAsync(bool animated = true)
		{
			Disappearing(animated);
			return UniTask.WhenAll(_viewTasks).ContinueWith(DidDisappeared);
		}

		private void Appearing(bool animated)
		{
			gameObject.SetActive(true);
			State = UIElementState.Appearing;

			for (var i = 0; i < _viewTasks.Length; i++)
				_viewTasks[i] = _additionalComponents[i].AppearAsync(animated);
		}

		private void Disappearing(bool animated)
		{
			State = UIElementState.Disappearing;

			for (var i = 0; i < _viewTasks.Length; i++)
				_viewTasks[i] = _additionalComponents[i].DisappearAsync(animated);
		}

		private void DidAppeared()
		{
			State = UIElementState.Appeared;

			for (var i = 0; i < _viewTasks.Length; i++)
				_additionalComponents[i].Appeared();
		}

		private void DidDisappeared()
		{
			gameObject.SetActive(false);
			State = UIElementState.Disappeared;

			for (var i = 0; i < _viewTasks.Length; i++)
				_additionalComponents[i].Disappeared();
		}
	}
}