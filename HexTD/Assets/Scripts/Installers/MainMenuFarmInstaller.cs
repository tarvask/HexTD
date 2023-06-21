using FarmDataBase;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "FarmDataBase", menuName = "Installers/FarmDataBase")]
    public class MainMenuFarmInstaller : ScriptableObjectInstaller<MainMenuFarmInstaller>
    {
        [SerializeField] private FarmDataBase.FarmDataBase.Settings farmDataBaseSettings;

        public override void InstallBindings()
        {
            Container.Bind<FarmSettings>().FromInstance(farmDataBaseSettings.FarmSettings).AsSingle();
        }
    }
}