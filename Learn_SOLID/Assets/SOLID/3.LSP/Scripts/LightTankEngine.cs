using UnityEngine;

public class LightTankEngine : TankEngine
{
    public override void StartEngine()
    {
        base.StartEngine();
        tankMovement.enabled = true;
        tankRotation.enabled = true;
        tankFire.enabled = true;
    }

    public override void StopEngine()
    {
        base.StopEngine();
        tankMovement.enabled = false;
        tankRotation.enabled = false;
        tankFire.enabled = false;
    }
}
