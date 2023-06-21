using System.Net;
using Cysharp.Threading.Tasks;
using WindowSystem.Controller;
using Extensions;
using UI.MainMenuWindow;
using UniRx;
using UnityEngine;

namespace UI.AuthorizationWindow
{
    public class AuthorizationWindowController : LoadableWindowController<AuthorizationWindowView>
    {
        protected override void DoInitialize()
        {
            View.GuestButtonClick
                .Subscribe(EnterAsGuest)
                .AddTo(View);

            View.GooglePlayButtonClick
                .Subscribe(EnterWithGooglePlay)
                .AddTo(View);

            View.StartButtonClick
                .Subscribe(EnterAsRegisteredPlayer)
                .AddTo(View);
        }
        
        protected override UniTask DoShowAsync(bool animated = true)
        {
            bool isLoggedIn = false;

            PrepareNotLoggedInVariant(!isLoggedIn);
            PrepareLoggedInVariant(isLoggedIn);

            return new UniTask();
        }
        
        #region View stuff
        private void PrepareNotLoggedInVariant(bool isShown)
        {
            View.GuestButtonGo.SetActive(isShown);
            View.GooglePlayButtonGo.SetActive(isShown);
        }

        private void PrepareLoggedInVariant(bool isShown)
        {
            View.StartButtonGo.SetActive(isShown);
            View.GooglePlayIconGo.SetActive(isShown);
            View.GooglePlayName.gameObject.SetActive(isShown);
            View.GooglePlayName.text = "Player";
        }
        #endregion

        #region Actions for buttons
        private void EnterAsGuest()
        {
            WindowsManager.CloseAsync(this).Forget();
            WindowsManager.OpenAsync<MainMenuWindowController>();
        }

        private void EnterWithGooglePlay()
        {
            Application.OpenURL("http://back.greensfi.com:8081/oauth2/authorization/google");
        }

        private void EnterAsRegisteredPlayer()
        {
            WindowsManager.CloseAsync(this).Forget();
            WindowsManager.OpenAsync<MainMenuWindowController>();
        }
        #endregion
    }
}