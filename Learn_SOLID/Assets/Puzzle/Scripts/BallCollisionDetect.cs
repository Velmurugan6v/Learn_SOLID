using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Puzzle.Scripts
{
    public class BallCollisionDetect : MonoBehaviour
    {
        public GameObject gameWonPanel;

        private void Start()
        {
            gameWonPanel.SetActive(false);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                gameWonPanel.SetActive(true);
            }

            if (other.gameObject.CompareTag("Respawn"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}