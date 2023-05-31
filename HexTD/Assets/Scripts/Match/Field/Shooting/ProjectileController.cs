using System.Threading.Tasks;
using Match.Field.AttackEffect;
using Match.Field.State;
using Tools;
using UnityEngine;

namespace Match.Field.Shooting
{
    public class ProjectileController : BaseDisposable
    {
        public struct Context
        {
            public int Id { get; }
            public ProjectileView View { get; }
            public BaseAttackEffect BaseAttackEffect { get; }
            public int AttackIndex { get; }
            public SplashShootType SplashShootType { get; }
            public float Speed { get; }
            public int SpawnTowerId { get; }
            public int TargetId { get; }
            public bool HasSplashDamage { get; }
            public bool HasTargetVolumeDamage { get; }

            public Context(int id, ProjectileView view, 
                BaseAttackEffect baseAttackEffect, int attackIndex, SplashShootType splashShootType, 
                float speed, bool hasSplashDamage, bool hasTargetVolumeDamage, int spawnTowerId, int targetId)
            {
                Id = id;
                View = view;
                BaseAttackEffect = baseAttackEffect;
                AttackIndex = attackIndex;
                SplashShootType = splashShootType;

                Speed = speed;
                HasSplashDamage = hasSplashDamage;
                HasTargetVolumeDamage = hasTargetVolumeDamage;
                SpawnTowerId = spawnTowerId;
                TargetId = targetId;
            }
        }

        private readonly Context _context;

        private Vector3 _currentPosition;
        private Vector3 _currentTargetPosition;
        private bool _hasReachedTarget;
        private bool _hasPlayedSplash;

        public BaseAttackEffect BaseAttackEffect => _context.BaseAttackEffect;
        public int Id => _context.Id;
        public int SpawnTowerId => _context.SpawnTowerId;
        public SplashShootType SplashShootType => _context.SplashShootType;
        public int TargetId => _context.TargetId;
        public bool HasSplashDamage => _context.HasSplashDamage;
        public bool HasTargetVolumeDamage => _context.HasTargetVolumeDamage;
        public Vector3 CurrentPosition => _currentPosition;
        public Vector3 CurrentTargetPosition => _currentTargetPosition;
        public bool HasReachedTarget => _hasReachedTarget;
        public bool HasPlayedSplash => _hasPlayedSplash;

        public ProjectileController(Context context)
        {
            _context = context;
            AddComponent(_context.View);
            AddComponent(_context.View.gameObject);
        }

        public void LogicMove(Vector3 targetPosition, float frameLength)
        {
            _currentPosition = _context.View.transform.localPosition;
            _currentTargetPosition = targetPosition;
            float distanceToTargetSqr = Vector3.SqrMagnitude(_currentPosition - targetPosition);
            float distancePerFrameSqr = _context.Speed * frameLength;
                
            if (distanceToTargetSqr > distancePerFrameSqr && distanceToTargetSqr > 0.0001f)
            {
                _currentPosition = Vector3.MoveTowards(_currentPosition, targetPosition, distancePerFrameSqr);
            }
            else
            {
                _currentPosition = targetPosition;
                Stop();
            }
        }

        public void VisualMove(float frameLength)
        {
            _context.View.transform.localPosition = Vector3.Lerp(
                _context.View.transform.localPosition, _currentPosition, FieldController.MoveLerpCoeff);
        }

        public void Stop()
        {
            _hasReachedTarget = true;
        }

        public void ShowSingleHit()
        {
            _hasPlayedSplash = true;
        }

        public void ShowSplash(float damageRadius)
        {
            _context.View.transform.localScale = Vector3.one * damageRadius;

            Task.Run(async () =>
            {
                await Task.Delay(300);
                _hasPlayedSplash = true;
            });
        }
        
        public void LoadState(in PlayerState.ProjectileState projectileState)
        {
            _currentPosition = new Vector3(projectileState.PositionX, projectileState.PositionY);
        }
        
        public PlayerState.ProjectileState GetProjectileState()
        {
            return new PlayerState.ProjectileState(_context.Id, _context.SpawnTowerId, _context.TargetId,
                _context.AttackIndex, CurrentPosition.x, CurrentPosition.y, _context.Speed,
                _context.HasSplashDamage);
        }
    }
}