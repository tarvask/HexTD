using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.VFX
{
    public class TargetVfxObjectPool : BaseVfxObjectPool<ITarget>
    {
        public TargetVfxObjectPool(Transform vfxObjectTransformPrefab, Transform parentTransform = null) : base(vfxObjectTransformPrefab, parentTransform)
        {
        }

        public override void OuterLogicUpdate(float frameLength)
        {
            foreach (var vfxControllerKeyPair in ActiveVfxControllers)
            {
                vfxControllerKeyPair.Value.SetPosition(vfxControllerKeyPair.Key.Position);
            }
        }
    }
}