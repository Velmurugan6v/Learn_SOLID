using UnityEngine;

namespace TankGame
{
    public class RedProjectTile : MonoBehaviour, IProjectTile
    {
        public GameObject shellPrefab;
        public float velocity;

        public void Fire(Transform firePoint)
        {
            GameObject shellBullet = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
            shellBullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * velocity, ForceMode2D.Impulse);
        }

    }
}
