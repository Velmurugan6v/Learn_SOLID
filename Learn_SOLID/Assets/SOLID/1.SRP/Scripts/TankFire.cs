using System;
using UnityEngine;

public class TankFire : MonoBehaviour,IUpdateObserver
{
    public MonoBehaviour projectTileComponent;
    private IProjectile projectile;

    public Transform firePoint;

    private IFireMode fireMode;


    //----Methods----

    private void OnEnable()
    {
        UpdataManager.RegisterObserver(this);
    }

    private void OnDisable()
    {
        UpdataManager.UnregisterObserver(this);
    }

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

    private void Fire()
    {
        if (projectile != null)
            projectile.Fire(firePoint);
    }


    public void ObservedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Fire();


        //Fire Mode
        if (Input.GetKeyDown(KeyCode.Alpha1))
            fireMode = new NormalFIreMode();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            fireMode = new BurstFireMode();
    }
}
