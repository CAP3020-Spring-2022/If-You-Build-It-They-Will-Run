using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData {

    public class ActionHandler : MonoBehaviour {
    
        Player player;
        PlayerController playerController;

        [Range(0.0f, 5.0f)]
        float slideTime = 0.0f;

        public enum ActionType {
            WALK_RUN,
            /* STRAFE, */
            JUMP,
            FALLING,
            SLIDE,
            WALLRUN,
            /* VAULT, */
        }

        void Start() {
            playerController = GetComponent<PlayerController>();
            player = playerController.GetPlayer();
        }

        void Update() {
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
                    animator.SetFloat("animSpeed", animationSpeedPercent, 0.01f, Time.deltaTime); // 0.01f is dampening/smoothing time  
                    animator.SetBool("jumping", false);
                    animator.SetBool("sliding", false);
                    animator.SetBool("wallrunning", false);
                    animator.SetBool("falling", false);
                    break;

                case ActionType.JUMP:
                    animator.SetFloat("animSpeed", 1.0f);
                    animator.SetBool("jumping", true);
                    animator.SetBool("sliding", false);
                    break;

                case ActionType.FALLING:
                    animator.SetBool("jumping", false);
                    animator.SetBool("falling", true);
                    break;

                case ActionType.SLIDE:
                    animator.SetFloat("slideTime", slideTime += 0.05f);
                    animator.SetBool("sliding", true);

                    // No longer needed, figured out animator
                    if(slideTime >= 5.0f) {
                        animator.SetBool("sliding", false);
                        slideTime = 0.0f;
                        player.action = ActionType.WALK_RUN;
                    }
                    break;

                case ActionType.WALLRUN:
                    animator.SetBool("wallrunning", true);
                    animator.SetBool("jumping", false);
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
