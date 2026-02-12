using UnityEngine;

namespace TankGame.PlayerTank
{
    public class TankFire : MonoBehaviour
    {
        public Transform firePoint;
        public MonoBehaviour projectTilePrefab;
        private IProjectTile m_projectTile;

        //FIreMode
        private IFireMode fireMode;


        void Start()
        {
            m_projectTile = projectTilePrefab as IProjectTile;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Fire();
        }

        void Fire()
        {
            if (m_projectTile != null)
            {
                m_projectTile.Fire(firePoint);
            }
        }
    }
}
