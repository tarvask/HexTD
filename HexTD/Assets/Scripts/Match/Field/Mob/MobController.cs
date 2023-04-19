using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuffLogic;
using HexSystem;
using Match.Field.Hexagons;
using Match.Field.Shooting;
using Match.Field.State;
using PathSystem;
using Tools.PriorityTools;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field.Mob
{
    public class MobController : BaseTargetableEntity
    {
        public struct Context
        {
            public int Id { get; }
            public int TargetId { get; }
            public IPathEnumerator PathEnumerator { get; }
            public IHexPositionConversionService HexPositionConversionService { get; }
            public MobParameters Parameters { get; }
            public MobView View { get; }

            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }

            public Context(int id, int targetId, MobParameters parameters, 
                IPathEnumerator pathEnumerator,
                IHexPositionConversionService hexPositionConversionService,
                MobView view,
                ReactiveCommand<MobController> removeMobReactiveCommand)
            {
                Id = id;
                TargetId = targetId;
                Parameters = parameters;
                PathEnumerator = pathEnumerator;
                HexPositionConversionService = hexPositionConversionService;
                View = view;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly MobReactiveModel _reactiveModel;
        private Vector3 _currentPosition;
        private Vector3 _currentTargetPosition;
        private Hex2d _currentHexPosition;
        
        private byte _pathIndex;
        private float _currentPathLength;
        
        private float _attackingTimer;
        private bool _wasNewHexReached;
        private bool _isCarrion;
        private bool _hasReachedCastle;
        private bool _isEscaping;
        private bool _isInSafety;

        public int Id => _context.Id;
        public override int TargetId => _context.TargetId;
        public override Vector3 Position => _context.View.transform.localPosition;
        public IReadOnlyReactiveProperty<float> Health => _reactiveModel.Health;
        public IReadonlyBuffableValue<float> Speed => _reactiveModel.Speed;
        public float PathLength => _currentPathLength;
        public float RemainingPathDistance => _context.PathEnumerator.PathLength - _currentPathLength;
        public override BaseReactiveModel BaseReactiveModel => _reactiveModel;
        public override Hex2d HexPosition => _currentHexPosition;
        public bool HasReachedCastle => _hasReachedCastle;
        public bool IsReadyToAttack => _attackingTimer >= _context.Parameters.ReloadTime;
        public int RewardInCoins => _context.Parameters.RewardInCoins;
        public bool IsCarrion => _isCarrion;
        public bool IsEscaping => _isEscaping;
        public bool IsInSafety => _isInSafety;
        
        public bool CanMove => true;

        public MobController(Context context)
        {
            _context = context;

            _reactiveModel = AddDisposable(new MobReactiveModel(_context.Parameters.Speed, _context.Parameters.HealthPoints));
            
            _currentPathLength = 0;

            _wasNewHexReached = false;
            _context.PathEnumerator.Reset();

            _currentTargetPosition = _context.HexPositionConversionService.GetUpHexPosition(
                _context.PathEnumerator.Current, false);
            _currentPosition = _currentTargetPosition;
            _context.View.transform.localPosition = _currentPosition;
            _currentHexPosition = _context.HexPositionConversionService.ToHexFromWorldPosition(_currentPosition, false);
        }

        public void LogicMove(float frameLength)
        {
            if (_hasReachedCastle)
                return;
            
            float distanceToTarget = Vector3.Magnitude(_currentPosition - _currentTargetPosition);
            float distancePerFrame = _reactiveModel.Speed.Value; //_buffsManager.ParameterResultValue(BuffedParameterType.MovementSpeed) * frameLength;

            // special treatment for range attack when approaching castle
            if (!CheckRangeAttackDistance(ref _currentPosition))
            {
                if (distancePerFrame < distanceToTarget)
                {
                    _currentPosition = Vector3.MoveTowards(_currentPosition, _currentTargetPosition, distancePerFrame);
                    _currentPathLength += distancePerFrame;
                    if (!_wasNewHexReached && 
                        _context.HexPositionConversionService.IsCloseToNewHex(distanceToTarget))
                    {
                        _wasNewHexReached = true;
                        UpdateHexPosition();
                    }
                }
                else
                {
                    _hasReachedCastle = !_context.PathEnumerator.MoveNext();
                    _wasNewHexReached = false;

                    if (!_hasReachedCastle)
                    {
                        _currentTargetPosition = _context.HexPositionConversionService.GetUpHexPosition(
                            _context.PathEnumerator.Current, false);
                    }
                }
            }
        }

        public void SubscribeOnHexPositionChange(Action<MobController, Hex2d> actionOnChange)
        {
            _reactiveModel.SubscribeOnHexPositionChange(actionOnChange);
        }

        public void UnsubscribeOnHexPositionChange(Action<MobController, Hex2d> actionOnChange)
        {
            _reactiveModel.UnsubscribeOnHexPositionChange(actionOnChange);
        }
        
        private void UpdateHexPosition()
        {
            var oldHexPosition = _currentHexPosition;
            _currentHexPosition = _context.HexPositionConversionService.ToHexFromWorldPosition(_currentPosition, false);
            _reactiveModel.OnHexPositionChange(this, oldHexPosition);
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

        public override void Heal(float heal)
        {
            float newHealth = _reactiveModel.Health.Value + heal;
            newHealth = Mathf.Clamp(newHealth, 0, _reactiveModel.MaxHealth.Value);
            _reactiveModel.SetHealth(newHealth);
        }

        public override void Hurt(float damage)
        {
            _reactiveModel.SetHealth(_reactiveModel.Health.Value - damage);
        }

        public void Die()
        {
            _context.RemoveMobReactiveCommand.Execute(this);
            
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _isCarrion = true;
            });
        }
        
        public void Escape()
        {
            _isEscaping = true;
            _context.RemoveMobReactiveCommand.Execute(this);
            
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _isInSafety = true;
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
        
        public void LoadState(in PlayerState.MobState mobState)
        {
            _context.PathEnumerator.MoveTo(mobState.NextWaypoint);
            ComputePathLengthAfterTeleport();
            _reactiveModel.SetHealth(mobState.CurrentHealth);
        }
        
        public PlayerState.MobState GetMobState()
        {
            return new PlayerState.MobState(_context.Id, _context.TargetId, _context.Parameters.TypeId,
                Position.x, Position.y, _context.PathEnumerator.CurrentPointIndex, _reactiveModel.Health.Value);
        }

        public void UpdateAddBuff(PrioritizeLinkedList<IBuff<ITargetable>> buffs, IBuff<ITargetable> addedBuff)
        {
            addedBuff.ApplyBuff(this);
        }

        public void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<ITargetable>> buffs, IEnumerable<IBuff<ITargetable>> removedBuffs)
        {
            foreach (var removedBuff in removedBuffs)
            {
                removedBuff.ApplyBuff(this);
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            Object.Destroy(_context.View.gameObject);
        }
    }
}