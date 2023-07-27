using System.Collections.Generic;
using BuffLogic;
using ExitGames.Client.Photon;
using Tools;

namespace Match.Field.Shooting
{
    public class ProjectileContainer : BaseDisposable, ISerializableToNetwork
    {
        private readonly Dictionary<int, ProjectileController> _projectiles;
        
        public IReadOnlyDictionary<int, ProjectileController> Projectiles => _projectiles;

        public ProjectileContainer(int size)
        {
            _projectiles = new Dictionary<int, ProjectileController>(size);
        }

        public void Add(ProjectileController projectileController)
        {
            _projectiles.Add(projectileController.Id, projectileController);
        }
        
        public void Remove(int projectileId)
        {
            _projectiles.Remove(projectileId);
        }

        public void Clear() => _projectiles.Clear();
        
        public Hashtable ToNetwork()
        {
            Hashtable hashtable = new Hashtable();
            
            int i = 0;
            foreach (var projectile in _projectiles.Values)
            {
                hashtable.Add($"{PhotonEventsConstants.SyncState.PlayerState.MobsParam}{i++}",
                    projectile.ToNetwork());
            }

            return hashtable;
        }
    }
}