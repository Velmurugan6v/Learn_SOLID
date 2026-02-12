using UnityEngine;
using UnityEngine.UI;

namespace TankGame
{
    public class TankHealthSystem : MonoBehaviour, IDamageable
    {
        public int health;
        public Image healthProgressBar;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(25);
            }
        }

        public void TakeDamage(int damageAmount)
        {
            health -= damageAmount;

            CheckStatus();
        }

        public void CheckStatus()
        {
            if (health <= 0)
            {
                DestroyTank();
            }

            float currentProgressValue = health / 100;

            if (currentProgressValue <= 0)
                currentProgressValue = 0;

            healthProgressBar.fillAmount = currentProgressValue;
        }

        public void DestroyTank()
        {
            this.gameObject.SetActive(false);
        }
    }
}
