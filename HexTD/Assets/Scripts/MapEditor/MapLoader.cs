using System.IO;
using Keiwando.NFSO;
using Newtonsoft.Json;

namespace MapEditor
{
    public class MapLoader
    {		
        public const string DefaultFileName = "LevelMap.txt";

        public static readonly SupportedFileType LevelMapFileType = new SupportedFileType {

            Name = "Level map",
            Extension = "txt",//"jpg|jpeg",
            Owner = false,
            MimeType = "Level map qwe"
        };
		
        public static readonly SupportedFileType[] FileTypes = {
            LevelMapFileType
        };
        
        private readonly JsonSerializer _serializer;

        public MapLoader()
        {			
            _serializer = new JsonSerializer();
            _serializer.NullValueHandling = NullValueHandling.Ignore;
        }
        
        public MapLoader(JsonSerializer serializer)
        {			
            _serializer = serializer;
        }
        
        public LevelMapModel Load()
        {
            LevelMapModel levelMapModel = null;
            NativeFileSO.shared.OpenFile(FileTypes, delegate (bool wasFileOpened, OpenedFile file) {
                if (wasFileOpened)
                {
                    levelMapModel = _serializer.Deserialize<LevelMapModel>(
                        new JsonTextReader(new StringReader(file.ToUTF8String())));
                }
            });

            if (levelMapModel == null)
                throw new FileLoadException("Saved map didn't load");

            return levelMapModel;
        }
    }
}