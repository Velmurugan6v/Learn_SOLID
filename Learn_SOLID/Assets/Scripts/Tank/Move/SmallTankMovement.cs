using UnityEngine;

namespace TankGame.PlayerTank
{
    public class SmallTankMovement : TankMovement, IMoveForward, IMoveBackward
    {
        public void MoveBackward(float moveInput)
        {
            tankObject.Translate(Vector2.up * moveInput * m_tankMoveSpeed * Time.deltaTime);
        }

        public void MoveForward(float moveInput)
        {
            transform.Translate(Vector2.up * moveInput * m_tankMoveSpeed * Time.deltaTime);
        }
    }
}
