using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: It may be a good idea to make an approximate function that checks a range around a number instead of just some
// floating-point value. Ex: check values from 99.995f to 100.005f if looking for value of 100.0f

public class PlayerController : MonoBehaviour
{
    Rigidbody player;
    Animator animator;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float currentSpeed;
    /* float turnSmoothTime = .2f;
    float turnSmoothVelocity; */

    float speedSmoothTime = .01f;
    float speedSmoothVelocity;

    [SerializeField] float gravity = -12f;
    [SerializeField] float jumpHeight = 1f;
    [Range(0,1)] float airControlPercent;
    float velocityY;
    Vector3 currentVelocity;

    bool running = false;
    bool sliding = false;

    [SerializeField] Transform groundCheck;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] float stamina = 300f;
    [SerializeField] float momentum = 0f;

    void Start() {
        animator = GetComponent<Animator>();
        player = GetComponent<Rigidbody>();
    }

    void Update() {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        if(Input.GetKeyDown(KeyCode.LeftShift)) // made a toggle
            running = !running;

        currentSpeed = GetSpeed(inputDir, running);
        //OrientPlayer();

        if(player.position.y > 0) {
            float deltaY = gravity * Time.deltaTime;
            velocityY += deltaY;
        }

        velocityY = Mathf.Clamp(velocityY, 0, velocityY);

        if(Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl)) {
            sliding = true;
            Slide();
        }else{
            sliding = false;
        }

        // calc velocity (this seems to work, but need to edit animations)
        currentVelocity = transform.right * inputDir.x + transform.forward * inputDir.y + Vector3.up * velocityY;
        player.MovePosition(transform.position + currentVelocity.normalized * currentSpeed * Time.deltaTime);

        // Animations
        float animationSpeedPercent = currentSpeed/walkSpeed * .5f;
        if(running)
            animationSpeedPercent = currentSpeed/runSpeed;
        //TODO: Sliding animation
        animationSpeedPercent = animationSpeedPercent * inputDir.magnitude;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

        // Calculate stamina/momentum at end of update (PostUpdate function??)
        normalizeStamina();
        normalizeMomentum();
    }

    float GetSpeed(Vector2 vec, bool running) {
        float targetSpeed;
        if(running) 
            targetSpeed = runSpeed;
        else
            targetSpeed = walkSpeed;
        targetSpeed *= vec.magnitude;

        float curSpeed = momentum/10 + Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
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
        if(sliding) {
            currentSpeed *= 0.95f;
        }
        if(currentSpeed == 0.0f) {
            sliding = false;
        }
    }

    void Jump() {
        // TODO: Probably a better calc for this, goes a little too vertical and not horizontal enough
        // Also weirdly dependent on speed (probably vector calculation issue)
        if(IsOnGround())
            velocityY = Mathf.Sqrt(-2 * gravity * jumpHeight);
    }

    /* int slideInstance = -1;
    int slideCount = 0; */
    void normalizeMomentum() {
        // If you are stationary, or walking with momentum
        if((currentVelocity == Vector3.zero || (!running && momentum > 10)) && !sliding) { // You should still keep some momentum if you're walking
            momentum -= 0.1f; // TODO: tweak
        }

        // If you are running and moving
        if(running && currentVelocity != Vector3.zero && momentum <= 100.0f) {
            momentum++;
        }

        // if sliding
        if(sliding) {
            if(momentum >= 100.0f) // weird bug with floating-points
                momentum = 99.999999f;
            momentum -= 0.05f;
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
        if(momentum < 0.0f) {
            momentum = 0.0f;
        }else if(momentum > 100.0f) {
            momentum = 100.0f;
        }
    }

    void normalizeStamina() {
        // If not running or jumping, and don't have max stamina
        if(!running && velocityY == 0 && stamina < 100f) {
            stamina += 0.2f; // TODO: gonna have to tweak all these numbers in the future
        }

        // If running and moving (can have run enabled and not be moving)
        if(running && currentVelocity != Vector3.zero && stamina > 0 && !sliding) {
            stamina -= 0.2f;
        }
        
        // If sliding, gain stamina
        if(sliding) {
            stamina += 0.5f;
        }

        // low stamina, no running
        if(stamina < 20f) { // TODO: should add jump check too
            running = false;
        }

        // Range checking. Sometimes the math makes it over/under flow
        if(stamina < 0.0f) {
            stamina = 0.0f;
        }else if(stamina > 100.0f) {
            stamina = 100.0f;
        }
    }

    public float GetCurrentSpeed() {
        return currentSpeed;
    }

    public Rigidbody GetPlayer() {
        return player;
    }

    public float GetStamina() {
        return stamina;
    }

    public float GetMomentum() {
        return momentum;
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if(IsOnGround())
            return smoothTime;

        if(airControlPercent == 0)
            return float.MaxValue;
        return smoothTime / airControlPercent;
    }

    public bool IsOnGround() {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, checkRadius, groundLayer);
        if(colliders.Length > 0)
            return true;
        else
            return false;
    }
}
