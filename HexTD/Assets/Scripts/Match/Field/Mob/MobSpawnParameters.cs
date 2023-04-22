namespace Match.Field.Mob
{
    public readonly struct MobSpawnParameters
    {
        private readonly MobConfig _mobConfig;
        private readonly byte _pathId;

        public MobConfig MobConfig => _mobConfig;
        public byte PathId => _pathId;

        public MobSpawnParameters(MobConfig mobConfigParam, byte pathIdParam)
        {
            _mobConfig = mobConfigParam;
            _pathId = pathIdParam;
        }
    }
}