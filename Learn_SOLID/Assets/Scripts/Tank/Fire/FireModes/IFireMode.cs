using UnityEngine;

namespace TankGame
{
    public interface IFireMode : IProjectTile
    {
        public void FireMode(Transform firePoint);
    }
}
