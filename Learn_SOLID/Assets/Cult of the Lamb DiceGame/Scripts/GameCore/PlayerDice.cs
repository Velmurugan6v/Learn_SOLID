using UnityEngine;

namespace CultOfTheLamb_DiceGame
{
    public class PlayerDice : IDice
    {
        public int RollDice()
        {
            int diceRoll = Random.Range(1, 7);
            Debug.Log($"Roll Dice : {diceRoll}");

            return diceRoll;
        }
    }
}