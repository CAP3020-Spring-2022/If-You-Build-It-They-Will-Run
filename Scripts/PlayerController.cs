using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody player;
    Animator animator;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float currentSpeed;
    float turnSmoothTime = .2f;
    float turnSmoothVelocity;

    float speedSmoothTime = .01f;
    float speedSmoothVelocity;

    [SerializeField] float gravity = -12f;
    [SerializeField] float jumpHeight = 1f;
    [Range(0,1)] float airControlPercent;
    float velocityY;

    [SerializeField] Transform groundCheck;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayer;

    void Start() {
        animator = GetComponent<Animator>();
        player = GetComponent<Rigidbody>();
    }

    void Update() {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        bool running = false;
        if(Input.GetKeyDown(KeyCode.LeftShift))
            running = !running;

        currentSpeed = GetSpeed(inputDir, running);

        if(player.position.y > 0) {
            float deltaY = gravity * Time.deltaTime;
            velocityY += deltaY;
        }

        velocityY = Mathf.Clamp(velocityY, 0, velocityY);

        //TODO: weird jump logic
        if(Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        /* if(velocityY != 0)
            velocityY += Time.deltaTime * gravity;
        else if(player.position.y < 0) {
            velocityY = 0;
            player.MovePosition(new Vector3(player.position.x, 0, player.position.z));
        } */

        /* velocityY = Mathf.Clamp(velocityY, 30f, 0); */

        // calc velocity (this seems to work, but need to edit animation)
        Vector3 velocity = transform.right * inputDir.x + transform.forward * inputDir.y + Vector3.up * velocityY;
        player.MovePosition(transform.position + velocity.normalized * currentSpeed * Time.deltaTime);

        float animationSpeedPercent = currentSpeed/walkSpeed * .5f;
        if(running)
            animationSpeedPercent = currentSpeed/runSpeed;
        animationSpeedPercent = animationSpeedPercent * inputDir.magnitude;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    float GetSpeed(Vector2 vec, bool running) {
        float targetSpeed;
        if(running) 
            targetSpeed = runSpeed;
        else
            targetSpeed = walkSpeed;
        targetSpeed *= vec.magnitude;

        return Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
    }

    void Jump() {
        if(IsOnGround())
            velocityY = Mathf.Sqrt(-2 * gravity * jumpHeight);
    }

    public float GetCurrentSpeed() {
        return currentSpeed;
    }

    public Rigidbody getPlayer() {
        return player;
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
