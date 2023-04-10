using UnityEditor;

namespace Logger.Editor
{
    public class LogSettingsWindow : EditorWindow
    {
        [MenuItem("Custom/Log Settings")]
        static void Open()
        {
            GetWindow<LogSettingsWindow>("Log Settings").Show();
        }

        private void OnGUI()
        {
            Log.LogLevel = (LogLevel)EditorGUILayout.EnumPopup("Log Level", Log.LogLevel);
            Log.SupportedTags = (LogTag)EditorGUILayout.EnumFlagsField("Supported Tags", Log.SupportedTags);
        }
    }
}