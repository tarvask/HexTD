namespace Match.Field.Mob
{
    public struct MobWithPath
    {
        private readonly byte _mobId;
        private readonly byte _pathId;
        
        public byte MobId => _mobId;
        public byte PathId => _pathId;

        public MobWithPath(byte mobIdParam, byte pathIdParam)
        {
            _mobId = mobIdParam;
            _pathId = pathIdParam;
        }
    }
}