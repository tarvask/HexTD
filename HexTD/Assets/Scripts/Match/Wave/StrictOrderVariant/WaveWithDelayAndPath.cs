using ExitGames.Client.Photon;

namespace Match.Wave
{
    public readonly struct WaveWithDelayAndPath
    {
        private readonly WaveParametersStrict _waveParameters;
        private readonly float _waveDelay;
        private readonly byte _pathId;

        public WaveParametersStrict WaveParameters => _waveParameters;
        public float WaveDelay => _waveDelay;
        public byte PathId => _pathId;

        public WaveWithDelayAndPath(WaveParametersStrict waveParameters, float waveDelay, byte pathId)
        {
            _waveParameters = waveParameters;
            _waveDelay = waveDelay;
            _pathId = pathId;
        }
        
        public Hashtable ToNetwork()
        {
            Hashtable waveNetwork = _waveParameters.ToNetwork();
            waveNetwork[PhotonEventsConstants.SyncMatchOnLoad.WaveStrictOrder.WaveDelayParam] = _waveDelay;
            waveNetwork[PhotonEventsConstants.SyncMatchOnLoad.WaveStrictOrder.PathIdParam] = _pathId;

            return waveNetwork;
        }
    }
}