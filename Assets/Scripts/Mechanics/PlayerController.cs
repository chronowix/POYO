using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.InputSystem;

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip ouchAudio;
        public AudioClip deathAudio;
        public AudioClip victoryAudio;

        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        private InputAction m_MoveAction;
        private InputAction m_JumpAction;
        private InputAction m_AttackAction;

        public Bounds Bounds => collider2d.bounds;

        private float afkTimer = 0f;
        public float afkDelay = 5f;
        private bool isAfk = false;

        public Weapon equippedWeapon;

        public float invincibilityDuration = 1.5f;
        private float invincibilityTimer = 0f;
        public bool IsInvincible => invincibilityTimer > 0;

        public void HideWeapon()
        {
            if (equippedWeapon != null) equippedWeapon.gameObject.SetActive(false);
        }

        public void ShowWeapon()
        {
            if (equippedWeapon != null) equippedWeapon.gameObject.SetActive(true);
        }

        public void DestroyWeapon()
        {
            if (equippedWeapon != null)
            {
                Destroy(equippedWeapon.gameObject);
                equippedWeapon = null;
            }
        }

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            m_MoveAction = InputSystem.actions.FindAction("Player/Move");
            m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
            m_AttackAction = InputSystem.actions.FindAction("Player/Attack");

            m_MoveAction.Enable();
            m_JumpAction.Enable();
            m_AttackAction.Enable();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = m_MoveAction.ReadValue<Vector2>().x;

                if (jumpState == JumpState.Grounded && m_JumpAction.WasPressedThisFrame())
                    jumpState = JumpState.PrepareToJump;
                else if (m_JumpAction.WasReleasedThisFrame())
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (m_AttackAction.WasPressedThisFrame() && equippedWeapon != null)
                {
                    equippedWeapon.Attack();
                }
            }
            else
            {
                move.x = 0;
            }

            // Timer AFK
            if (move.x == 0 && IsGrounded)
            {
                afkTimer += Time.deltaTime;
                if (afkTimer >= afkDelay && !isAfk)
                {
                    isAfk = true;
                    animator.SetBool("isAfk", true);
                }
            }
            else
            {
                afkTimer = 0f;
                isAfk = false;
                animator.SetBool("isAfk", false);
            }

            UpdateJumpState();

            // Gestion de l'invincibilité
            if (invincibilityTimer > 0)
            {
                invincibilityTimer -= Time.deltaTime;

                float blinkSpeed = 10f;
                float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1.0f);
                spriteRenderer.color = new Color(1, 1, 1, alpha > 0.5f ? 1f : 0.2f);

                if (invincibilityTimer <= 0)
                    spriteRenderer.color = Color.white;
            }

            base.Update();
        }

        public void StartInvincibility()
        {
            invincibilityTimer = invincibilityDuration;
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                    velocity.y = velocity.y * model.jumpDeceleration;
            }

            if (move.x > 0.01f)
            {
                spriteRenderer.flipX = false;
                if (equippedWeapon != null)
                {
                    equippedWeapon.transform.localPosition = equippedWeapon.gripOffset;
                    float s = equippedWeapon.currentScaleMultiplier;
                    equippedWeapon.transform.localScale = new Vector3(s, s, 1);
                }
            }
            else if (move.x < -0.01f)
            {
                spriteRenderer.flipX = true;
                if (equippedWeapon != null)
                {
                    Vector3 flippedOffset = equippedWeapon.gripOffset;
                    flippedOffset.x *= -1;
                    equippedWeapon.transform.localPosition = flippedOffset;
                    float s = equippedWeapon.currentScaleMultiplier;
                    equippedWeapon.transform.localScale = new Vector3(-s, s, 1);
                }
            }

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}