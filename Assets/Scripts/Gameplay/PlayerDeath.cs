using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDeath"></typeparam>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

public override void Execute()
        {
            var player = model.player;
            if (player.health.IsAlive)
            {
                player.health.Decrement();

                if (!player.health.IsAlive){
                    // si plus de hp - MORT
                    player.DestroyWeapon();
                    
                    if (player.audioSource && player.deathAudio) 
                        player.audioSource.PlayOneShot(player.deathAudio);

                    model.virtualCamera.Follow = null;
                    model.virtualCamera.LookAt = null;
                    player.controlEnabled = false;
                    player.animator.SetTrigger("hurt");
                    player.animator.SetBool("dead", true);
                    var spawnEvent = Simulation.Schedule<PlayerSpawn>(2);
                    spawnEvent.isDeath = true;
                }
                else{
                    // Toujours en vie - DEGATS UNIQUEMENT
                    if (player.audioSource && player.ouchAudio)
                        player.audioSource.PlayOneShot(player.ouchAudio);

                    player.animator.SetTrigger("hurt");
                    var hurtEvent = Simulation.Schedule<PlayerSpawn>(0.5f);
                    hurtEvent.isDeath = false;
                }
            }
        }
    }
}