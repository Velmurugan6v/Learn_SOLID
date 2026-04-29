using System;
using UnityEngine;

namespace Hollow_Knight_Like.Scripts
{
    public class Player_AnimationTrigger : MonoBehaviour
    {
        private Player player;

        private void Start()
        {
            player = GetComponentInParent<Player>();
        }

        private void AttackOver()
        {
            player.CallAnimationTrigger();
        }
    }
}