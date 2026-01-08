using UnityEngine;

public class LargeTankEngine : TankEngine
{

    public override void StartEngine()
    {
        tankMovement.enabled = true;
        tankRotation.enabled = true;
        tankFire.enabled = true;
    }

    public override void StopEngine()
    {
        tankMovement.enabled = false;
        tankRotation.enabled = false;
        tankFire.enabled = false;
    }
}
