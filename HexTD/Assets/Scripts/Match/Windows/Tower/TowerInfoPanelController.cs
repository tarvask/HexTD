using Match.Field.Tower.TowerConfigs;
using Tools;
using UnityEngine;

namespace Match.Windows.Tower
{
    public class TowerInfoPanelController : BaseDisposable
    {
        public struct Context
        {
            public TowerInfoPanelView View { get; }
            
            public Context(TowerInfoPanelView view)
            {
                View = view;
            }
        }

        private readonly Context _context;
        
        public TowerInfoPanelController(Context context)
        {
            _context = context;
        }
        
        public void Init(TowerConfigNew towerParameters, int towerLevel)
        {
            _context.View.TowerNameText.text = towerParameters.RegularParameters.TowerName;

            //TODO: adaptive new levels config init
            //_context.View.TowerDamageText.text = $"{towerParameters.TowerLevelConfigs[towerLevel - 1].LevelRegularParams.Data.AttackPower}";
            //_context.View.TowerAttackRateText.text =
            //    $"{(1f / towerParameters.Levels[towerLevel - 1].LevelRegularParams.Data.ReloadTime):F2} / sec";
            //_context.View.TowerRangeText.text = $"{towerParameters.Levels[towerLevel - 1].LevelRegularParams.Data.AttackRadiusInHexCount}";
            //_context.View.TowerTargetText.text = $"{towerParameters.RegularParameters.Data.TargetFindingTacticType}";
            
            // buffs
            // if (buffs != null)
            // {
            //     foreach (KeyValuePair<int, AbstractBuffModel> buffPair in buffs)
            //     {
            //         TextMeshProUGUI buffInfoElement = Object.Instantiate(_context.View.BuffInfoElementPrefab, _context.View.BuffsInfoScrollRoot);
            //         buffInfoElement.text = buffPair.Value.GetTextInfo();
            //     }
            // }
        }

        public void Clear()
        {
            // clear buff info elements
            foreach (Transform buffInfoElementTransform in _context.View.BuffsInfoScrollRoot)
            {
                Object.Destroy(buffInfoElementTransform.gameObject);
            }
        }
    }
}