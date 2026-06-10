using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    public class Weapon : MonoBehaviour
    {
        public string weaponName = "Sword";
        public int damage = 1;
        public float attackRange = 0.8f;
        public float attackCooldown = 0.2f; 
        public Vector3 gripOffset = new Vector3(0.5f, 0, 0); 
        public Vector2 attackOffset = new Vector2(0.5f, 0); 
        public LayerMask enemyLayer; // À configurer dans l'inspecteur (choisir "Enemies" ou "Default")
        
        [Header("Bonus Drôle")]
        public float currentScaleMultiplier = 1f;
        public float growthPerKill = 0.5f;

        [Header("Audio")]
        public AudioClip hitAudio;
        private AudioSource audioSource;
        
        private float nextAttackTime = 0f;
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            
            // MÉTHODE RADICALE : On détruit tous les colliders de l'épée.
            // Sans collider, l'épée n'existe pas pour le système de trigger de victoire.
            // On utilise quand même OverlapCircle pour les dégâts, qui n'a pas besoin de collider sur l'objet lui-même.
            var colliders = GetComponents<Collider2D>();
            foreach (var col in colliders) Destroy(col);
            
            var childColliders = GetComponentsInChildren<Collider2D>();
            foreach (var col in childColliders) Destroy(col);

            if (enemyLayer == 0) enemyLayer = ~LayerMask.GetMask("Player");
        }

        public void Attack()
        {
            if (Time.time >= nextAttackTime)
            {
                Debug.Log("Attacking with " + weaponName);
                
                // On utilise TransformPoint pour que l'offset s'adapte automatiquement au Scale -1
                Vector3 attackPoint = transform.TransformPoint(new Vector3(attackOffset.x, attackOffset.y, 0));
                Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint, attackRange, enemyLayer);

                foreach (Collider2D col in hitObjects)
                {
                    // 1. Ignorer si c'est le joueur lui-même
                    if (col.gameObject.transform.IsChildOf(transform.root)) continue;

                    // 2. Ignorer les triggers (comme le CinemachineConfiner)
                    if (col.isTrigger) continue;

                    Debug.Log("Valid hit: " + col.name + " on layer " + col.gameObject.layer);

                    // Chercher le script EnemyController sur l'objet ou ses parents
                    var enemy = col.GetComponentInParent<EnemyController>();
                    if (enemy != null)
                    {
                        // Jouer le son d'impact de l'épée
                        if (hitAudio != null && audioSource != null) audioSource.PlayOneShot(hitAudio);

                        var health = enemy.GetComponent<Health>();
                        if (health == null) health = enemy.GetComponentInChildren<Health>();

                        if (health != null)
                        {
                            health.Decrement();
                            Debug.Log("SUCCESS! Hit " + enemy.name + ". Health: " + health.CurrentHP + "/" + health.maxHP);
                            if (!health.IsAlive) 
                            {
                                currentScaleMultiplier += growthPerKill;
                                var ev = Schedule<EnemyDeath>();
                                ev.enemy = enemy;
                                ev.playAudio = false; // L'épée fait déjà son son
                            }
                        }
                        else
                        {
                            // Si pas de script Health, on tue l'ennemi directement
                            Debug.Log("No Health component found, killing " + enemy.name + " instantly!");
                            currentScaleMultiplier += growthPerKill;
                            var ev = Schedule<EnemyDeath>();
                            ev.enemy = enemy;
                            ev.playAudio = false; // L'épée fait déjà son son
                        }
                    }
                    else
                    {
                        Debug.Log("Hit " + col.name + " but it has no EnemyController component.");
                    }
                }

                nextAttackTime = Time.time + attackCooldown;
                StartCoroutine(AttackAnimation());
            }
        }

        IEnumerator AttackAnimation()
        {
            // On inverse l'angle si on regarde à gauche (Scale.x négatif)
            float angle = (transform.lossyScale.x < 0) ? 45f : -45f;
            
            Quaternion startRotation = transform.localRotation;
            transform.Rotate(0, 0, angle); 
            yield return new WaitForSeconds(0.1f);
            transform.localRotation = startRotation;
        }

        // Pour voir la zone d'attaque dans l'éditeur (pratique pour régler !)
        void OnDrawGizmosSelected()
        {
            Vector3 attackPoint = transform.TransformPoint(new Vector3(attackOffset.x, attackOffset.y, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint, attackRange);
        }
    }
}
