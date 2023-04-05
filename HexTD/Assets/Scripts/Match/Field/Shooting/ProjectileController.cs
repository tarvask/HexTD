using System.Threading.Tasks;
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
            public float Speed { get; }
            public int SpawnTowerId { get; }
            public int TargetId { get; }
            public bool HasSplashDamage { get; }
            public float SplashDamageRadius { get; }
            public bool HasProgressiveSplash { get; }

            public Context(int id, ProjectileView view, float speed, bool hasSplashDamage, float splashDamageRadius, bool hasProgressiveSplash,
                int spawnTowerId, int targetId)
            {
                Id = id;
                View = view;
                Speed = speed;
                HasSplashDamage = hasSplashDamage;
                SplashDamageRadius = splashDamageRadius;
                HasProgressiveSplash = hasProgressiveSplash;
                SpawnTowerId = spawnTowerId;
                TargetId = targetId;
            }
        }

        private readonly Context _context;
        private Vector3 _currentPosition;
        private Vector3 _currentTargetPosition;
        private bool _hasReachedTarget;
        private bool _hasPlayedSplash;

        public int Id => _context.Id;
        public int SpawnTowerId => _context.SpawnTowerId;
        public int TargetId => _context.TargetId;
        public bool HasSplashDamage => _context.HasSplashDamage;
        public float SplashDamageRadius => _context.SplashDamageRadius;
        public bool HasProgressiveSplash => _context.HasProgressiveSplash;
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

        public void Move(Vector3 targetPosition, float frameLength)
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

            _context.View.transform.localPosition = _currentPosition;
        }

        public void Stop()
        {
            _hasReachedTarget = true;
        }

        public void ShowSingleHit()
        {
            _hasPlayedSplash = true;
        }

        public void ShowSplash()
        {
            _context.View.ProjectileSprite.enabled = false;
            // 18 is size of default Knob sprite in pixels, 100 is resolution of pixels per unit
            float splashAdoptedScale = _context.SplashDamageRadius * 2 * 100 / 18;
            _context.View.SplashTransform.localScale = new Vector3(splashAdoptedScale, splashAdoptedScale);
            _context.View.SplashTransform.gameObject.SetActive(true);

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
                CurrentPosition.x, CurrentPosition.y, _context.Speed,
                _context.HasSplashDamage, _context.SplashDamageRadius, _context.HasProgressiveSplash);
        }
    }
}