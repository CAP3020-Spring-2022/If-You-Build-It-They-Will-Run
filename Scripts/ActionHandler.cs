using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerData {

    public class ActionHandler : MonoBehaviour {
    
        Player player;
        PlayerController playerController;

        public Text timerText;
        string slidingBooleanText = "false";
        string slideTimeText;
        string state = "NO_STATE";
        

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
                    playerController.GetAnimator().SetBool("falling", false);
                    state = "WALK_RUN";
                    break;

                case ActionType.JUMP:
                    playerController.GetAnimator().SetFloat("animSpeed", 1.0f);
                    playerController.GetAnimator().SetBool("jumping", true);
                    playerController.GetAnimator().SetBool("sliding", false);      
                    state = "JUMP";              
                    break;

                case ActionType.FALLING:
                    playerController.GetAnimator().SetBool("jumping", false);
                    playerController.GetAnimator().SetBool("falling", true);       
                    state = "FALLING";             
                    break;

                case ActionType.SLIDE:
                    Debug.Log("Slide started");
                    //playerController.GetAnimator().SetFloat("slideTime", slideTime += 0.05f);
                    //slideTime += .05f;
                    playerController.GetAnimator().SetBool("sliding", true);
                    slidingBooleanText = "true";
                    slideTimeText = slideTime.ToString("f2");
                    state = "SLIDE";

                    if(slideTime >= 5.0f) {
                        Debug.Log("Slide ended");
                        playerController.GetAnimator().SetBool("sliding", false);
                        slideTime = 0.0f;
                        player.SetAction(ActionType.WALK_RUN);
                        slidingBooleanText = "false";
                        state = "SLIDE_END";
                    }
                    break;

                case ActionType.WALLRUN:
                    playerController.GetAnimator().SetBool("wallrunning", true);
                    state = "WALLRUN";
                    break;

                default: 
                    break;
            }

            timerText.text = state + " " + slideTimeText + " " + slidingBooleanText;

        }

        public void SetPlayer(Player player) {
            this.player = player;
        }
    }
}
