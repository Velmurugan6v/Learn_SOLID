using UnityEngine;

public class LightTankMovement : TankMovement, IMoveForward, IMoveBackward, IMoveLeft, IMoveRight
{
    public float rotationSpeed;

    public void MoveBackward(float inputValue)
    {
        tankObject.Translate(Vector2.up * inputValue * moveSpeed * Time.deltaTime);
    }

    public void MoveForward(float inputValue)
    {
        tankObject.Translate(Vector2.up * inputValue * moveSpeed * Time.deltaTime);
    }

    public void MoveLeft(float inputValue)
    {
        tankObject.Rotate(Vector3.forward * inputValue * rotationSpeed * Time.deltaTime);
    }

    public void MoveRight(float inputValue)
    {
        tankObject.Rotate(Vector3.forward * inputValue * rotationSpeed * Time.deltaTime);
    }
}
