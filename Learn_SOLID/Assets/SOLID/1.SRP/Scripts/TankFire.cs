using System;
using UnityEngine;

public class TankFire : MonoBehaviour
{
    public MonoBehaviour projectTileComponent;
    private IProjectile projectile;

    public Transform firePoint;

    private IFireMode fireMode;


    //----Methods----

    void Start()
    {
        fireMode = new NormalFIreMode();

        SetUpProjectTile();
    }

    private void SetUpProjectTile()
    {
        projectile = projectTileComponent as IProjectile;

        if (projectile == null)
            Debug.Log("ProjectTile is empty");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Fire();


        //Fire Mode
        if (Input.GetKeyDown(KeyCode.Alpha1))
            fireMode = new NormalFIreMode();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            fireMode = new BurstFireMode();

    }

    private void Fire()
    {
        if (projectile != null)
            projectile.Fire(firePoint);
    }


}
