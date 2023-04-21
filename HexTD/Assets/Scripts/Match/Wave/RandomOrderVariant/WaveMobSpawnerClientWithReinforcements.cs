namespace Match.Wave
{
    public class WaveMobSpawnerClientWithReinforcements : WaveMobSpawnerBaseWithReinforcements
    {
        public WaveMobSpawnerClientWithReinforcements(Context context) : base(context) { }

        protected override void RoleSpecialConstructorActions()
        {
            _context.IncomingGeneralGeneralCommands.StartWaveSpawn.Subscribe(StartWave);
        }

        protected override void NextWave()
        { }
    }
}