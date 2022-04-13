using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData {

    public class ActionHandler : MonoBehaviour {
    
        Player player;
        PlayerController playerController;

        [Range(0.0f, 5.0f)]
        float slideTime = 0.0f;
        public float totalSlideTime = 6.0f;
        float vaultTime = 0.0f;
        public float totalVaultTime = 2.0f;


        /** Colliders **/
        public CapsuleCollider standingCollider;
        public CapsuleCollider slidingCollider;
        public CapsuleCollider vaultingCollider;

        /** States **/
        public enum ActionType {
            WALK_RUN,
            /* STRAFE, */
            JUMP,
            FALLING,
            SLIDE,
            WALLRUN,
            VAULTING
        }

        void Start() {
            playerController = GetComponent<PlayerController>();
            player = playerController.GetPlayer();
        }

        void FixedUpdate() {
            Animate();
        }

        public void Animate() {
            Animator animator = playerController.GetAnimator();

            float animationSpeedPercent = player.speed/playerController.GetWalkSpeed() * .5f;
            if(player.IsSprinting())
                animationSpeedPercent = player.speed/playerController.GetRunSpeed();
            Mathf.Clamp(animationSpeedPercent, 0.0f, 1.0f);

            switch(player.action) {
                case ActionType.WALK_RUN:
                    vaultTime = 0.0f;
                    slideTime = 0.0f;
                    animator.SetFloat("animSpeed", animationSpeedPercent, 0.01f, Time.deltaTime); // 0.01f is dampening/smoothing time  
                    animator.SetBool("jumping", false);
                    animator.SetBool("sliding", false);
                    animator.SetBool("wallrunning", false);
                    animator.SetBool("falling", false);
                    animator.SetBool("vaulting", false);
                    standingCollider.enabled = true;
                    slidingCollider.enabled = false;
                    vaultingCollider.enabled = false;

                    break;

                case ActionType.JUMP:
                    vaultTime = 0.0f;
                    slideTime = 0.0f;
                    animator.SetFloat("animSpeed", 1.0f);
                    animator.SetBool("jumping", true);
                    animator.SetBool("sliding", false);
                    animator.SetBool("wallrunning", false);
                    animator.SetBool("falling", false);
                    animator.SetBool("vaulting", false);
                    standingCollider.enabled = true;
                    slidingCollider.enabled = false;
                    vaultingCollider.enabled = false;

                    break;

                case ActionType.FALLING:
                    vaultTime = 0.0f;
                    slideTime = 0.0f;
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", true);
                    animator.SetBool("wallrunning", false);
                    animator.SetBool("sliding", false);
                    animator.SetBool("vaulting", false);
                    standingCollider.enabled = true;
                    slidingCollider.enabled = false;
                    vaultingCollider.enabled = false;

                    break;

                case ActionType.SLIDE:
                    vaultTime = 0.0f;
                    animator.SetFloat("slideTime", slideTime += 0.05f);
                    animator.SetBool("sliding", true);
                    animator.SetBool("wallrunning", false);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", false);
                    animator.SetBool("vaulting", false);

                    // No longer needed, figured out animator
                    if(slideTime >= totalSlideTime) {
                        animator.SetBool("sliding", false);
                        slideTime = 0.0f;
                        player.action = ActionType.WALK_RUN;
                    }
                    standingCollider.enabled = false;
                    slidingCollider.enabled = true;
                    vaultingCollider.enabled = false;

                    break;

                case ActionType.WALLRUN:
                    vaultTime = 0.0f;
                    slideTime = 0.0f;
                    animator.SetBool("wallrunning", true);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", false);
                    animator.SetBool("sliding", false);
                    animator.SetBool("vaulting", false);
                    standingCollider.enabled = true;
                    slidingCollider.enabled = false;
                    vaultingCollider.enabled = false;

                    break;

                case ActionType.VAULTING:
                    slideTime = 0.0f;
                    animator.SetFloat("vaultTime", vaultTime += 0.05f);
                    animator.SetBool("vaulting", true);
                    animator.SetBool("wallrunning", false);
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", false);
                    animator.SetBool("sliding", false);
                    standingCollider.enabled = false;
                    slidingCollider.enabled = false;
                    vaultingCollider.enabled = true;

                    if(vaultTime >= totalVaultTime) {
                        animator.SetBool("vaulting", false);
                        vaultTime = 0.0f;
                        player.action = ActionType.WALK_RUN;
                    }
                    break;

                default: 
                    break;
            }
        }

        public void SetPlayer(Player player) {
            this.player = player;
        }
    }
}
