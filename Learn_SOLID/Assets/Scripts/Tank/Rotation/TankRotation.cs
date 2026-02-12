using System;
using UnityEngine;

namespace TankGame.PlayerTank
{
    public class TankRotation : MonoBehaviour
    {
        public Transform rotateObject;
        public float rotationSpeed;

        void Update()
        {
            Aim();
        }

        void Aim()
        {
            Vector3 mouseInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseInput - rotateObject.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            rotateObject.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
