using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class WeaponPickup : MonoBehaviour
    {
        public GameObject weaponPrefab;

        void OnEnable()
        {
            PlayerDeath.OnExecute += OnPlayerDeath;
        }

        void OnDisable()
        {
            PlayerDeath.OnExecute -= OnPlayerDeath;
        }

        void OnPlayerDeath(PlayerDeath ev)
        {
            var model = Simulation.GetModel<PlatformerModel>();
            // Quand le joueur meurt (HP à 0), on réaffiche le pickup au sol
            if (!model.player.health.IsAlive)
            {
                if (GetComponent<Renderer>() != null) GetComponent<Renderer>().enabled = true;
                if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = true;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                EquipWeapon(player);
            }
        }

        void EquipWeapon(PlayerController player)
        {
            // Si le joueur a déjà une arme, on la détruit
            if (player.equippedWeapon != null)
            {
                Destroy(player.equippedWeapon.gameObject);
            }

            // Instancier l'arme et l'attacher au joueur
            GameObject weaponObj = Instantiate(weaponPrefab, player.transform);
            Weapon weapon = weaponObj.GetComponent<Weapon>();
            
            // S'assurer que l'arme s'affiche devant le joueur
            SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
            SpriteRenderer weaponRenderer = weaponObj.GetComponent<SpriteRenderer>();
            if (playerRenderer != null && weaponRenderer != null)
            {
                weaponRenderer.sortingLayerID = playerRenderer.sortingLayerID;
                weaponRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
            }

            // Positionner l'arme
            weaponObj.transform.localPosition = weapon.gripOffset; 
            
            // Enregistrer l'arme dans le PlayerController
            player.equippedWeapon = weapon;

            // Au lieu de désactiver le GameObject, on cache juste le sprite et on coupe le collider
            if (GetComponent<Renderer>() != null) GetComponent<Renderer>().enabled = false;
            if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        }
    }
}
