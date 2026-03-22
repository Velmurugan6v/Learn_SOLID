using System;
using UnityEngine;

public class TankSelector : MonoBehaviour, IUpdateObserver
{
    [SerializeField] TankEngine lightTankEngine;
    [SerializeField] TankEngine mediumTankEngine;
    [SerializeField] TankEngine largeTankEngine;

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
        DisabbleAllTankEngine();
    }

    public void ObservedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DisabbleAllTankEngine();
            lightTankEngine.StartEngine();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            DisabbleAllTankEngine();
            mediumTankEngine.StartEngine();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            DisabbleAllTankEngine();
            largeTankEngine.StartEngine();
        }
    }


    public void DisabbleAllTankEngine()
    {
        lightTankEngine.StopEngine();
        mediumTankEngine.StopEngine();
        largeTankEngine.StopEngine();
    }
}