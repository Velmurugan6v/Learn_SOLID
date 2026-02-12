using UnityEngine;

namespace TankGame.PlayerTank
{
    public class TankEngine : MonoBehaviour
    {
        public TankMovement tankMovement;
        public TankRotation tankRotation;
        public TankFire tankFire;

        public EngineStatus currentEngineStatus;

        public virtual void StartEngine()
        {
            if (tankMovement != null)
                tankMovement.enabled = true;

            if (tankRotation != null)
                tankRotation.enabled = true;

            if (tankFire != null)
                tankFire.enabled = true;
        }

        public virtual void StopEngine()
        {
            if (tankMovement != null)
                tankMovement.enabled = false;

            if (tankRotation != null)
                tankRotation.enabled = false;

            if (tankFire != null)
                tankFire.enabled = false;
        }

        public void TankIsDead()
        {
            currentEngineStatus = EngineStatus.InOperatable;
            StopEngine();
        }

        public EngineStatus GetEngineStatus()
        {
            return currentEngineStatus;
        }
    }

    public enum EngineStatus
    {
        Operatable,
        InOperatable
    }
}
