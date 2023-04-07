using System.IO;
using UnityEngine;

public static class BuildInfo
{
	public static int BundleVersionCode
	{
		get => int.TryParse(ReadFile("Bundle Version Code"), out var version) ? version : 1;
		set => WriteFile("Bundle Version Code", value.ToString());
	}

	public static string BranchName
	{
		get => ReadFile("Branch Name");
		set => WriteFile("Branch Name", value);
	}

	public static string Sha
	{
		get => ReadFile("Sha");
		set => WriteFile("Sha", value);
	}

	public static string ServerUrl
	{
		get => ReadFile("Server Url");
		set => WriteFile("Server Url", value);
	}

	public static string FullBuildVersion => Application.version;

	private static string ReadFile(string name)
	{
		return Resources.Load<TextAsset>($"Build Info/{name}")?.text;
	}

	private static void WriteFile(string name, string value)
	{
#if UNITY_EDITOR
		var file = Application.dataPath + $"/Resources/Build Info/{name}.txt";
		File.WriteAllText(file, value);
		UnityEditor.AssetDatabase.Refresh();
#else
            throw new System.NotSupportedException();
#endif
	}
}