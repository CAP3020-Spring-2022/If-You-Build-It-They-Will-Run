using System;
using UnityEngine;
using PlayerData;

public class PlayerController : MonoBehaviour
{

    /** Camera **/
    Transform cameraT;

    /** Player **/
    Animator animator;
    // Transform orientation;
    Rigidbody rb;
    Player player = new Player();
    // float x, y;

    /** Movement **/
    Vector3 normalVector = Vector3.up;
    Vector3 input;
    Vector2 normalInput; // normalized x and z movement

    float maxSlopeAngle = 35f;
    public bool grounded; // TODO: put into player
    float turnSmoothTime = .2f;
    float turnSmoothVelocity;
    float speedSmoothTime = .1f;
    float speedSmoothVelocity;

    float walkSpeed = 2f;
    float runSpeed = 5f;
    float gravity = 10f; // now positive because Vector3.down

    /** Jump **/
    bool jumpCheck = true;
    float jumpHeight = 10f;

    [Range(0,1)]
    float airControlPercent; // TODO: reimplement

    /** Slide **/
    float slideForce = 10f;

    /** Wallrun **/
    float wallrunForce;
    float maxWallrunTime;
    float maxWallrunSpeed;
    bool isWallRight, isWallLeft;

    /** World **/
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        if(!cameraT.gameObject.activeSelf)
            this.gameObject.SetActive(false);
        player.action = ActionHandler.ActionType.WALK_RUN;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        Movements();
        /* Animations(); */ // no longer needed because it gets handled automatically now
        CheckForWall();

