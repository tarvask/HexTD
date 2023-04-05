namespace Match.Field.Shooting.SplashDamage
{
    public struct TargetWithSqrDistancePair
    {
        public int TargetId { get; }
        public float SqrDistance { get; }

        public TargetWithSqrDistancePair(int targetId, float sqrDistance)
        {
            TargetId = targetId;
            SqrDistance = sqrDistance;
        }
    }
}