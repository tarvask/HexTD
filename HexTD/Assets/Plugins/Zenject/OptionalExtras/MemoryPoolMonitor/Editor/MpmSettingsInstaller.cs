using Plugins.Zenject.OptionalExtras.MemoryPoolMonitor.Editor.DebugWindow;
using Plugins.Zenject.Source.Install;
using UnityEditor;
using UnityEngine;

namespace Plugins.Zenject.OptionalExtras.MemoryPoolMonitor.Editor
{
    [CreateAssetMenu(fileName = "MpmSettingsInstaller", menuName = "Installers/MpmSettingsInstaller")]
    public class MpmSettingsInstaller : ScriptableObjectInstaller<MpmSettingsInstaller>
    {
        public MpmView.Settings MpmView;
        public MpmView.Settings MpmViewDark;

        public override void InstallBindings()
        {
            Container.BindInstance(EditorGUIUtility.isProSkin ? MpmViewDark : MpmView);
        }
    }
}
