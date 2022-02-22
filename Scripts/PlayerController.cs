using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    private float jumpVelocity;

    bool running;
    [Range(0,1)]
    public float airControlPercent;

    public float turnSmoothTime = .2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = .1f;
    float speedSmoothVelocity;
    float velocityY;

    private float currentSpeed;
    private float animationSpeedPercent;
    private float animationJumpPercent;

    Animator animator;
    Transform cameraT;
    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator> ();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController> ();

        jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
    }

    // Update is called once per frame
    void Update()
    {
        //input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDir, running);

        if(Input.GetKeyDown(KeyCode.Space))
            Jump();

        Animate(inputDir);        
    }

    void Move(Vector2 inputDir, bool running)
    {
        if(inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        float targetSpeed = walkSpeed;
        if(running)
            targetSpeed = runSpeed;        
        targetSpeed = targetSpeed * inputDir.magnitude;

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocityY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if(controller.isGrounded)
            velocityY = -1;
    }

    void Jump()
    {
        if(controller.isGrounded || transform.position.y < .1)
            velocityY = Mathf.Sqrt(-2 * gravity * jumpHeight);
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if(controller.isGrounded)
            return smoothTime;

        if(airControlPercent == 0)
            return float.MaxValue;
        return smoothTime / airControlPercent;
    }
//TODO: LOOK UP JUMPING ANIMATION TUTORIAL
    void Animate(Vector2 inputDir)
    {
        //horizontal
        animationSpeedPercent = currentSpeed/walkSpeed * .5f;
        if(running)
            animationSpeedPercent = currentSpeed/runSpeed;
        animationSpeedPercent = animationSpeedPercent * inputDir.magnitude;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

        //vertical
        animationJumpPercent = 0;
        if(velocityY > 0)
            animationJumpPercent = 1;

        animator.SetFloat("jumpPercent", animationJumpPercent, speedSmoothTime, Time.deltaTime);
    }
}
