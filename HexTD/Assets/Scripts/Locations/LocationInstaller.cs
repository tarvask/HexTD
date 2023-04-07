using UnityEngine;
using Zenject;

namespace Locations
{
    [CreateAssetMenu(fileName = "Locations", menuName = "Installers/Locations")]
    public class LocationInstaller : ScriptableObjectInstaller<LocationInstaller>
    {
        [SerializeField] private LocationDB.Settings locationDB;

        public override void InstallBindings()
        {
            Container.Bind<LocationDB>().AsSingle().WithArguments(locationDB);
        }

//#if UNITY_EDITOR
//        [UnityEditor.MenuItem("Cradle/Settings/Locations")]
//        private static void Focus()
//        {
//            UnityEditor.Selection.activeObject = Resources.FindObjectsOfTypeAll<LocationInstaller>().First();
//        }
//#endif
    }
}