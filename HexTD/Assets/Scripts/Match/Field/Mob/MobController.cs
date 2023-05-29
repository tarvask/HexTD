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
    public class MobController : BaseTargetEntity
    {
        public struct Context
        {
            public int Id { get; }
            public int TargetId { get; }
            public IPathEnumerator PathEnumerator { get; }
            public IHexPositionConversionService HexPositionConversionService { get; }
            public MobParameters Parameters { get; }
            public MobView View { get; }

            public Context(int id, int targetId, MobParameters parameters, 
                IPathEnumerator pathEnumerator,
                IHexPositionConversionService hexPositionConversionService,
                MobView view)
            {
                Id = id;
                TargetId = targetId;
                Parameters = parameters;
                PathEnumerator = pathEnumerator;
                HexPositionConversionService = hexPositionConversionService;
                View = view;
            }
        }

        private readonly Context _context;
        private readonly MobReactiveModel _reactiveModel;
        private Vector3 _currentPosition;
        private Vector3 _currentTargetPosition;
        private Hex2d _currentHexPosition;
        private Hex2d _currentTargetHexPosition;
        
        private byte _pathIndex;
        private float _currentPathLength;

        private int _blockerId;
        
        private float _attackingTimer;
        private bool _wasNewHexReached;
        private bool _isCarrion;
        private bool _isBlocked;
        private bool _hasReachedCastle;
        private bool _isEscaping;
        private bool _isInSafety;
        private bool _isBoss;

        public int Id => _context.Id;
        public override int TargetId => _context.TargetId;
        public override Vector3 Position => _context.View.transform.localPosition;
        public IReadOnlyReactiveProperty<float> Health => _reactiveModel.Health;
        public IReadonlyBuffableValue<float> Speed => _reactiveModel.Speed;
        public float PathLength => _currentPathLength;
        public float RemainingPathDistance => _context.PathEnumerator.PathLength - _currentPathLength;
        public override BaseReactiveModel BaseReactiveModel => _reactiveModel;
        public override Hex2d HexPosition => _currentHexPosition;
        public Hex2d CurrentTargetHexPosition => _currentTargetHexPosition;
        public int BlockerId => _blockerId;
        
        public bool IsReadyToAttack => _attackingTimer >= _context.Parameters.Cooldown + _context.Parameters.Delay;
        //public int RewardInCoins => _context.Parameters.RewardInCoins;
        public bool IsCarrion => _isCarrion;
        public bool IsBlocked => _isBlocked;
        public bool HasReachedCastle => _hasReachedCastle;
        public bool IsEscaping => _isEscaping;
        public bool IsInSafety => _isInSafety;
        public bool IsBoss => _context.Parameters.IsBoss;
        
        public bool CanMove => true;

        public MobController(Context context)
        {
            _context = context;

            _reactiveModel = AddDisposable(new MobReactiveModel(_context.Parameters.Speed, _context.Parameters.HealthPoints));
            
            _currentPathLength = 0;

            _wasNewHexReached = false;
            _context.PathEnumerator.Reset();

            _currentTargetPosition = _context.HexPositionConversionService.GetHexPosition(
                _context.PathEnumerator.Current, false);
            _currentPosition = _currentTargetPosition;
            _context.View.transform.localPosition = _currentPosition;
            _currentHexPosition = _context.HexPositionConversionService.ToHexFromWorldPosition(_currentPosition, false);
			_currentTargetHexPosition = _currentHexPosition;
        }

        public void LogicMove(float frameLength)
        {
            float distanceToTarget = Vector3.Magnitude(_currentPosition - _currentTargetPosition);
            float distancePerFrame = _reactiveModel.Speed.Value; //_buffsManager.ParameterResultValue(BuffedParameterType.MovementSpeed) * frameLength;

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
                    _currentTargetHexPosition = _context.PathEnumerator.Current;
                    _currentTargetPosition = _context.HexPositionConversionService.GetHexPosition(
                        _context.PathEnumerator.Current, false);
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
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _isCarrion = true;
            });
        }
        
        public void Escape()
        {
            _isEscaping = true;

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

        public void Block(int blockerId)
        {
            _isBlocked = true;
            _blockerId = blockerId;
        }

        public void Unblock()
        {
            _isBlocked = false;
            _blockerId = 0;
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
                Position.x, Position.y, _pathIndex, _context.PathEnumerator.CurrentPointIndex,
                _reactiveModel.Health.Value, _blockerId);
        }

        public void UpdateAddBuff(PrioritizeLinkedList<IBuff<ITarget>> buffs, IBuff<ITarget> addedBuff)
        {
            addedBuff.ApplyBuff(this);
        }

        public void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<ITarget>> buffs, IEnumerable<IBuff<ITarget>> removedBuffs)
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