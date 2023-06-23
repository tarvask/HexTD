using Cysharp.Threading.Tasks;
using System;
using UniRx;

namespace MainMenuFarm
{
    public interface IMainMenuFarmLoader
    {
        bool IsLoading { get; }

        IObservable<Unit> LocationLoaded { get; }

        UniTask<MainMenuFarm> LoadAsync(bool autoComplete = true);

        void DestroyAndRelease();
    }
}