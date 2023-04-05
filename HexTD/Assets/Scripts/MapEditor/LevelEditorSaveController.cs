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
		private static readonly string FileName = "LevelMap.txt";

		private static readonly SupportedFileType LevelMapFileType = new SupportedFileType {

			Name = "Level map",
			Extension = "txt",//"jpg|jpeg",
			Owner = false,
			MimeType = "Level map qwe"
		};
		
		private static readonly SupportedFileType[] FileTypes = {
			LevelMapFileType
		};
		
		private readonly HexGridModel _hexGridModel;
		private readonly EditorPathContainer _pathContainer;
		private readonly PathEditorController _pathEditorController;

		private JsonSerializer serializer;
		
		public LevelEditorSaveController(HexGridModel hexGridModel,
			EditorPathContainer pathContainer,
			PathEditorController pathEditorController)
		{
			_hexGridModel = hexGridModel;
			_pathContainer = pathContainer;
			_pathEditorController = pathEditorController;
			
			serializer = new JsonSerializer();
			serializer.NullValueHandling = NullValueHandling.Ignore;
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
				serializer.Serialize(writer, levelMapModel);
			}
			
			var fileToSave = new FileToSave(GetTempFilePath(), FileName, LevelMapFileType);
			
			NativeFileSO.shared.SaveFile(fileToSave);
			
			DeleteTempFile();
		}
		
		public void Load()
		{
			NativeFileSO.shared.OpenFile(FileTypes, delegate (bool wasFileOpened, OpenedFile file) {
				if (wasFileOpened) {
					ProcessLoadedFile(file);
				}
			});
		}
		
		private static void DeleteTempFile() => 
			File.Delete(GetTempFilePath());

		private static string GetTempFilePath()=>
			Path.Combine(Application.persistentDataPath, FileName);
		

		private void ProcessLoadedFile(OpenedFile file)
		{
			var levelMapModel = serializer.Deserialize<LevelMapModel>(
				new JsonTextReader(new StringReader(file.ToUTF8String())));

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