namespace Match.Wave.WaveMobSpawnerImplementations
{
    public class WaveMobSpawnerClient : WaveMobSpawnerBase
    {
        public WaveMobSpawnerClient(Context context) : base(context) { }

        protected override void RoleSpecialConstructorActions()
        {
            _context.IncomingGeneralGeneralCommands.StartWaveSpawn.Subscribe(StartWave);
        }

        protected override void NextWave()
        { }
    }
}