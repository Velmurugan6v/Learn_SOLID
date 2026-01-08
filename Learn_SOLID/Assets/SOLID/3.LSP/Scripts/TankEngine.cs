using UnityEngine;

public class TankEngine : MonoBehaviour
{
    [SerializeField] protected TankMovement tankMovement;
    [SerializeField] protected TankRotation tankRotation;
    [SerializeField] protected TankFire tankFire;

    public virtual void StartEngine()
    {

    }

    public virtual void StopEngine()
    {
        
    }
}
