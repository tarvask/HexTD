using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UI.UIElement;
using UnityEngine;
using Zenject;

namespace UI
{
	[UsedImplicitly]
	public class UIFactory<T> : PlaceholderFactory<T>
		where T : Component
	{
		private UICanvas uiCanvas;

		[Inject]
		public void Construct(UICanvas uiCanvas)
		{
			this.uiCanvas = uiCanvas;
		}

		public override T Create()
		{
			var instance = base.Create();
			if (instance is Widget widget)
			{
				widget.AppearAsync().Forget();
			}

			uiCanvas.Add(instance);
			return instance;
		}
	}

	[UsedImplicitly]
	public class UIFactory<TArg1, T> : PlaceholderFactory<TArg1, T>
		where T : Component
	{
		private UICanvas uiCanvas;

		[Inject]
		public void Construct(UICanvas uiCanvas)
		{
			this.uiCanvas = uiCanvas;
		}

		public override T Create(TArg1 param)
		{
			var instance = base.Create(param);
			if (instance is Widget widget)
			{
				widget.AppearAsync().Forget();
			}

			uiCanvas.Add(instance);
			return instance;
		}
	}

	[UsedImplicitly]
	public class UIFactory<TArg1, TArg2, T> : PlaceholderFactory<TArg1, TArg2, T>
		where T : Component
	{
		private UICanvas uiCanvas;

		[Inject]
		public void Construct(UICanvas uiCanvas)
		{
			this.uiCanvas = uiCanvas;
		}

		public override T Create(TArg1 param1, TArg2 param2)
		{
			var instance = base.Create(param1, param2);
			if (instance is Widget widget)
			{
				widget.AppearAsync().Forget();
			}

			uiCanvas.Add(instance);
			return instance;
		}
	}

	[UsedImplicitly]
	public class UIFactory<TArg1, TArg2, TArg3, T> : PlaceholderFactory<TArg1, TArg2, TArg3, T>
		where T : Component
	{
		private UICanvas uiCanvas;

		[Inject]
		public void Construct(UICanvas uiCanvas)
		{
			this.uiCanvas = uiCanvas;
		}

		public override T Create(TArg1 param1, TArg2 param2, TArg3 param3)
		{
			var instance = base.Create(param1, param2, param3);
			if (instance is Widget widget)
			{
				widget.AppearAsync().Forget();
			}

			uiCanvas.Add(instance);
			return instance;
		}
	}

	[UsedImplicitly]
	public class UIFactory<TArg1, TArg2, TArg3, TArg4, T> : PlaceholderFactory<TArg1, TArg2, TArg3, TArg4, T>
		where T : Component
	{
		private UICanvas uiCanvas;

		[Inject]
		public void Construct(UICanvas uiCanvas)
		{
			this.uiCanvas = uiCanvas;
		}

		public override T Create(TArg1 param1, TArg2 param2, TArg3 param3, TArg4 param4)
		{
			var instance = base.Create(param1, param2, param3, param4);
			if (instance is Widget widget)
			{
				widget.AppearAsync().Forget();
			}

			uiCanvas.Add(instance);
			return instance;
		}
	}
}