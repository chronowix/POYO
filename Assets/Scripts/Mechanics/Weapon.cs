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
        public LayerMask enemyLayer;

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

                Vector3 attackPoint = transform.TransformPoint(new Vector3(attackOffset.x, attackOffset.y, 0));
                Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint, attackRange, enemyLayer);

                foreach (Collider2D col in hitObjects)
                {
                    if (col.gameObject.transform.IsChildOf(transform.root)) continue;
                    if (col.isTrigger) continue;

                    var enemy = col.GetComponentInParent<EnemyController>();
                    if (enemy != null)
                    {
                        if (hitAudio != null && audioSource != null)
                            audioSource.PlayOneShot(hitAudio);

                        var health = enemy.GetComponent<Health>();
                        if (health == null) health = enemy.GetComponentInChildren<Health>();

                        if (health != null)
                        {
                            health.Decrement();
                            if (!health.IsAlive)
                            {
                                currentScaleMultiplier += growthPerKill;
                                var ev = Schedule<EnemyDeath>();
                                ev.enemy = enemy;
                                ev.playAudio = true;
                            }
                        }
                        else
                        {
                            currentScaleMultiplier += growthPerKill;
                            var ev = Schedule<EnemyDeath>();
                            ev.enemy = enemy;
                            ev.playAudio = true;
                        }
                    }
                }

                nextAttackTime = Time.time + attackCooldown;
                StartCoroutine(AttackAnimation());
            }
        }

        IEnumerator AttackAnimation()
        {
            float angle = (transform.lossyScale.x < 0) ? 45f : -45f;
            Quaternion startRotation = transform.localRotation;
            transform.Rotate(0, 0, angle);
            yield return new WaitForSeconds(0.1f);
            transform.localRotation = startRotation;
        }

        void OnDrawGizmosSelected()
        {
            Vector3 attackPoint = transform.TransformPoint(new Vector3(attackOffset.x, attackOffset.y, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint, attackRange);
        }
    }
}