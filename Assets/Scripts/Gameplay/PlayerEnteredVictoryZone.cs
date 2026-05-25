using System.Numerics;
using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{

    /// <summary>
    /// This event is triggered when the player character enters a trigger with a VictoryZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredVictoryZone"></typeparam>
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            model.player.controlEnabled = false;
            var rb = model.player.GetComponent<Rigidbody2D>();
            model.player.StartCoroutine(VictoryJump(rb));
        }

        private System.Collections.IEnumerator VictoryJump(Rigidbody2D rb)
        {
            while (true)
            {
                rb.linearVelocity = new UnityEngine.Vector2(0, 5f);
                model.player.animator.SetTrigger("victory");
                yield return new WaitForSeconds(0.8f);
            }
        }
    }
}