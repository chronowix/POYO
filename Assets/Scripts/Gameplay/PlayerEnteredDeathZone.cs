using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when a player enters a trigger with a DeathZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredDeathZone"></typeparam>
    public class PlayerEnteredDeathZone : Simulation.Event<PlayerEnteredDeathZone>
    {
        public DeathZone deathzone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            player.health.Die();
            model.virtualCamera.Follow = null;
            model.virtualCamera.LookAt = null;
            player.controlEnabled = false;
            player.animator.SetTrigger("hurt");
            player.animator.SetBool("dead", true);
            var spawnEvent = Simulation.Schedule<PlayerSpawn>(2);
            spawnEvent.isDeath = true;
        }
    }
}