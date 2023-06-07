using System.Collections.Generic;
using Match.Field.Shooting;
using Tools;
using Tools.Interfaces;
using UnityEngine;

namespace UI.ScreenSpaceOverlaySystem
{
	public class ScreenSpaceOverlayController : BaseDisposable, IOuterViewUpdatable
	{
		private readonly TargetObjectInfoPanelView.Factory _targetObjectInfoPanelViewFactory;

		private readonly Dictionary<Transform, TargetObjectInfoPanelView> _targetObjectInfoPanelViews = new();

		public ScreenSpaceOverlayController(TargetObjectInfoPanelView.Factory targetObjectInfoPanelViewFactory)
		{
			_targetObjectInfoPanelViewFactory = targetObjectInfoPanelViewFactory;
		}

		public void CreateForTarget(ITarget target, ITargetView targetView, bool isShowHealthBarWhenFullHealth = true)
		{
			var newTargetObjectInfoPanelView = _targetObjectInfoPanelViewFactory.Create(
				new TargetObjectInfoPanelView.Context(
					target.BaseReactiveModel.MaxHealth,
					target.BaseReactiveModel.Health,
					targetView.InfoPanelPivot,
					isShowHealthBarWhenFullHealth)
			);

			_targetObjectInfoPanelViews.Add(targetView.InfoPanelPivot, newTargetObjectInfoPanelView);
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
			Transform targetViewTransform = target.TargetView.InfoPanelPivot;

			if (!_targetObjectInfoPanelViews.ContainsKey(targetViewTransform))
				return;
			
			_targetObjectInfoPanelViews[targetViewTransform].Dispose();
			_targetObjectInfoPanelViews.Remove(targetViewTransform);
		}
	}
}