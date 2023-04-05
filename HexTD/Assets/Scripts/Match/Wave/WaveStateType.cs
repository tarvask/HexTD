namespace Match.Wave
{
    public enum WaveStateType : byte
    {
        Undefined = 0,
        Loading = 1,
        BetweenWavesTechnicalPause = 3,
        BetweenWavesPlanning = 4,
        ArtifactChoosing = 5,
        Spawning = 6,
    }
}