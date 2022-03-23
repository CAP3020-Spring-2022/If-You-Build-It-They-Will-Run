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
            float animationSpeedPercent = player.GetSpeed()/playerController.GetWalkSpeed() * .5f;
            if(player.IsSprinting())
                animationSpeedPercent = player.GetSpeed()/playerController.GetRunSpeed();
            Mathf.Clamp(animationSpeedPercent, 0.0f, 1.0f);

            switch(player.GetAction()) {
                case ActionType.WALK_RUN:
                    playerController.GetAnimator().SetFloat("animSpeed", animationSpeedPercent, 0.01f, Time.deltaTime); // 0.01f is dampening/smoothing time  
                    playerController.GetAnimator().SetBool("jumping", false);
                    playerController.GetAnimator().SetBool("sliding", false);
                    playerController.GetAnimator().SetBool("wallrunning", false);
                    break;

                case ActionType.JUMP:
                    playerController.GetAnimator().SetFloat("animSpeed", 1.0f);
                    playerController.GetAnimator().SetBool("jumping", true);
                    playerController.GetAnimator().SetBool("sliding", false);
                    break;

                case ActionType.FALLING:
                    playerController.GetAnimator().SetBool("jumping", false);
                    break;

                case ActionType.SLIDE:
                    playerController.GetAnimator().SetFloat("slideTime", slideTime += 0.05f);
                    playerController.GetAnimator().SetBool("sliding", true);

                    if(slideTime >= 5.0f) {
                        playerController.GetAnimator().SetBool("sliding", false);
                        slideTime = 0.0f;
                        player.SetAction(ActionType.WALK_RUN);
                    }
                    break;

                case ActionType.WALLRUN:
                    playerController.GetAnimator().SetBool("wallrunning", true);
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
