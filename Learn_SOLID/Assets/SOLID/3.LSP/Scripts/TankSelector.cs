using UnityEngine;

public class TankSelector : MonoBehaviour
{
    [SerializeField] TankEngine lightTankEngine;
    [SerializeField] TankEngine mediumTankEngine;
    [SerializeField] TankEngine largeTankEngine;

    void Start()
    {
        DisabbleAllTankEngine();
    }


    void Update()
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
