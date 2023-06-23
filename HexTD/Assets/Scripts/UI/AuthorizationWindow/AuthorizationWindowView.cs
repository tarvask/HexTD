using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.AuthorizationWindow
{
    public class AuthorizationWindowView : WindowViewBase
    {
        [SerializeField] private Button guestButton;
        [SerializeField] private Button googlePlayButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Image googlePlayIcon;
        [SerializeField] private TextMeshProUGUI googlePlayName;

        public GameObject GuestButtonGo => guestButton.gameObject;
        public GameObject GooglePlayButtonGo => googlePlayButton.gameObject;
        public GameObject StartButtonGo => startButton.gameObject;
        
        public IObservable<Unit> GuestButtonClick => guestButton
            .OnClickAsObservable()
            .WhereAppeared(this);
        
        public IObservable<Unit> GooglePlayButtonClick => googlePlayButton
            .OnClickAsObservable()
            .WhereAppeared(this);
        
        public IObservable<Unit> StartButtonClick => startButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public GameObject GooglePlayIconGo => googlePlayIcon.gameObject;
        public TextMeshProUGUI GooglePlayName => googlePlayName;
    }
}