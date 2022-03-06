using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
{
    public class ActionHandler : MonoBehaviour
    {
        Player player;
        [SerializeField] PlayerController playerController;
        
        public enum ActionType {
            WALK_RUN,
            STRAFE,
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

        void PostUpdate() {
            Animate();
        }

        // Works with animator to process required values for an action.
        public void Animate() {
            float animationSpeedPercent = player.GetSpeed()/playerController.GetWalkSpeed() * .5f;
            if(player.IsSprinting())
                animationSpeedPercent = player.GetSpeed()/playerController.GetRunSpeed();

            Mathf.Clamp(animationSpeedPercent, 0.0f, 1.0f);

            /* SetActionDefaults(); */
            switch(player.GetAction()) {

                case ActionType.WALK_RUN:
                    playerController.GetAnimator().SetFloat("animSpeed", animationSpeedPercent, 0.01f, Time.deltaTime); // 0.01f is dampening/smoothing time  
                    break;

                case ActionType.JUMP:
                    // player.SetVelocity(new Vector3(player.GetVelocity().x, Mathf.Sqrt(-2 * gravity * Time.deltaTime), player.GetVelocity().z));
                    playerController.GetAnimator().SetFloat("animSpeed", 1.0f);
                    playerController.GetAnimator().SetBool("jumping", true);
                    break;

                case ActionType.FALLING:
                    playerController.GetAnimator().SetBool("jumping", false);
                    break;

                default: 
                    break;
            }
        }

        /* void SetActionDefaults() {
            playerController.GetAnimator().SetBool("jumping", false);
        } */

        public void SetPlayer(Player player) {
            this.player = player;
        }
    }
}
