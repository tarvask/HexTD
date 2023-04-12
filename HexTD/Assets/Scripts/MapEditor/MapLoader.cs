using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Keiwando.NFSO;
using Newtonsoft.Json;
using Unity.VisualScripting;

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
        
        public async UniTask<LevelMapModel> Load()
        {
            LevelMapModel levelMapModel = null;
            
            NativeFileSO.shared.OpenFile(FileTypes, delegate (bool wasFileOpened, OpenedFile file) {
                if (wasFileOpened)
                {
                    levelMapModel = _serializer.Deserialize<LevelMapModel>(
                        new JsonTextReader(new StringReader(file.ToUTF8String())));
                }
                else
                {
                    if (levelMapModel == null)
                        throw new FileLoadException("Saved map didn't load");
                }
            });

            while (levelMapModel == null)
            {
                await Task.Delay(100);
            }

            return levelMapModel;
        }
    }
}