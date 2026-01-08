using UnityEngine;

public class BurstFireMode : IFireMode
{
    public void Fire(Transform firePoint, GameObject bulletPrefab)
    {
        float[] angles = { -15f, 0f, 15f };

        foreach (float angle in angles)
        {
            Quaternion rotationForBullet = firePoint.rotation * Quaternion.Euler(0, 0, angle);
            GameObject bullet = GameObject.Instantiate(bulletPrefab, firePoint.position, rotationForBullet);
            bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * 10, ForceMode2D.Impulse);
        }
    }
}
