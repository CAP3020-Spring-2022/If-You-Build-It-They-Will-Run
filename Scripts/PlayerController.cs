using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using PlayerData;

// TODO: It may be a good idea to make an approximate function that checks a range around a number instead of just some
// floating-point value. Ex: check values from 99.995f to 100.005f if looking for value of 100.0f

public class PlayerController : MonoBehaviour
{
    Player player = new Player();
    Rigidbody body;
    Animator animator;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float currentSpeed;
    float speedSmoothTime = .01f;
    float speedSmoothVelocity;

    /* [SerializeField] float gravity = -12f; */
    [SerializeField] float jumpHeight = 1f;
    [Range(0,1)] float airControlPercent;

    [SerializeField] Transform groundCheck;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] ActionHandler actionHandler;

    float velocityY = 0.0f;

    void Start() {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        // attach player to actionhandler 
        actionHandler = GetComponent<ActionHandler>();
    }

    void Update() {
        Vector3 currentVelocity = Vector3.zero;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        if(Input.GetKeyDown(KeyCode.LeftShift)) // made a toggle
            player.SetSprinting(!player.IsSprinting());

        player.SetSpeed(CalculateSpeed(inputDir, player.IsSprinting()));
        //OrientPlayer();

        /* if(player.GetPosition().y > 0) {
            float deltaY = gravity * Time.deltaTime;
            velocityY += deltaY;
        } */

        /* velocityY = Mathf.Clamp(velocityY, 0, velocityY); */

        if(Input.GetKeyDown(KeyCode.Space)) {
            player.SetAction(ActionHandler.ActionType.JUMP);
        }

        /* if(Input.GetKeyDown(KeyCode.LeftControl)) {
            player.SetSliding(true);
            Slide();
        }else{
            player.SetSliding(false);
        } */

        // calc velocity (this seems to work, but need to edit animations)
        player.SetVelocity(transform.right * inputDir.x + transform.forward * inputDir.y/*  + Vector3.up * velocityY */);
        body.MovePosition(transform.position + player.GetVelocity().normalized * player.GetSpeed() * Time.deltaTime);

        // Animations

        /* float animationSpeedPercent = currentSpeed/walkSpeed * .5f;
        if(player.IsSprinting())
            animationSpeedPercent = currentSpeed/runSpeed;
        //TODO: Sliding animation
        animationSpeedPercent = animationSpeedPercent * inputDir.magnitude;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime); */

        // Calculate stamina/momentum at end of update (PostUpdate function??)
        normalizeStamina();
        normalizeMomentum();
    }

    float CalculateSpeed(Vector2 vec, bool running) {
        float targetSpeed;
        if(running) 
            targetSpeed = runSpeed;
        else
            targetSpeed = walkSpeed;
        targetSpeed *= vec.magnitude;

        float curSpeed = player.GetMomentum()/10 + Mathf.SmoothDamp(player.GetSpeed(), targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
        if(curSpeed > 35f)
            curSpeed = 35f;
        return curSpeed;
    }

    // TODO: Figure out some way to turn the player for sliding? Probably just make a sliding animation
    /* void OrientPlayer() {
        if(sliding && player.rotation.x != 90.0f) {
            player.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f), Space.Self);
        }else if(player.rotation.x != 0.0f) {
            player.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f), Space.Self);
        }
    } */

    // TODO: Figure out good interaction if sliding AND running, messes with momentum/stamina right now
    void Slide() {
        if(player.IsSliding()) {
            currentSpeed *= 0.95f;
        }
        if(currentSpeed == 0.0f) {
            player.SetSliding(false);
        }
    }

    /* void Jump() {
        // TODO: Probably a better calc for this, goes a little too vertical and not horizontal enough
        // Also weirdly dependent on speed (probably vector calculation issue)
        if(player.IsOnGround())
            velocityY = Mathf.Sqrt(-2 * gravity * jumpHeight);
    } */

    /* int slideInstance = -1;
    int slideCount = 0; */
    void normalizeMomentum() {
        // If you are stationary, or walking with momentum
        if((player.GetVelocity() == Vector3.zero || (!player.IsSprinting() && player.GetMomentum() > 10)) && !player.IsSliding()) { // You should still keep some momentum if you're walking
            player.SetMomentum(player.GetMomentum() - 0.1f); // TODO: tweak
        }

        // If you are running and moving
        if(player.IsSprinting() && player.GetVelocity() != Vector3.zero && player.GetMomentum() <= 100.0f) {
            player.SetMomentum(player.GetMomentum() + 1.0f);
        }

        // if sliding
        if(player.IsSliding()) {
            if(player.GetMomentum() >= 100.0f) // weird bug with floating-points
                player.SetMomentum(99.9999f);
            player.SetMomentum(player.GetMomentum() + 1.0f);
            // This block doesn't work for some reason, come back to it maybe if you can't find a better solution
            // TODO: Want sliding to have a short period before reducing momentum
            // Temporary fix is to just half the usual momentum decrease
            /* if(slideInstance == -1) { // first instance
                slideInstance = 60;
                return;
            }
            // after period
            if(slideCount == slideInstance) {
                momentum -= 0.1f;
                if(currentSpeed <= 10.0f) {
                    sliding = false;
                    slideInstance = -1;
                    slideCount = 0;
                }
            }
            slideCount++;
        }else{
            slideInstance = -1;
            slideCount = 0; */
        }

        // range check
        if(player.GetMomentum() < 0.0f) {
            player.SetMomentum(0.0f);
        }else if(player.GetMomentum() > 100.0f) {
            player.SetMomentum(100.0f);
        }
    }

    void normalizeStamina() {
        // If not running or jumping, and don't have max stamina
        if(!player.IsSprinting() && velocityY == 0 && player.GetStamina() < 100f) {
            player.SetStamina(player.GetStamina() + 0.2f); // TODO: gonna have to tweak all these numbers in the future
        }

        // If running and moving (can have run enabled and not be moving)
        if(player.IsSprinting() && player.GetVelocity() != Vector3.zero && player.GetStamina() > 0 && !player.IsSliding()) {
            player.SetStamina(player.GetStamina() - 0.2f);
        }
        
        // If sliding, gain stamina
        if(player.IsSliding()) {
            player.SetStamina(player.GetStamina() + 0.5f);
        }

        // low stamina, no running
        if(player.GetStamina() < 20f) { // TODO: should add jump check too
            player.SetSprinting(false);
        }

        // Range checking. Sometimes the math makes it over/under flow
        if(player.GetStamina() < 0.0f) {
            player.SetStamina(0.0f);
        }else if(player.GetStamina() > 100.0f) {
            player.SetStamina(100.0f);
        }
    }

    public float GetCurrentSpeed() {
        return currentSpeed;
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if(player.IsOnGround())
            return smoothTime;

        if(airControlPercent == 0)
            return float.MaxValue;
        return smoothTime / airControlPercent;
    }

    public void CheckGround() {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, checkRadius, groundLayer);
        if(colliders.Length > 0)
            player.SetOnGround(true);
        else
            player.SetOnGround(false);
    }

    public Player GetPlayer() {
        return player;
    }

    public Animator GetAnimator() {
        return animator;
    }

    public float GetWalkSpeed() {
        return walkSpeed;
    }

    public float GetRunSpeed() {
        return runSpeed;
    }
}
