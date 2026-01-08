using UnityEngine;

public class NormalFIreMode : IFireMode
{
    public void Fire(Transform firePoint, GameObject bulletPrefab)
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * 10, ForceMode2D.Impulse);
    }
}
