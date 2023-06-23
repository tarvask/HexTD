using DG.Tweening;
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
        [SerializeField] private Transform _readyBgImage;
        [SerializeField] private Transform _notReadyBgImage;
        //[SerializeField] private Transform _selectPosition;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private TMP_Text _towerNameText;
        [SerializeField] private Image _readyTowerImage;
        [SerializeField] private Image _notReadyTowerImage;

        public Transform ReadyBgImage => _readyBgImage;
        public Transform NotReadyBgImage => _notReadyBgImage;
        public Button Button => _button;
        public TMP_Text CostText => _costText;
        public TMP_Text TowerNameText => _towerNameText;
        public Image ReadyTowerImage => _readyTowerImage;
        public Image NotReadyTowerImage => _notReadyTowerImage;

        private bool _isDraged = false;
        private bool _isReady = false;

        //private Vector3 _startPosition;

        //private float _animationTime = 0.5f;

        //private Tweener _animationTweener;

        private Color _startColor;
        private Color _selectedColor;

        public event Action<bool> OnDragEvent;

        private void Awake()
        {
            _startColor = _readyBgImage.GetComponent<Image>().color;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isReady) return;
 
            _isDraged = true;

            _selectedColor = _startColor;
            _selectedColor = new Color(0.56f, 0.93f, 0.56f);
            _readyBgImage.GetComponent<Image>().color = _selectedColor;

            OnDragEvent?.Invoke(_isDraged);

            //if (_animationTweener != null)
            //{
            //    _animationTweener.Kill();
            //    transform.position = _startPosition;
            //}
            //_startPosition = transform.position;

            //CardSelectAnimation();
        }

        public void OnDrag(PointerEventData eventData)
        {
            //if (_isDraged && _isReady)
            //{
            //    transform.position = eventData.position;
            //}
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDraged && !_isReady) return;

            _isDraged = false;
            //transform.position = _startPosition;
            _readyBgImage.GetComponent<Image>().color = _startColor;
            OnDragEvent?.Invoke(_isDraged);
        }

        public void SetTowerCardReadyState(bool isReady)
        {
            _isReady = isReady;
        }

        //private void CardSelectAnimation()
        //{
        //    _animationTweener = _cardClone.transform.DOMove(_selectPosition.position, _animationTime).SetEase(Ease.Linear);
        //}
    }
}