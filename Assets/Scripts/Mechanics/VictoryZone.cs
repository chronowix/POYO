using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponentInParent<PlayerController>();
            if (p != null)
            {
                // Si c'est l'arme (ou un de ses composants) qui touche, on ignore
                if (collider.gameObject.GetComponent<Weapon>() != null || 
                    collider.gameObject.GetComponentInParent<Weapon>() != null)
                {
                    return;
                }

                var ev = Schedule<PlayerEnteredVictoryZone>();
                ev.victoryZone = this;
            }
        }
    }
}