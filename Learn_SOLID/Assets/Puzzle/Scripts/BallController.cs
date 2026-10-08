using System;
using UnityEngine;

namespace Puzzle.Scripts
{
    public class BallController : MonoBehaviour
    {
        //Field
        [SerializeField] private Rigidbody2D ballRb;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Vector2 moveForce;
        [SerializeField] private Transform airFlowIndicatorCircle;

        
        //Methods
        private void Update()
        {
            CalculateInput();
            UpdateAirFlowIndicatorCircle();
        }

        private void FixedUpdate()
        {
            AddAirFlowToBall();
        }


        private void CalculateInput()
        {
            Vector2 mousePosition = Input.mousePosition;

            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;

            float x = mousePosition.x - centerX;
            float y = mousePosition.y - centerY;


            Vector2 direction = Vector2.zero;

            // Right
            if (x > 0 && Mathf.Abs(x) > Mathf.Abs(y))
            {
                direction = Vector2.left;
            }

            // Left
            else if (x < 0 && Mathf.Abs(x) > Mathf.Abs(y))
            {
                direction = Vector2.right;
            }

            // Top
            else if (y > 0 && Mathf.Abs(y) > Mathf.Abs(x))
            {
                direction = Vector2.down;
            }

            // Down
            else if (y < 0 && Mathf.Abs(y) > Mathf.Abs(x))
            {
                direction = Vector2.up;
            }

            // Right Top
            else if (x > 0 && y > 0)
            {
                direction = new Vector2(-1, -1).normalized;
            }

            // Right Down
            else if (x > 0 && y < 0)
            {
                direction = new Vector2(-1, 1).normalized;
            }

            // Left Top
            else if (x < 0 && y > 0)
            {
                direction = new Vector2(1, -1).normalized;
            }

            // Left Down
            else if (x < 0 && y < 0)
            {
                direction = new Vector2(1, 1).normalized;
            }

            moveForce = direction * moveSpeed;
        }

        private void AddAirFlowToBall()
        {
            ballRb.AddForce(moveForce);
        }

        private void UpdateAirFlowIndicatorCircle()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - airFlowIndicatorCircle.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            airFlowIndicatorCircle.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}