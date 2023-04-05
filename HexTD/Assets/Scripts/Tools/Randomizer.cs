using UniRx;
using UnityEngine;
using static UnityEngine.Random;

namespace Tools
{
    // decorator to UnityEngine.Random methods
    public static class Randomizer
    {
        public const bool ShowLog = true;
        
        private static int _currentSeed;
        private static readonly ReactiveProperty<int> _randomCallsCountReactiveProperty;
        private static readonly ReactiveProperty<int> _lastRandomIntValueReactiveProperty;
        private static readonly ReactiveProperty<float> _lastRandomFloatValueReactiveProperty;

        public static int CurrentSeed => _currentSeed;
        public static IReadOnlyReactiveProperty<int> RandomCallsCountReactiveProperty => _randomCallsCountReactiveProperty;
        public static IReadOnlyReactiveProperty<int> LastRandomIntValueReactiveProperty => _lastRandomIntValueReactiveProperty;
        public static ReactiveProperty<float> LastRandomFloatValueReactiveProperty => _lastRandomFloatValueReactiveProperty;

        static Randomizer()
        {
            _randomCallsCountReactiveProperty = new ReactiveProperty<int>(0);
            _lastRandomIntValueReactiveProperty = new ReactiveProperty<int>(0);
            _lastRandomFloatValueReactiveProperty = new ReactiveProperty<float>(0);
        }

        public static void InitState(int newSeed)
        {
            _currentSeed = newSeed;
            Random.InitState(_currentSeed);
            _randomCallsCountReactiveProperty.Value = 0;
        }
        
        public static int GetRandomInRange(int inclusiveMin, int exclusiveMax)
        {
            _randomCallsCountReactiveProperty.Value++;
            _lastRandomIntValueReactiveProperty.Value = Range(inclusiveMin, exclusiveMax);

            return _lastRandomIntValueReactiveProperty.Value;
        }
        
        public static float GetRandomInRange(float inclusiveMin, float inclusiveMax)
        {
            _randomCallsCountReactiveProperty.Value++;
            _lastRandomFloatValueReactiveProperty.Value = Range(inclusiveMin, inclusiveMax);

            return _lastRandomFloatValueReactiveProperty.Value;
        }
    }
}