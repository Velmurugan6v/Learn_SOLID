using UnityEngine;

public class MediumTankMovement : TankMovement, IMoveForward,IMoveBackward
{
    public void MoveBackward(float inputValue)
    {
        tankObject.Translate(Vector2.up * inputValue * moveSpeed * Time.deltaTime);
    }

    public void MoveForward(float inputValue)
    {
        tankObject.Translate(Vector2.up * inputValue * moveSpeed * Time.deltaTime);
    }
}
