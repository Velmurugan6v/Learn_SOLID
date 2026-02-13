using System;
using UnityEngine;



public class HeavyShelProjecttile : MonoBehaviour,IProjectile,IPoolable
{
    public GameObject shellPrefab;
    public float destroyDelayTime;
    

    public void OnSpawn()
    {
        print("Heavy Shel projectile spawned");
    }

    public void OnDespawn()
    {
        print("Heavy Shel projectile destroyed");
    }
    
    public void Fire(Transform firePoint)
    {
        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
        shell.GetComponent<Rigidbody2D>().AddForce(shell.transform.up * 10, ForceMode2D.Impulse);
    }
}