        UpdateMomentum();
        UpdateStamina();
    }

    void Inputs() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        normalInput = new Vector2(input.x, input.z).normalized;

        if(normalInput != Vector2.zero) // change direction of player depending on rotation of camera
        {
            float targetRotation = Mathf.Atan2(normalInput.x, normalInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
            transform.localRotation = Quaternion.Euler(0, cameraT.transform.localRotation.eulerAngles.y, 0);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ToggleSprinting();
        }

        if(Input.GetKey(KeyCode.Space) && player.stamina >= 15.0f && jumpCheck) {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && player.momentum >= 15.0f) {
            Slide();
        }

        if(Input.GetKey(KeyCode.D) && isWallRight) {
            StartWallrun();
        }

        if(Input.GetKey(KeyCode.A) && isWallLeft) {
            StartWallrun();
        }
    }

    void Movements()
    {           
        if(rb.velocity.y <= -1.5f && !grounded) { // check before applying gravity?
            player.action = ActionHandler.ActionType.FALLING;
        }

        rb.AddForce(Vector3.down * Time.deltaTime * gravity); // gravity

        // get speed multiplier
        float targetSpeed = walkSpeed;
        if(player.IsSprinting()) {
            targetSpeed = runSpeed;
        }
        /* targetSpeed *= 1.0f + player.momentum; */
        /* targetSpeed *= normalInput.magnitude; */ // I don't think this does anything
        player.speed = Mathf.SmoothDamp(player.speed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
        /* Debug.Log("Player speed: " + player.GetSpeed()); */

        /* Vector2 relativeVel = GetRelativePlayerVelocity();
        CounterMove(input.x, input.y, relativeVel, targetSpeed); */
        
        // reset things if grounded
        if(grounded) {
            jumpCheck = true;
            if(player.action == ActionHandler.ActionType.FALLING)
                player.action = ActionHandler.ActionType.WALK_RUN;
        }
        
        // set velocity of rigidbody
        player.velocity = transform.TransformDirection(input) * player.speed;
        // transform.Translate(player.velocity, this.transform);
        rb.velocity = new Vector3(player.velocity.x, rb.velocity.y, player.velocity.z);
    }

    private Vector2 GetRelativePlayerVelocity() {
        float lookAngle = cameraT.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private void CounterMove(float x, float y, Vector2 magnitude, float targetSpeed) {
        if(!grounded || player.IsJumping()) return;

        if(Mathf.Abs(magnitude.x) > 0.01f && Math.Abs(x) < 0.05f || (magnitude.x < -0.01f && x > 0) || (magnitude.x > 0.01f && x < 0))
            rb.AddForce(targetSpeed * transform.right * Time.deltaTime * -magnitude.x * 0.175f);
        if (Math.Abs(magnitude.y) > 0.01f && Math.Abs(y) < 0.05f || (magnitude.y < -0.01f && y > 0) || (magnitude.y > 0.01f && y < 0))
            rb.AddForce(targetSpeed * transform.forward * Time.deltaTime * -magnitude.y * 0.175f);
    }

    // Handles control of ActionHandler depending on movement type, actual 
    // variables changes of animator handled in ActionHandler.cs
    /* void Animations() {
        switch(player.action) {
            case ActionHandler.ActionType.WALK_RUN:
                break; // default handled
            case ActionHandler.ActionType.JUMP:
                Jump();
                break;
            case ActionHandler.ActionType.SLIDE:
                Slide();
                break;
            case ActionHandler.ActionType.FALLING:
                if(grounded)
                    player.action = ActionHandler.ActionType.WALK_RUN;
                break;
            case ActionHandler.ActionType.WALLRUN:
                Wallrun();
                break;
        }
    } */

    // LEO TODO: FINISH WATCHING ANIMATION VIDEOS
    /*
    BUGS
    SLIDE -> STUCK ANIMATION
    PLAYER MOVEMENT RANDOMLY STUCK, BUT FIXES WITH FIDGETING
    */
    void Jump()
    {
        if(grounded) {
            jumpCheck = false;
            player.action = ActionHandler.ActionType.JUMP;

            rb.AddForce(Vector3.up * jumpHeight * 1.5f);
            rb.AddForce(normalVector * jumpHeight * 0.5f);
        }
    }

    void Slide() {
        if(rb.velocity.magnitude > 0.5f && grounded) {
            player.action = ActionHandler.ActionType.SLIDE;
            rb.AddForce(transform.forward * slideForce);
        }else{
            player.action = ActionHandler.ActionType.WALK_RUN;
        }
    }

    void StartWallrun() {
        rb.useGravity = false;
        player.action = ActionHandler.ActionType.WALLRUN;

        if(rb.velocity.magnitude <= maxWallrunSpeed) {
            rb.AddForce(transform.forward * wallrunForce * Time.deltaTime);

            // stick character to wall
            if(isWallRight)
                rb.AddForce(transform.right * wallrunForce / 5 * Time.deltaTime);
            else
                rb.AddForce(-transform.right * wallrunForce / 5 * Time.deltaTime);
        }
    }

    void StopWallrun() {
        player.action = ActionHandler.ActionType.WALK_RUN;
        rb.useGravity = true;
    }

    void CheckForWall() {
        isWallRight = Physics.Raycast(transform.position, transform.right, 1f, wallLayer);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, 1f, wallLayer);

        if(!isWallLeft && !isWallRight && player.IsWallrunning())
            StopWallrun();
    }

    bool IsFloor(Vector3 vec) {
        float angle = Vector3.Angle(Vector3.up, vec); // check angle between (deg) between y-axis and object
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    private void OnCollisionStay(Collision other) {
        int layer = other.gameObject.layer;
        if(groundLayer != (groundLayer | (1 << layer))) return;

        for(int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            if(IsFloor(normal)) { // if its just a slope you never left the ground
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        // Invoke ground/wall cancel, can't check normals with CollisionExit
        // delay ensures you're actually on the ground, can be weird otherwise
        float delay = 3f;
        if(!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

    void UpdateMomentum() {
        float nm = player.momentum;
        switch(player.action) {
            case ActionHandler.ActionType.WALK_RUN:
                if(normalInput == Vector2.zero) // not moving (horizontal axis)
                    nm -= 0.001f;
                else
                    if(player.IsSprinting())
                        nm += 0.002f;
                    else
                        nm -= 0.001f;
                break;
            case ActionHandler.ActionType.JUMP:
                nm += 0.0005f;
                break;
            case ActionHandler.ActionType.SLIDE:
                // keep momentum
                break;
        }

        // bound checking
        if(nm < 0.0f)
            nm = 0.0f;
        else if(nm > 2.0f)
            nm = 2.0f;

        player.momentum = nm;
    }

    void UpdateStamina() {
        float ns = player.stamina;
        switch(player.action) {
            case ActionHandler.ActionType.WALK_RUN:
                if(normalInput == Vector2.zero) // not moving (horizontal axis)
                    ns += 0.5f;
                else
                    if(player.IsSprinting())
                        ns -= 0.02f;
                    else
                        ns += 0.02f;
                break;
            case ActionHandler.ActionType.JUMP:
                ns -= 0.00001f;
                break;
            case ActionHandler.ActionType.SLIDE:
                ns += 0.5f;
                break;
        }

        // bound checking
        if(ns < 0.0f)
            ns = 0.0f;
        else if(ns > 100.0f)
            ns = 100.0f;

        player.stamina = ns;
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if(grounded)
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
