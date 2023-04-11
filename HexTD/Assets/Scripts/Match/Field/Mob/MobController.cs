using System.Threading.Tasks;
using HexSystem;
using Match.Field.Hexagons;
using Match.Field.Shooting;
using Match.Field.State;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field.Mob
{
    public class MobController : BaseDisposable, IShootable
    {
        public struct Context
        {
            public int Id { get; }
            public int TargetId { get; }
            public MobParameters Parameters { get; }
            public MobView View { get; }

            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }

            public Context(int id, int targetId, MobParameters parameters, MobView view,
                ReactiveCommand<MobController> removeMobReactiveCommand)
            {
                Id = id;
                TargetId = targetId;
                Parameters = parameters;
                View = view;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly MobReactiveModel _reactiveModel;
        //private readonly MobBuffsManager _buffsManager;
        private Vector3 _currentPosition;
        private Hex2d _currentHexPosition;
        private float _currentPathLength;
        private byte _nextWaypoint;
        private float _attackingTimer;
        private byte _currentDamageTextIndex;
        private bool _isCarrion;
        private bool _hasReachedCastle;

        public int Id => _context.Id;
        public int TargetId => _context.TargetId;
        public int Health => _reactiveModel.HealthReactiveProperty.Value;
        public float PathLength => _currentPathLength;
        public Vector3 Position => _currentPosition;
        public Hex2d HexPosition => _currentHexPosition;
        public bool HasReachedCastle => _hasReachedCastle;
        public bool IsReadyToAttack => _attackingTimer >= _context.Parameters.ReloadTime;
        public int RewardInSilver => _context.Parameters.RewardInSilver;
        public int CrystalDropChance => _context.Parameters.CrystalDropChance;
        public bool IsCarrion => _isCarrion;
        public bool CanMove => true;

        public MobController(Context context)
        {
            _context = context;

            _reactiveModel = AddDisposable(new MobReactiveModel(_context.Parameters.Speed, _context.Parameters.HealthPoints));
            // _buffsManager = AddDisposable(new MobBuffsManager(new MobBuffsManager.Context(_reactiveModel.SpeedReactiveProperty,
            //     _reactiveModel.HealthReactiveProperty)));
            
            _currentPathLength = 0;
            _nextWaypoint = 0;
            _currentDamageTextIndex = 0;
        }

        public void UpdateHealth(float frameLength)
        {
            //_buffsManager.OuterUpdate(frameLength);
            //_reactiveModel.HealthReactiveProperty.Value += (int)_buffsManager.ParameterResultValue(BuffedParameterType.Health);
        }

        public void LogicMove(float frameLength)
        {
            if (_hasReachedCastle)
                return;
            
            Vector3 currentPosition = Position;
            Vector3 targetPosition = Vector3.zero;
            float distanceToTargetSqr = Vector3.SqrMagnitude(currentPosition - targetPosition);
            float distancePerFrame = _context.Parameters.Speed;//_buffsManager.ParameterResultValue(BuffedParameterType.MovementSpeed) * frameLength;

            // special treatment for range attack when approaching castle
            if (!CheckRangeAttackDistance(ref currentPosition))
            {
                if (distancePerFrame * distancePerFrame < distanceToTargetSqr)
                {
                    _currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, distancePerFrame);
                    _currentPathLength += distancePerFrame;
                }
                else
                {
                    _nextWaypoint++;
                }
            }

            _hasReachedCastle = false;
        }

        public void UpdateHexPosition(IHexPositionConversionService hexPositionConversionService)
        {
            _currentHexPosition = hexPositionConversionService.ToHexFromWorldPosition(_currentPosition);
        }

        public void VisualMove(float frameLength)
        {
            _context.View.transform.localPosition = Vector3.Lerp(
                _context.View.transform.localPosition, _currentPosition, FieldController.MoveLerpCoeff);
        }

        private bool CheckRangeAttackDistance(ref Vector3 currentPosition)
        {
            //if (_context.Parameters.HasRangeDamage)
            //{
            //    float distanceToCastleSqr =
            //        Vector3.SqrMagnitude(currentPosition - _context.Waypoints[_context.Waypoints.Length - 1]);
//
            //    if (distanceToCastleSqr < _context.Parameters.AttackRangeRadius * _context.Parameters.AttackRangeRadius)
            //    {
            //        _nextWaypoint = (byte) _context.Waypoints.Length;
            //        return true;
            //    }
            //    
            //    return false;
            //}
            
            return false;
        }

        private void ComputePathLengthAfterTeleport()
        {
            //byte waypointIndex = 1;
//
            //while (waypointIndex < _nextWaypoint)
            //{
            //    _currentPathLength +=
            //        Mathf.Abs(_context.Waypoints[waypointIndex].x - _context.Waypoints[waypointIndex - 1].x)
            //        + Mathf.Abs(_context.Waypoints[waypointIndex].y - _context.Waypoints[waypointIndex - 1].y);
            //    waypointIndex++;
            //}
            //
            //_currentPathLength +=
            //    Mathf.Abs(Position.x - _context.Waypoints[waypointIndex - 1].x)
            //    + Mathf.Abs(Position.y - _context.Waypoints[waypointIndex - 1].y);
        }

        public void Hurt(int damage)
        {
            _reactiveModel.HealthReactiveProperty.Value -= damage;
            ShowDamage(damage);
        }

        // public void ApplyBuffs(List<AbstractBuffParameters> buffs)
        // {
        //     foreach (AbstractBuffParameters buff in buffs)
        //     {
        //         _buffsManager.AddBuff(buff);
        //     }
        // }
        //
        // public void RemoveBuff(BuffedParameterType buffedParameterType, byte buffSubtype)
        // {
        //     _buffsManager.RemoveBuff(buffedParameterType, buffSubtype);
        // }
        //
        // public bool HasBuff(AbstractBuffParameters buff)
        // {
        //     return _buffsManager.HasBuff(buff);
        // }

        public void Die()
        {
            _context.RemoveMobReactiveCommand.Execute(this);
            
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _isCarrion = true;
            });
        }

        public void RemoveBody()
        {
            Dispose();
        }

        public void UpdateTimer(float frameLength)
        {
            _attackingTimer += frameLength;
        }

        public int Attack()
        {
            _attackingTimer = 0;
            return _context.Parameters.AttackPower;
        }

        private void ShowDamage(int damage)
        {
            //_context.View.DamageTextItems[_currentDamageTextIndex].text = $"{-damage}";
            //_context.View.DamageTextItems[_currentDamageTextIndex].gameObject.SetActive(false);
            //_context.View.DamageTextItems[_currentDamageTextIndex].gameObject.SetActive(true);
            //_currentDamageTextIndex++;
            
            //if (_currentDamageTextIndex == _context.View.DamageTextItems.Length)
            //    _currentDamageTextIndex = 0;
        }
        
        public void LoadState(in PlayerState.MobState mobState)
        {
            _nextWaypoint = mobState.NextWaypoint;
            ComputePathLengthAfterTeleport();
            _reactiveModel.HealthReactiveProperty.Value = mobState.CurrentHealth;
        }
        
        public PlayerState.MobState GetMobState()
        {
            return new PlayerState.MobState(_context.Id, _context.TargetId, _context.Parameters.TypeId,
                Position.x, Position.y, _nextWaypoint, _reactiveModel.HealthReactiveProperty.Value);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            Object.Destroy(_context.View.gameObject);
        }
    }
}