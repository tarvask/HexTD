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
		
		private readonly EditorHexesModel _editorHexesModel;
		private readonly EditorPropsModel _editorPropsModel;
		private readonly EditorPathContainer _pathContainer;
		private readonly PathEditorController _pathEditorController;

		
		public LevelEditorSaveController(
			EditorHexesModel editorHexesModel,
			EditorPropsModel editorPropsModel,
			EditorPathContainer pathContainer,
			PathEditorController pathEditorController)
		{
			_editorHexesModel = editorHexesModel;
			_editorPropsModel = editorPropsModel;
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
				HexModels = _editorHexesModel.GetAllHexes(),
				PropsModels = _editorPropsModel.GetAllProps(),
				PathDatas = _pathContainer.GetPathsForSave()
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
		
		public async void Load()
		{
			LevelMapModel levelMapModel = await _mapLoader.Load();
			ApplyLevelMapModel(levelMapModel);
		}

		private void ApplyLevelMapModel(LevelMapModel levelMapModel)
		{
			_editorHexesModel.Clear();
			foreach (HexModel hexModel in levelMapModel.HexModels)
			{
				_editorHexesModel.CreateHex(hexModel);
			}
			
			foreach (PropsModel propsModel in levelMapModel.PropsModels)
			{
				_editorPropsModel.CreateProps(propsModel);
			}
			
			_pathEditorController.Clear();
			foreach (var path in levelMapModel.PathDatas)
			{
				_pathEditorController.AddPath(path);
			}
		}
	}
}