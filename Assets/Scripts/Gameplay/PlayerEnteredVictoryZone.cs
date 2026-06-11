using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using Platformer.UI;

namespace Platformer.Gameplay
{
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            Debug.Log("VictoryZone triggered!");

            if (!model.player.controlEnabled) return;

            model.player.controlEnabled = false;

            var pauseManager = GameObject.FindFirstObjectByType<PauseManager>();
            if (pauseManager != null)
                pauseManager.ShowVictory();

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