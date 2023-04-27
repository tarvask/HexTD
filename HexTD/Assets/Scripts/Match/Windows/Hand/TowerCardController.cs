using Match.Field.Hand;
using Match.Field.Tower;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Windows.Hand
{
    public class TowerCardController : BaseDisposable
    {
        public struct Context
        {
            public TowerCardView View { get; }
            public PlayerHandController PlayerHandController { get; }
            public ReactiveCommand<bool> DragCardChangeStatusCommand { get; }
            public TowerType TowerType { get; }

            public Context(TowerCardView towerCardView, PlayerHandController playerHandController, ReactiveCommand<bool> dragCardChangeStatusCommand, TowerType towerType)
            {
                View = towerCardView;
                PlayerHandController = playerHandController;
                DragCardChangeStatusCommand = dragCardChangeStatusCommand;
                TowerType = towerType;
            }
        }

        private readonly Context _context;

        public TowerCardController(Context context)
        {
            _context = context;

            Subscribe();
        }

        private void OnDrag(bool isDrag)
        {
            if (isDrag)
            {
                _context.PlayerHandController.SetChosenTower(_context.TowerType);
            }

            _context.DragCardChangeStatusCommand.Execute(isDrag);
        }

        private void Subscribe()
        {
            _context.View.OnDragEvent += OnDrag;
        }

        private void Unsubscribe()
        {
            _context.View.OnDragEvent -= OnDrag;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            Object.Destroy(_context.View);
        }
    }
}