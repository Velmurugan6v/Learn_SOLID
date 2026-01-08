using UnityEngine;

public class TankRotation : MonoBehaviour
{
    public Transform aimObject;
    public float rotateSpeed;

    void Update()
    {
        Aim();
    }


    private void Aim()
    {
        // Vector3 mouseInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 direction = mouseInput - transform.position;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        // aimObject.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
