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
        
        private bool victoryMusicPlayed = false;

        public override void Execute()
        {
            if (victoryMusicPlayed) return;
            victoryMusicPlayed = true;
            
            model.player.controlEnabled = false;
            model.player.animator.SetTrigger("victory");

            var audioSource = GameObject.FindFirstObjectByType<GameController>()?.GetComponent<AudioSource>();
            if (audioSource != null && model.player.victoryAudio != null)
            {
                audioSource.Stop();
                audioSource.clip = model.player.victoryAudio;
                audioSource.loop = true;
                audioSource.Play();
            }

            var rb = model.player.GetComponent<Rigidbody2D>();
            model.player.StartCoroutine(VictoryJump(rb));
        }

        private System.Collections.IEnumerator VictoryJump(Rigidbody2D rb)
        {
            while (true)
            {
                rb.linearVelocity = new UnityEngine.Vector2(0, 4f);
                yield return new WaitForSeconds(1.2f);
            }
        }
    }
}