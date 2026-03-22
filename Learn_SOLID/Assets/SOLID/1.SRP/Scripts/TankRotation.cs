using System;
using UnityEngine;

public class TankRotation : MonoBehaviour, IUpdateObserver
{
    public Camera camera;
    public Transform aimObject;
    public float rotateSpeed;

    private void OnEnable()
    {
        UpdataManager.RegisterObserver(this);
    }

    private void OnDisable()
    {
        UpdataManager.UnregisterObserver(this);
    }

    public void ObservedUpdate()
    {
        Aim();
    }


    private void Aim()
    {
        // Vector3 mouseInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 direction = mouseInput - transform.position;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        // aimObject.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - transform.position;

        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        aimObject.rotation = Quaternion.Lerp(
            aimObject.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }
}