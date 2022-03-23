using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Joseph **/
// Data Strcuture for player object. Has important vals like action, stamina, position, etc.
namespace PlayerData {

    public class Player {

        Vector3 velocity;
		Vector3 position;

		bool sprinting;
		bool sliding;
		bool jumping;

		bool onGround;

		float stamina;
		float momentum;
		float speed;

		Transform orientation;

		ActionHandler.ActionType action;
		// Start is called before the first frame update

		public Player() {
			stamina = 100f;
			momentum = 0.0f;
			speed = 0.0f;
			velocity = Vector3.zero;
			position = Vector3.zero;
			action = ActionHandler.ActionType.WALK_RUN;
			orientation = null;
		}

		public Transform GetOrientation() {
			return orientation;
		}

		public void SetOrientation(Transform newOrientation) {
			orientation = newOrientation;
		}

		public float GetSpeed() {
			return speed;
		}

		public void SetSpeed(float newSpeed) {
			speed = newSpeed;
		}

		public Vector3 GetPosition()
		{
			return position;
		}

		public void SetPosition(Vector3 newPosition)
		{
			position = newPosition;
		}

		public Vector3 GetVelocity()
		{
			return velocity;
		}

		public void SetVelocity(Vector3 newVelocity)
		{
			velocity = newVelocity;
		}

		public float GetMomentum()
		{
			return momentum;
		}

		public void SetMomentum(float newMomentum)
		{
			momentum = newMomentum;
		}

		public float GetStamina()
		{
			return stamina;
		}

		public void SetStamina(float newStamina)
		{
			stamina = newStamina;
		}

		public bool IsSprinting()
		{
			return sprinting;
		}

		public void SetSprinting(bool flag)
		{
			sprinting = flag;
		}

		public bool IsSliding()
		{
			return sliding;
		}

		public void SetSliding(bool flag)
		{
			sliding = flag;
		}

		public bool IsJumping()
		{
			return jumping;
		}

		public void SetJumping(bool flag)
		{
			jumping = flag;
		}

		public bool OnGround()
		{
			return onGround;
		}

		public void SetOnGround(bool flag)
		{
			onGround = flag;
		}
		
		public ActionHandler.ActionType GetAction() {
			return action;
		}

		public void SetAction(ActionHandler.ActionType newAction) {
			action = newAction;	
		}
    }
}
