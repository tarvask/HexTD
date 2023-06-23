using System;
using System.Threading.Tasks;
using HexSystem;
using Match.Field.Hexagons;
using Match.Field.Shooting;
using Match.Field.State;
using PathSystem;
using UI.ScreenSpaceOverlaySystem;
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
            public byte PathId { get; }
            public IPathEnumerator PathEnumerator { get; }
            public IHexPositionConversionService HexPositionConversionService { get; }
            public MobParameters Parameters { get; }
            public MobView View { get; }

            public Context(int id, int targetId, byte pathId,
                MobParameters parameters, 
                IPathEnumerator pathEnumerator,
                IHexPositionConversionService hexPositionConversionService,
                MobView view)
            {
                Id = id;
                TargetId = targetId;
                PathId = pathId;
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
        
        private float _currentPathLength;

        private int _blockerId;
        
        private float _attackingTimer;
        private bool _isHittingForTheFirstTime;
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
        public float PathLength => _currentPathLength;
        public float RemainingPathDistance => _context.PathEnumerator.PathLength - _currentPathLength;
        public override BaseReactiveModel BaseReactiveModel => _reactiveModel;
        public override Hex2d HexPosition => _currentHexPosition;
        public int BlockerId => _blockerId;
        public override ITargetView TargetView => _context.View;
        
        public bool IsReadyToAttack => _isHittingForTheFirstTime
            ? _attackingTimer >= _context.Parameters.Delay
            : _attackingTimer >= _context.Parameters.Cooldown;
        //public int RewardInCoins => _context.Parameters.RewardInCoins;
        public bool IsCarrion => _isCarrion;
        public bool IsBlocked => _isBlocked;
        public bool HasReachedCastle => _hasReachedCastle;
        public bool IsEscaping => _isEscaping;
        public bool IsInSafety => _isInSafety;
        public bool IsBoss => _context.Parameters.IsBoss;

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
        }

        public void LogicMove(float frameLength)
        {
            float distanceToTargetSqr = Vector3.SqrMagnitude(_currentPosition - _currentTargetPosition);
            float distancePerFrame = _reactiveModel.Speed.Value; //_buffsManager.ParameterResultValue(BuffedParameterType.MovementSpeed) * frameLength;

            if (distancePerFrame * distancePerFrame < distanceToTargetSqr)
            {
                _currentPosition = Vector3.MoveTowards(_currentPosition, _currentTargetPosition, distancePerFrame);
                _currentPathLength += distancePerFrame;
                if (!_wasNewHexReached && 
                    //_context.HexPositionConversionService.IsCloseToNewHex(distanceToTargetSqr)
                    _context.HexPositionConversionService.ToHexFromWorldPosition(_currentPosition) != _currentHexPosition)
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
            // cache current waypoint
            byte lastWaypointIndex = _context.PathEnumerator.CurrentPointIndex;
            // reset enumerator to iterate from the beginning
            _context.PathEnumerator.Reset();
            byte currentWaypointIndex = 0;
            Vector3 currentPoint = _context.HexPositionConversionService.GetHexPosition(_context.PathEnumerator.Current);

            while (currentWaypointIndex < lastWaypointIndex)
            {
                var prevPoint = currentPoint;
                _context.PathEnumerator.MoveNext();
                currentPoint = _context.HexPositionConversionService.GetHexPosition(_context.PathEnumerator.Current);
                _currentPathLength +=
                    Mathf.Abs(currentPoint.x - prevPoint.x) + Mathf.Abs(currentPoint.y - prevPoint.y);
                currentWaypointIndex++;
            }
            
            _currentPathLength +=
                Mathf.Abs(Position.x - currentPoint.x)
                + Mathf.Abs(Position.y - currentPoint.y);
            
            // restore enumerator from cached value
            _context.PathEnumerator.MoveTo(lastWaypointIndex);
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

        public void UpdateTimer(float frameLength)
        {
            _attackingTimer += frameLength;
        }

        public void Block(int blockerId)
        {
            _isBlocked = true;
            _blockerId = blockerId;
            _isHittingForTheFirstTime = true;
        }

        public void Unblock()
        {
            _isBlocked = false;
            _blockerId = 0;
        }

        public int Attack()
        {
            _attackingTimer = 0;
            _isHittingForTheFirstTime = false;
            return _context.Parameters.AttackPower;
        }
        
        public void LoadState(in PlayerState.MobState mobState)
        {
            _currentPosition = new Vector3(mobState.PositionX, 0, mobState.PositionZ);
            _context.PathEnumerator.MoveTo(mobState.NextWaypoint);
            _currentTargetPosition = _context.HexPositionConversionService.GetHexPosition(
                _context.PathEnumerator.Current, false);
            _currentHexPosition = _context.HexPositionConversionService.ToHexFromWorldPosition(_currentPosition, false);
            ComputePathLengthAfterTeleport();
            _reactiveModel.SetHealth(mobState.CurrentHealth);
        }
        
        public PlayerState.MobState GetMobState()
        {
            return new PlayerState.MobState(_context.Id, _context.TargetId, _context.Parameters.TypeId,
                Position.x, Position.z, _context.PathId, _context.PathEnumerator.CurrentPointIndex,
                _reactiveModel.Health.Value, _blockerId);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            Object.Destroy(_context.View.gameObject);
        }
    }
}