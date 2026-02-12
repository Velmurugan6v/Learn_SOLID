using UnityEngine;
using TankGame.PlayerTank;

namespace TankGame.Core
{
    public class TankSelector : MonoBehaviour
    {
        public TankEngine smallTankEngine;
        public TankEngine mediumTankEngine;

        void Start()
        {
            smallTankEngine.StopEngine();
            mediumTankEngine.StopEngine();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                TurnOffAllEngine();
                smallTankEngine.StartEngine();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                TurnOffAllEngine();
                mediumTankEngine.StartEngine();
            }
        }

        public void TurnOffAllEngine()
        {
            smallTankEngine.StopEngine();
            mediumTankEngine.StopEngine();
        }
    }
}
