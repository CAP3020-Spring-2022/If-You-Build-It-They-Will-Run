using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0,1)]
    public float airControlPercent;

    public float turnSmoothTime = .2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = .1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    Animator animator;
    Transform cameraT;
    CharacterController controller;

    // build/run phase switching
    public GameObject CamSwitchObject;
    camswitch camSwitch;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator> ();
        cameraT = Camera.main.transform;
<<<<<<< Updated upstream
        controller = GetComponent<CharacterController> ();
=======
        if(!cameraT.gameObject.activeSelf)
            this.gameObject.SetActive(false);
        body = GetComponent<CharacterController>();
        /* actionHandler = GetComponent<ActionHandler>(); */

        // build/run stuff
        camSwitch = CamSwitchObject.GetComponent<camswitch>();
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        //input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);

<<<<<<< Updated upstream
        Move(inputDir, running);
=======
        /* player.SetOrientation(orientation.transform); */

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.SetSprinting(!player.IsSprinting());
        }

        // CheckWall();

        if (!camSwitch.buildphase)
        {
            Move(inputDir);
        }
        
        if(Input.GetKeyDown(KeyCode.Space) && player.GetStamina() >= 15.0f) {
            player.SetAction(ActionHandler.ActionType.JUMP);
            /* Jump(); */
        }
>>>>>>> Stashed changes

        if(Input.GetKeyDown(KeyCode.Space))
            Jump();



        //Debug.Log("velocityY = " + velocityY);
        //Debug.Log("isGrounded = " + controller.isGrounded);

        //animation
        float animationSpeedPercent = currentSpeed/walkSpeed * .5f;
        if(running)
            animationSpeedPercent = currentSpeed/runSpeed;
        animationSpeedPercent = animationSpeedPercent * inputDir.magnitude;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
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
            velocityY = 0;
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
}
