namespace Match.Field.Shooting.SplashDamage
{
    public struct TargetWithSqrDistancePair
    {
        public ITargetable Target { get; }
        public float SqrDistance { get; }

        public TargetWithSqrDistancePair(ITargetable target, float sqrDistance)
        {
            Target = target;
            SqrDistance = sqrDistance;
        }
    }
}