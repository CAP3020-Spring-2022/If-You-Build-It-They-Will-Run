using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Joseph **/
// Data Strcuture for player object. Has important vals like action, stamina, position, etc.
namespace PlayerData {

    public class Player {

        public Vector3 velocity { get; set; }

		bool sprinting;

		public float stamina { get; set; }
		public float momentum { get; set; }
		public float speed { get; set; }

		public ActionHandler.ActionType action { get; set; }

		public Player() {
			stamina = 100f;
			momentum = 0.0f;
			speed = 0.0f;
			velocity = Vector3.zero;
			action = ActionHandler.ActionType.WALK_RUN;
		}

		public void SetSprinting(bool flag) {
			sprinting = flag;
			if(flag) {
				action = ActionHandler.ActionType.WALK_RUN;
			}
		}

		public bool IsSprinting()
		{
			return sprinting;
		}

		public void ToggleSprinting() {
			sprinting = !sprinting;
		}

		public bool IsSliding()
		{
			return this.action == ActionHandler.ActionType.SLIDE ? true : false;
		}

		public bool IsJumping()
		{
			return this.action == ActionHandler.ActionType.JUMP ? true : false;
		}

		public bool IsWallrunning() {
			return this.action == ActionHandler.ActionType.WALLRUN ? true : false;
		}

		public bool IsFalling() {
			return this.action == ActionHandler.ActionType.FALLING ? true : false;
		}
    }
}
