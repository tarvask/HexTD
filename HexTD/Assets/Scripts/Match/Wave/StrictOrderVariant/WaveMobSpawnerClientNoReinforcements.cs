namespace Match.Wave
{
    public class WaveMobSpawnerClientNoReinforcements : WaveMobSpawnerBaseNoReinforcements
    {
        public WaveMobSpawnerClientNoReinforcements(Context context) : base(context) { }

        protected override void RoleSpecialConstructorActions()
        {
            _context.IncomingGeneralGeneralCommands.StartWaveSpawn.Subscribe(StartWave);
        }

        protected override void NextWave()
        { }
    }
}