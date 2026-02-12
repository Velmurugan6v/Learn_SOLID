using UnityEngine;

namespace TankGame.PlayerTank
{
    public class TankMovement : MonoBehaviour
    {
        //Fields
        private float m_verticalMoveImput = 0f;
        [SerializeField]
        protected private Transform tankObject;

        [SerializeField]
        protected float m_tankMoveSpeed;

        IMoveForward moveForward;
        IMoveBackward moveBackward;

        void Start()
        {
            SetUpMovement();
        }

        void SetUpMovement()
        {
            moveForward = this as IMoveForward;
            moveBackward = this as IMoveBackward;
        }


        //Moethods
        void Update()
        {
            HandImput();
            Move();
        }

        //This function Get Player Input
        private void HandImput()
        {
            m_verticalMoveImput = Input.GetAxis("Vertical");
        }

        //This function will move Tank by Player given input 
        private void Move()
        {
            if (m_verticalMoveImput > 0)
            {
                if (moveForward != null)
                    moveForward.MoveForward(m_verticalMoveImput);
            }
            else if (m_verticalMoveImput < 0)
            {
                if (moveBackward != null)
                    moveBackward.MoveBackward(m_verticalMoveImput);
            }

        }
    }
}
