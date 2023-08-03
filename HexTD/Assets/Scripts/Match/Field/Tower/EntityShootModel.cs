using System.Collections.Generic;
using ExitGames.Client.Photon;
using Match.Field.AttackEffect;
using Match.Serialization;
using Tools;
using Tools.Interfaces;

namespace Match.Field.Tower
{
    public class EntityShootModel : BaseDisposable, IOuterLogicUpdatable, ISerializableToNetwork
    {
        private readonly AttacksConfig _attacksConfig;
        private Dictionary<int, float> _cooldowns;

        private int _readyTowerAttackId;

        public bool IsSplashAttackReady => _readyTowerAttackId >= _attacksConfig.Attacks.Count;
        public bool IsReadyAttack => _readyTowerAttackId > -1;

        public int ReadyTowerIndex => IsSplashAttackReady
            ? _readyTowerAttackId - _attacksConfig.Attacks.Count
            : _readyTowerAttackId;

        public IReadOnlyDictionary<int, float> Cooldowns => _cooldowns;
        public int ReadyTowerAttackId => _readyTowerAttackId; 

        public EntityShootModel(AttacksConfig attacksConfig)
        {
            _attacksConfig = attacksConfig;
            
            _cooldowns = new Dictionary<int, float>(_attacksConfig.Attacks.Count + _attacksConfig.SplashAttacks.Count);
            int id = 0;
            
            foreach (var towerAttack in _attacksConfig.Attacks)
            {
                _cooldowns.Add(id, towerAttack.CooldownAndDelay);
                id++;
            }
            
            foreach (var towerAttack in _attacksConfig.SplashAttacks)
            {
                _cooldowns.Add(id, towerAttack.CooldownAndDelay);
                id++;
            }
        }

        public void LoadState(IReadOnlyDictionary<int, float> cooldowns, int readyTowerAttackId)
        {
            foreach (KeyValuePair<int,float> cooldownPair in cooldowns)
            {
                _cooldowns[cooldownPair.Key] = cooldownPair.Value;
            }

            _readyTowerAttackId = readyTowerAttackId;
        }

        public void OuterLogicUpdate(float frameLength)
        {
            for (int id = 0; id < _cooldowns.Count; id++)
            {
                if(_cooldowns[id] > 0)
                    _cooldowns[id] -= frameLength;
                
                if (_readyTowerAttackId == -1 && _cooldowns[id] <= 0f)
                    _readyTowerAttackId = id;
            }
        }

        public bool TryGetTowerAttack(out BaseAttackEffect towerAttack)
        {
            if (!IsReadyAttack)
            {
                towerAttack = null;
                return false;
            }

            towerAttack = GetReadyAttack();
            return true;
        }

        private BaseAttackEffect GetReadyAttack()
        {
            if (IsSplashAttackReady)
                return _attacksConfig.SplashAttacks[_readyTowerAttackId - _attacksConfig.Attacks.Count];
            else
                return _attacksConfig.Attacks[ReadyTowerIndex];
        }

        public void ReloadCurrentAttack()
        {
            _cooldowns[_readyTowerAttackId] = GetReadyAttack().CooldownAndDelay;
            _readyTowerAttackId = -1;
        }

        protected override void OnDispose()
        {
            _cooldowns.Clear();
        }

        public Hashtable ToNetwork()
        {
            Hashtable result = new Hashtable(_cooldowns.Count + 1)
            {
                { PhotonEventsConstants.SyncState.PlayerState.Towers.TowerIdParam, _readyTowerAttackId }
            };

            foreach (KeyValuePair<int,float> cooldownPair in _cooldowns)
            {
                result.Add(cooldownPair.Key, cooldownPair.Value);
            }

            return result;
        }
    }
}