using System;
using BuffLogic;
using ExitGames.Client.Photon;
using Match.Field.State;
using Match.Serialization;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field.Castle
{
    [Serializable]
    public class CastleController : BaseDisposable, ISerializableToNetwork
    {
        public struct Context
        {
            public int Health { get; }
            public ReactiveCommand<int> AttackCastleByMobReactiveCommand { get; }
            public ReactiveCommand CastleDestroyedReactiveCommand { get; }

            public Context(int health,
                ReactiveCommand<int> attackCastleByMobReactiveCommand,
                ReactiveCommand castleDestroyedReactiveCommand)
            {
                Health = health;
                AttackCastleByMobReactiveCommand = attackCastleByMobReactiveCommand;
                CastleDestroyedReactiveCommand = castleDestroyedReactiveCommand;
            }
        }

        private readonly Context _context;

        private readonly ReactiveProperty<int> _castleHealthReactiveProperty;
        private readonly ReactiveProperty<int> _castleMaxHealthReactiveProperty;

        public IReadOnlyReactiveProperty<int> CastleHealthReactiveProperty => _castleHealthReactiveProperty;
        public IReadOnlyReactiveProperty<int> CastleMaxHealthReactiveProperty => _castleMaxHealthReactiveProperty;

        public CastleController(Context context)
        {
            _context = context;

            _castleHealthReactiveProperty = AddDisposable(new ReactiveProperty<int>(_context.Health));
            _castleMaxHealthReactiveProperty = AddDisposable(new ReactiveProperty<int>(_context.Health));

            _context.AttackCastleByMobReactiveCommand.Subscribe(Hurt);
        }

        private void Hurt(int damage)
        {
            _castleHealthReactiveProperty.Value = Mathf.Clamp(_castleHealthReactiveProperty.Value - damage,
                0, _castleMaxHealthReactiveProperty.Value);

            if (_castleHealthReactiveProperty.Value <= 0)
                Die();
        }

        private void Die()
        {
            _context.CastleDestroyedReactiveCommand.Execute();
            Dispose();
        }

        public PlayerState.CastleState GetCastleState()
        {
            return new PlayerState.CastleState(_castleHealthReactiveProperty.Value, CastleMaxHealthReactiveProperty.Value);
        }

        public void LoadState(in PlayerState.CastleState castleState)
        {
            _castleHealthReactiveProperty.Value = castleState.CurrentHealth;
            _castleMaxHealthReactiveProperty.Value = castleState.MaxHealth;
        }
        
        public Hashtable ToNetwork()
        {
            return PlayerState.CastleState.CastleToHashtable(GetCastleState());
        }
        
        public static object FromNetwork(Hashtable hashtable)
        {
            return PlayerState.CastleState.CastleFromHashtable(hashtable);
        }
    }
}