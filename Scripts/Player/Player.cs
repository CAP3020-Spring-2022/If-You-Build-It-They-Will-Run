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

		public bool grounded { get; set; }
		public bool onWall { get; set; }
		public bool wallRight { get; set; }

		public ActionHandler.ActionType action { get; set; }

		public Player() {
			stamina = 100f;
			momentum = 0.0f;
			speed = 0.0f;
			velocity = Vector3.zero;
			action = ActionHandler.ActionType.WALK_RUN;
		}

		// TODO: Sprinting is technically just a speed mod, why have it change action?
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
			return this.action == ActionHandler.ActionType.SLIDE;
		}

		public bool IsJumping()
		{
			return this.action == ActionHandler.ActionType.JUMP;
		}

		public bool IsWallrunning() {
			return this.action == ActionHandler.ActionType.WALLRUN;
		}

		public bool IsFalling() {
			return this.action == ActionHandler.ActionType.FALLING;
		}
		
		public bool IsVaulting() {
			return this.action == ActionHandler.ActionType.VAULT;
		}

		public void SetJumping(bool flag) {
			if(flag && grounded) {
				action = ActionHandler.ActionType.JUMP;
			}
		}

		public void SetSliding(bool flag) {
			if(flag && grounded) {
				action = ActionHandler.ActionType.SLIDE;
			}
		}

		public void SetWallrunning(bool flag) {
			if(flag && onWall) {
				action = ActionHandler.ActionType.WALLRUN;
			}
		}
		
		public void SetFalling(bool flag) {
			if(flag && !grounded) {
				action = ActionHandler.ActionType.FALLING;
			}
		}

		public void SetVaulting(bool flag) {
			if(flag) {
				action = ActionHandler.ActionType.VAULT;
			}
		}
    }
}
