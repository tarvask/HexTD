using Match;
using Match.Field;
using Match.Wave;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MatchConfig))]
    public class MatchConfigEditor : UnityEditor.Editor
    {
        private const int TableCellWidth = 60;
        private MatchConfig _config;

        private int _wavesArraySize;
        private bool[] _wavesExpandArray;
        
        private void OnEnable()
        {
            _config = target as MatchConfig;
        }

        public override void OnInspectorGUI()
        {
            // waves
            DrawWaves();
            EditorGUILayout.Space();
            // cells
            //DrawCells();
            EditorGUILayout.Space();
            // coins
            //_config.CoinsCount = EditorGUILayout.IntField("Coins count", _config.CoinsCount);
            // energy
            _config.EnergyStartCount = EditorGUILayout.IntField("Energy start count", _config.EnergyStartCount);

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DrawWaves()
        {
            EditorGUILayout.LabelField("Waves");

            if (_config.Waves == null)
            {
                _config.Waves = new WaveParametersStrict[0];
                _wavesExpandArray = new bool[0];
            }

            if (_wavesExpandArray == null || _config.Waves.Length != _wavesExpandArray.Length)
                _wavesExpandArray = new bool[_config.Waves.Length];

            // deal with array size
            _wavesArraySize = EditorGUILayout.IntField("Size", _config.Waves.Length);
            if (_wavesArraySize != _config.Waves.Length)
            {
                // create array with new size
                WaveParametersStrict[] newArray = new WaveParametersStrict[_wavesArraySize];
                // copy existing array as much as possible
                for (int i = 0; i < _wavesArraySize; i++)
                {
                    if (i < _config.Waves.Length)
                        newArray[i] = _config.Waves[i];
                    else
                        newArray[i] = new WaveParametersStrict();
                }
                _config.Waves = newArray;
                
                // fold all waves, the whole array is false
                _wavesExpandArray = new bool[_wavesArraySize];
            }

            // waves content
            if (_config.Waves.Length == 0)
                EditorGUILayout.LabelField("0 waves");
                
            for (var waveIndex = 0; waveIndex < _config.Waves.Length; waveIndex++)
            {
                WaveParametersStrict wave = _config.Waves[waveIndex];
                _wavesExpandArray[waveIndex] = EditorGUILayout.Foldout(_wavesExpandArray[waveIndex], $"Wave {waveIndex}");

                if (_wavesExpandArray[waveIndex])
                {
                    DrawSingleWaveStrict(ref wave);
                    _config.Waves[waveIndex] = wave;
                }
            }
        }
        
        private void DrawSingleWaveStrict(ref WaveParametersStrict wave)
        {
            // size
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Wave size");
                EditorGUILayout.LabelField($"{wave.Size}");
            }
            EditorGUILayout.EndHorizontal();
            
            // duration
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Wave duration in seconds");
                EditorGUILayout.LabelField($"{wave.Duration}");
            }
            EditorGUILayout.EndHorizontal();
            
            float pauseBeforeWave = EditorGUILayout.FloatField("Pause before wave", wave.PauseBeforeWave);
            EditorGUILayout.Space();
            
            // elements
            WaveElementDelay[] waveElements = wave.Elements;
            DrawWaveStrictElements(ref waveElements);
            wave.CheckConsistency();
            
            wave = new WaveParametersStrict(wave.Size, wave.Duration, pauseBeforeWave, waveElements);
        }

        private void DrawSingleWaveWithChances(ref WaveParametersWithChances wave)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Wave size");
                EditorGUILayout.LabelField($"{wave.Size}");
            }
            EditorGUILayout.EndHorizontal();
            float duration = EditorGUILayout.FloatField("Wave duration in seconds", wave.Duration);
            float minSpawnPause = EditorGUILayout.FloatField("Min spawn pause", wave.MinSpawnPause);
            float maxSpawnPause = EditorGUILayout.FloatField("Max spawn pause", wave.MaxSpawnPause);
            float pauseBeforeWave = EditorGUILayout.FloatField("Pause before wave", wave.PauseBeforeWave);
            EditorGUILayout.Space();
            
            // elements
            WaveElementChance[] waveElements = wave.Elements;
            DrawWaveWithChancesElements(ref waveElements);
            wave.CheckConsistency();
            
            wave = new WaveParametersWithChances(wave.Size, duration, minSpawnPause, maxSpawnPause, 
                pauseBeforeWave, waveElements);
        }

        private void DrawWaveWithChancesElements(ref WaveElementChance[] waveElements)
        {
            EditorGUILayout.LabelField("Wave elements");
            
            if (waveElements == null)
                waveElements = new WaveElementChance[0];
                
            // deal with array size
            int waveElementsSize = EditorGUILayout.IntField("Size", waveElements.Length);
            if (waveElementsSize != waveElements.Length)
            {
                // create array with new size
                WaveElementChance[] newArray = new WaveElementChance[waveElementsSize];
                // copy existing array as much as possible
                for (int i = 0; i < waveElementsSize; i++)
                {
                    if (i < waveElements.Length)
                    {
                        newArray[i] = waveElements[i];
                    }
                }
                waveElements = newArray;
            }

            // elements content
            EditorGUILayout.BeginVertical();
            {
                for (int elementIndex = 0; elementIndex < waveElements.Length; elementIndex++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        byte mobId = (byte) EditorGUILayout.IntField("Mob id", waveElements[elementIndex].MobId,
                            GUILayout.Width(TableCellWidth * 3), GUILayout.ExpandWidth(true));
                        byte mobCount =
                            (byte) EditorGUILayout.IntField("Max count", waveElements[elementIndex].MaxCount,
                                GUILayout.Width(TableCellWidth * 3), GUILayout.ExpandWidth(true));
                        waveElements[elementIndex] = new WaveElementChance(mobId, mobCount);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        private void DrawWaveStrictElements(ref WaveElementDelay[] waveElements)
        {
            EditorGUILayout.LabelField("Wave elements");
            
            if (waveElements == null)
                waveElements = new WaveElementDelay[0];
                
            // deal with array size
            int waveElementsSize = EditorGUILayout.IntField("Size", waveElements.Length);
            if (waveElementsSize != waveElements.Length)
            {
                // create array with new size
                WaveElementDelay[] newArray = new WaveElementDelay[waveElementsSize];
                // copy existing array as much as possible
                for (int i = 0; i < waveElementsSize; i++)
                {
                    if (i < waveElements.Length)
                    {
                        newArray[i] = waveElements[i];
                    }
                }
                waveElements = newArray;
            }

            // elements content
            EditorGUILayout.BeginVertical();
            {
                for (int elementIndex = 0; elementIndex < waveElements.Length; elementIndex++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        byte mobId = (byte) EditorGUILayout.IntField("Mob id", waveElements[elementIndex].MobId,
                            GUILayout.Width(TableCellWidth * 3), GUILayout.ExpandWidth(true));
                        float mobDelay =
                            (byte) EditorGUILayout.FloatField("Delay", waveElements[elementIndex].Delay,
                                GUILayout.Width(TableCellWidth * 3), GUILayout.ExpandWidth(true));
                        waveElements[elementIndex] = new WaveElementDelay(mobId, mobDelay);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndVertical();
        }

        //private void DrawCells()
        //{
        //    EditorGUILayout.LabelField("Cells");
        //    EditorGUILayout.LabelField($"Field width is {MatchShortParameters.FieldWidth}," +
        //                               $" field height is {MatchShortParameters.FieldHeight}");
//
        //    if (_config.Cells == null)
        //    {
        //        // create and fill with Unavailable
        //        _config.Cells = new FieldHexType[MatchShortParameters.FieldHeight * MatchShortParameters.FieldWidth];
        //        for (int cellIndex = 0; cellIndex < MatchShortParameters.FieldHeight * MatchShortParameters.FieldWidth; cellIndex++)
        //        {
        //            _config.Cells[cellIndex] = FieldHexType.Unavailable;
        //        }
        //    }
//
        //    // table content
        //    // draw rows from last to first
        //    for (int cellY = MatchShortParameters.FieldHeight - 1; cellY >= 0; cellY--)
        //    {
        //        EditorGUILayout.BeginHorizontal();
        //        {
        //            // coeff values
        //            for (int cellX = 0; cellX < MatchShortParameters.FieldWidth; cellX++)
        //            {
        //                _config.Cells[cellY * MatchShortParameters.FieldWidth + cellX] =
        //                    (FieldHexType) EditorGUILayout.EnumPopup("", _config.Cells[cellY * MatchShortParameters.FieldWidth + cellX],
        //                        GUILayout.Width(TableCellWidth));
        //            }
        //        }
        //        EditorGUILayout.EndHorizontal();
        //    }
        //}
    }
}