using System.Collections.Generic;
using Match.Field.Shooting;
using Tools;
using Tools.Interfaces;
using UnityEngine;

namespace UI.ScreenSpaceOverlaySystem
{
	public class ScreenSpaceOverlayController : BaseDisposable, IOuterViewUpdatable
	{
		private readonly TargetObjectInfoPanelView.Factory targetObjectInfoPanelViewFactory;

		private readonly Dictionary<int, TargetObjectInfoPanelView> _targetObjectInfoPanelViews = new();

		public ScreenSpaceOverlayController(TargetObjectInfoPanelView.Factory targetObjectInfoPanelViewFactory)
		{
			this.targetObjectInfoPanelViewFactory = targetObjectInfoPanelViewFactory;
		}

		public void CreateForTarget(ITarget target, ITargetView targetView, bool isShowHealthBarWhenFullHealth = true)
		{
//			Debug.Log(target.TargetId);
			var newTargetObjectInfoPanelView = targetObjectInfoPanelViewFactory.Create(
				new TargetObjectInfoPanelView.Context(
					target.BaseReactiveModel.MaxHealth,
					target.BaseReactiveModel.Health,
					targetView.InfoPanelPivot,
					isShowHealthBarWhenFullHealth)
			);

			_targetObjectInfoPanelViews.Add(target.TargetId, newTargetObjectInfoPanelView);
		}

		public void OuterViewUpdate(float frameLength)
		{
			foreach (var value in _targetObjectInfoPanelViews.Values)
			{
				value.OuterViewUpdate(frameLength);
			}
		}

		public void RemoveByTarget(ITarget target)
		{
//			Debug.Log($"qwe {target.TargetId}");
			_targetObjectInfoPanelViews[target.TargetId].Dispose();
			_targetObjectInfoPanelViews.Remove(target.TargetId);
		}
	}
}