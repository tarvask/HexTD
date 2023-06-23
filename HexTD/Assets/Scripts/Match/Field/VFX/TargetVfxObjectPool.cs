using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.VFX
{
    public class TargetVfxObjectPool : BaseVfxObjectPool<ITarget>
    {
        public TargetVfxObjectPool(Transform vfxObjectTransformPrefab, Transform parentTransform = null) : base(vfxObjectTransformPrefab, parentTransform)
        {
        }

        protected override void SetPosition(ITarget key, VfxController vfxController)
        {
            vfxController.SetPosition(key.Position);
        }
    }
}