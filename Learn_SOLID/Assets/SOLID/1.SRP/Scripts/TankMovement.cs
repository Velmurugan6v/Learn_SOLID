using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public Transform tankObject;
    public float moveSpeed = 5f;

    private IMoveForward moveForward;
    private IMoveBackward moveBackward;
    private IMoveRight moveRight;
    private IMoveLeft moveLeft;

    void Start()
    {
        moveForward = this as IMoveForward;
        moveBackward = this as IMoveBackward;
        moveRight = this as IMoveRight;
        moveLeft = this as IMoveLeft;
    }


    void Update()
    {
        Move();
    }

    public void Move()
    {
        float moveInput = Input.GetAxis("Vertical");
        float moveInputX = Input.GetAxis("Horizontal");

        if (moveInput > 0)
        {
            if (moveForward != null)
                moveForward.MoveForward(moveInput);
        }
        else if (moveInput < 0)
        {
            if (moveBackward != null)
                moveBackward.MoveBackward(moveInput);
        }

        if (moveInputX > 0)
        {
            if (moveRight != null)
                moveRight.MoveRight(moveInputX);
        }
        else if (moveInputX < 0)
        {
            if (moveLeft != null)
                moveLeft.MoveLeft(moveInputX);
        }
    }
}
