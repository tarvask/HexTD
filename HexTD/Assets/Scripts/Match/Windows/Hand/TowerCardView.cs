using System;
using TMPro;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match.Windows.Hand
{
    public class TowerCardView : BaseMonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private Transform readyBgImage;
        [SerializeField] private Transform notReadyBgImage;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text towerNameText;
        
        public Transform ReadyBgImage => readyBgImage;
        public Transform NotReadyBgImage => notReadyBgImage;
        public Button Button => button;
        public TMP_Text CostText => costText;
        public TMP_Text TowerNameText => towerNameText;

        private bool _isDraged = false;
        private bool _isReady = false;

        private Vector3 _startPosition;

        public event Action<bool> OnDragEvent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isReady) return;
 
            _isDraged = true;
            _startPosition = transform.position;
            OnDragEvent?.Invoke(_isDraged);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDraged && _isReady)
            {
                transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDraged && !_isReady) return;

            _isDraged = false;
            transform.position = _startPosition;
            OnDragEvent?.Invoke(_isDraged);
        }

        public void SetTowerCardReadyState(bool isReady)
        {
            _isReady = isReady;
        }
    }
}