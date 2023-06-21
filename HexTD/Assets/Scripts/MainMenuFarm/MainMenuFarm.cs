using MatchStarter;
using Tools;
using Zenject;

namespace MainMenuFarm
{
    public class MainMenuFarm : BaseMonoBehaviour
    {
        public bool IsLoaded { get; private set; }

        private void Start()
        {
            IsLoaded = true;
        }
    }
}