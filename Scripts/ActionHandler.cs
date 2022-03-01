using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
{
    public class ActionHandler : MonoBehaviour
    {
        Player player;
        [SerializeField] PlayerController playerController;

        [SerializeField] float gravity = -12f;
        
        public enum ActionType {
            WALK_RUN,
            STRAFE,
            JUMP,
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

        // Works with animator to process required values for an action.
        public void Animate() {
            float animationSpeedPercent = player.GetSpeed()/playerController.GetWalkSpeed() * .5f;
            if(player.IsSprinting())
                animationSpeedPercent = player.GetSpeed()/playerController.GetRunSpeed();

            switch(player.GetAction()) {

                case ActionType.WALK_RUN:
                    playerController.GetAnimator().SetFloat("speedPercent", animationSpeedPercent, 0.01f, Time.deltaTime); // 0.01f is dampening/smoothing time
                    break;

                /* case ActionType.JUMP:
                    player.SetVelocity(new Vector3(player.GetVelocity().x, Mathf.Sqrt(-2 * gravity * Time.deltaTime), player.GetVelocity().z));
                    playerController.GetAnimator().SetBool("jump", true);
                    break; */

                default: 
                    break;
            }
        }

        public void SetPlayer(Player player) {
            this.player = player;
        }
    }
}
