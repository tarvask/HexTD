using System.Collections.Generic;
using Match.Field.AttackEffect;
using Match.Field.Tower.TowerConfigs;
using Tools;
using Tools.Interfaces;

namespace Match.Field.Tower
{
    public class EntityShootModel : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly AttacksConfig _attacksConfig;
        private Dictionary<int, float> _cooldowns;

        private int _readyTowerAttackId;

        public bool IsReadyAttack => _readyTowerAttackId > -1;

        public EntityShootModel(AttacksConfig attacksConfig)
        {
            _attacksConfig = attacksConfig;
            
            _cooldowns = new Dictionary<int, float>();
            int id = 0;
            foreach (var towerAttack in _attacksConfig.Attacks)
            {
                _cooldowns.Add(id, towerAttack.Cooldown);
                id++;
            }
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            for(int id = 0; id < _cooldowns.Count; id++)
            {
                if(_cooldowns[id] > 0)
                    _cooldowns[id] -= frameLength;
                
                if (_readyTowerAttackId == -1 && _cooldowns[id] <= 0f)
                    _readyTowerAttackId = id;
            }
        }

        public bool TryReleaseTowerAttack(bool isReloadNeeded, out BaseAttackEffect towerAttack, out int attackindex)
        {
            if (!IsReadyAttack)
            {
                towerAttack = null;
                attackindex = -1;
                return false;
            }

            attackindex = _readyTowerAttackId;
            towerAttack = _attacksConfig.Attacks[_readyTowerAttackId];

            if (isReloadNeeded)
            {
                _cooldowns[_readyTowerAttackId] = towerAttack.Cooldown;
                _readyTowerAttackId = -1;
            }
            
            return true;
        }

        protected override void OnDispose()
        {
            _cooldowns.Clear();
        }
    }
}