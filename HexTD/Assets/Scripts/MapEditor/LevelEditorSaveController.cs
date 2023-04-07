using System.IO;
using HexSystem;
using Keiwando.NFSO;
using Newtonsoft.Json;
using PathSystem;
using Application = UnityEngine.Application;

namespace MapEditor
{
	public class LevelEditorSaveController
	{
		private readonly MapLoader _mapLoader;
		private readonly JsonSerializer _serializer;
		
		private readonly HexGridModel _hexGridModel;
		private readonly EditorPathContainer _pathContainer;
		private readonly PathEditorController _pathEditorController;

		
		public LevelEditorSaveController(HexGridModel hexGridModel,
			EditorPathContainer pathContainer,
			PathEditorController pathEditorController)
		{
			_hexGridModel = hexGridModel;
			_pathContainer = pathContainer;
			_pathEditorController = pathEditorController;

			_serializer = new JsonSerializer();
			_serializer.NullValueHandling = NullValueHandling.Ignore;
			
			_mapLoader = new MapLoader(_serializer);
		}

		public void Save()
		{
			LevelMapModel levelMapModel = new LevelMapModel()
			{
				HexModels = _hexGridModel.GetAllHexes(),
				PathDatas = _pathContainer.GetPaths()
			};
			
			using (StreamWriter sw = new StreamWriter(GetTempFilePath()))
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				_serializer.Serialize(writer, levelMapModel);
			}
			
			var fileToSave = new FileToSave(GetTempFilePath(), MapLoader.DefaultFileName, MapLoader.LevelMapFileType);
			
			NativeFileSO.shared.SaveFile(fileToSave);
			
			DeleteTempFile();
		}
		
		private static void DeleteTempFile() => 
			File.Delete(GetTempFilePath());

		private static string GetTempFilePath()=>
			Path.Combine(Application.persistentDataPath, MapLoader.DefaultFileName);
		
		public void Load()
		{
			LevelMapModel levelMapModel = _mapLoader.Load();
			ApplyLevelMapModel(levelMapModel);
		}

		private void ApplyLevelMapModel(LevelMapModel levelMapModel)
		{
			_hexGridModel.Clear();
			foreach (HexModel hexModel in levelMapModel.HexModels)
			{
				_hexGridModel.CreateHex(hexModel);
			}
			
			_pathContainer.Clear();
			foreach (var path in levelMapModel.PathDatas)
			{
				_pathEditorController.AddPath(path);
			}
		}
	}
}