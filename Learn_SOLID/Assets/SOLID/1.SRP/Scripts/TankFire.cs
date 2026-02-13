using System;
using Generics;
using UnityEngine;

public class TankFire : MonoBehaviour
{
    public MonoBehaviour projectTileComponent;
    
    private IProjectile _projectile;

    public Transform firePoint;

    private IFireMode _fireMode;
    


    //----Methods----

    void Start()
    {
        _fireMode = new NormalFIreMode();

        SetUpProjectTile();
    }
    

    private void SetUpProjectTile()
    {
        _projectile = projectTileComponent as IProjectile;

        if (_projectile == null)
            Debug.Log("ProjectTile is empty");
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Fire();


        //Fire Mode
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _fireMode = new NormalFIreMode();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _fireMode = new BurstFireMode();

    }

    private void Fire()
    {
        if (_projectile != null)
            _projectile.Fire(firePoint);
    }


}
