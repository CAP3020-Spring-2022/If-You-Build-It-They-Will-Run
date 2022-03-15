using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;

public class PlayerController : MonoBehaviour
{
    /* ActionHandler actionHandler; */
    /* [SerializeField] Transform orientation; */
    Animator animator;
    Transform cameraT;
    // As far as I know, CharacterController is a barebones movement/collision system, should change into a rigidbody at some point
    // for more complex movement. CharacterController seems pretty limited (and filled with bugs)
    CharacterController body;
    /* Rigidbody rb; */

    Player player = new Player();

    float walkSpeed = 2;
    float runSpeed = 6;
    float gravity = -12;
    float jumpHeight = 1;

    [Range(0,1)]
    float airControlPercent;

    float turnSmoothTime = .2f;
    float turnSmoothVelocity;

    float speedSmoothTime = .1f;
    float speedSmoothVelocity;
    /* float currentSpeed; */
    float velocityY;

    // Wallrunning stuff
    LayerMask walls;
    float wallrunForce;
    float maxWallrunTime;
    float maxWallrunSpeed;
    bool isWallRight, isWallLeft;
    bool canJump;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        if(!cameraT.gameObject.activeSelf)
            this.gameObject.SetActive(false);
        body = GetComponent<CharacterController>();
        /* actionHandler = GetComponent<ActionHandler>(); */
    }

    // Update is called once per frame
    void Update()
    {
        //input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        /* player.SetOrientation(orientation.transform); */

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.SetSprinting(!player.IsSprinting());
        }

        // CheckWall();

        Move(inputDir);

        if(Input.GetKeyDown(KeyCode.Space) && player.GetStamina() >= 15.0f) {
            player.SetAction(ActionHandler.ActionType.JUMP);
            /* Jump(); */
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && player.GetMomentum() >= 15.0f) {
            player.SetSprinting(false);
            player.SetAction(ActionHandler.ActionType.SLIDE);
        }

        if(player.GetStamina() <= 10.0f) { // if you have no stamina, no more sprinting and change action to WALK_RUN
            player.SetSprinting(false);
            player.SetAction(ActionHandler.ActionType.WALK_RUN);
        }

        /* if(Input.GetMouseButtonDown(1) && (isWallRight || isWallLeft)) {
            player.SetAction(ActionHandler.ActionType.WALLRUN);
            canJump = true;
        }else{
            canJump = false;
        } */

        switch(player.GetAction()) {
            case ActionHandler.ActionType.WALK_RUN:
                break; // default handled
            case ActionHandler.ActionType.JUMP:
                Jump();
                break;
            case ActionHandler.ActionType.SLIDE:
                Slide();
                break;
            /* case ActionHandler.ActionType.WALLRUN:
                Wallrun();
                break; */
        }

        UpdateMomentum();
        UpdateStamina();

        //Debug.Log("velocityY = " + velocityY);
        //Debug.Log("isGrounded = " + controller.isGrounded);

        //animations moved to actionHandler
    }

    void Move(Vector2 inputDir)
    {
        if(inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        float targetSpeed = walkSpeed;
        if(player.IsSprinting() || player.GetAction() == ActionHandler.ActionType.SLIDE)
            targetSpeed = runSpeed;
        targetSpeed = targetSpeed * inputDir.magnitude;

        player.SetSpeed(/* (1.0f + player.GetMomentum()/100) *  */Mathf.SmoothDamp(player.GetSpeed(), targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime)));
        /* if(player.GetSpeed() > 35.0f)
            player.SetSpeed(35.0f); */

        /* if(player.GetSpeed() < 1.0f && player.GetAction() == ActionHandler.ActionType.SLIDE)
            player.SetAction(ActionHandler.ActionType.WALK_RUN); */

        if(player.GetAction() != ActionHandler.ActionType.WALLRUN)
            velocityY += Time.deltaTime * gravity;

        player.SetVelocity(transform.forward * player.GetSpeed() + Vector3.up * velocityY);

        body.Move(player.GetVelocity() * Time.deltaTime);
        player.SetSpeed(new Vector2(body.velocity.x, body.velocity.z).magnitude);

        if((body.isGrounded || transform.position.y < 0.1f)
            && player.GetAction() != ActionHandler.ActionType.SLIDE) {

            velocityY = -1;
            player.SetAction(ActionHandler.ActionType.WALK_RUN);
        }
    }

    void UpdateMomentum() {

        // If you are stationary, or walking with momentum (and not sliding)
        if((player.GetVelocity() == Vector3.zero || (!player.IsSprinting() && player.GetMomentum() > 10)) && player.GetAction() != ActionHandler.ActionType.SLIDE) { // You should still keep some momentum if you're walking
            player.SetMomentum(player.GetMomentum() - 0.1f); // TODO: tweak
        }

        // If you are running and moving
        if(player.IsSprinting() && player.GetVelocity() != Vector3.zero && player.GetMomentum() <= 100.0f) {
            player.SetMomentum(player.GetMomentum() + 0.05f);
        }

        // if sliding
        if(player.GetAction() == ActionHandler.ActionType.SLIDE) {
            if(player.GetMomentum() >= 100.0f) // weird bug with floating-points
                player.SetMomentum(99.9999f);
            player.SetMomentum(player.GetMomentum() - 0.01f);
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

    void UpdateStamina() {

        // If not running or jumping, and don't have max stamina
        if(!player.IsSprinting() && velocityY < 1 && player.GetStamina() < 100f) {
            player.SetStamina(player.GetStamina() + 0.15f); // TODO: gonna have to tweak all these numbers in the future
        }

        // If running and moving (can have run enabled and not be moving)
        if(player.IsSprinting() && player.GetVelocity() != Vector3.zero && player.GetStamina() > 0 && player.GetAction() != ActionHandler.ActionType.SLIDE) {
            player.SetStamina(player.GetStamina() - 0.05f);
        }
        
        // If sliding, gain stamina
        if(player.GetAction() == ActionHandler.ActionType.SLIDE) {
            player.SetStamina(player.GetStamina() + 0.5f);
        }

        // low stamina, no running
        // moved to update function for better timing
        /* if(player.GetStamina() < 20f) { // TODO: should add jump check too
            player.SetSprinting(false);
            player.SetAction(ActionHandler.ActionType.WALK_RUN);
        } */

        // Range checking. Sometimes the math makes it over/under flow
        if(player.GetStamina() < 0.0f) {
            player.SetStamina(0.0f);
        }else if(player.GetStamina() > 100.0f) {
            player.SetStamina(100.0f);
        }
    }

    void Jump()
    {
        if((body.isGrounded || transform.position.y < .1 || canJump) && player.GetAction() == ActionHandler.ActionType.JUMP) {
            velocityY = Mathf.Sqrt(-2 * gravity * jumpHeight);
        }
    }

    void Slide() {
        if(body.isGrounded || transform.position.y < 0.1f && player.GetAction() == ActionHandler.ActionType.SLIDE) {
            player.SetSpeed(/* player.GetMomentum()/10 *  */player.GetSpeed() * 0.9f);
        }
    }

    /* void Wallrun() {
        if(player.GetVelocity().magnitude <= maxWallrunSpeed) {
            rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

            if(isWallRight)
                rb.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime); // better name for wallrunForce is probably wallrunStickiness
            else
                rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
        }
    }

    void WallrunEnd() {
        player.SetAction(ActionHandler.ActionType.WALK_RUN);
    }

    void CheckWall() {
        isWallRight = Physics.Raycast(transform.position, player.GetOrientation().right, 1f, walls);
        isWallLeft = Physics.Raycast(transform.position, -player.GetOrientation().right, 1f, walls);

        if(!isWallRight && !isWallLeft)
            WallrunEnd();
        if(isWallRight || isWallLeft)
            canJump = true;
    } */

    float GetModifiedSmoothTime(float smoothTime)
    {
        if(body.isGrounded)
            return smoothTime;

        if(airControlPercent == 0)
            return float.MaxValue;
        return smoothTime / airControlPercent;
    }

    public Player GetPlayer() {
        return player;
    }

    public float GetWalkSpeed() {
        return walkSpeed;
    }

    public float GetRunSpeed() {
        return runSpeed;
    }

    public Animator GetAnimator() {
        return animator;
    }
}
