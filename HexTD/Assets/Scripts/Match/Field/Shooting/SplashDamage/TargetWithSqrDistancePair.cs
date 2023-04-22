namespace Match.Field.Shooting.SplashDamage
{
    public struct TargetWithSqrDistancePair
    {
        public ITarget Target { get; }
        public float SqrDistance { get; }

        public TargetWithSqrDistancePair(ITarget target, float sqrDistance)
        {
            Target = target;
            SqrDistance = sqrDistance;
        }
    }
}