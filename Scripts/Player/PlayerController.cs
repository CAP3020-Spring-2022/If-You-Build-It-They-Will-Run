using System;
using UnityEngine;
using UnityEngine.UI;
using PlayerData;

public class PlayerController : MonoBehaviour
{
    public Text textBox;

    /** Camera **/
    Transform cameraT;
    [SerializeField] camswitch cameraSettings;

    /** Player **/
    Animator animator;
    // Transform orientation;
    Rigidbody rb;
    Collider collider;
    Player player = new Player();
    // float x, y;

    /** Movement **/
    Vector3 normalVector = Vector3.up;
    Vector3 input;
    Vector2 normalInput; // normalized x and z movement

    float maxSlopeAngle = 35f;
    public bool grounded; // TODO: put into player
    float speedSmoothTime = .1f;
    float speedSmoothVelocity;

    float walkSpeed = 2f;
    float runSpeed = 5f;
    float gravity = 9f; // now positive because Vector3.down

    /** Jump **/
    bool jumpCheck = true;
    float jumpHeight = 100.0f;
    float turnSmoothTime = .2f;
    float turnSmoothVelocity;

    [Range(0,1)]
    float airControlPercent; // TODO: reimplement
    //TODO: post video link that implements this for reference

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
        /* Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; */
        collider = GetComponent<CapsuleCollider>();
    }
//TODO: HAVE 2 COLLIDERS, A SMALL BOX AND A LARGE CAPSULE, THEN DISABLE THE LARGE CAPSULE DURING SLIDE
//USE .ENABLED() FUNCTION TO SET THE CAPSULE COLLIDERS STATUS


//TODO: FIND OUT WHY PLAYER ISN'T MOVING
//MAYBE THE SAME REASON WHY IT ROTATES INCORRECTLY

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
        /* if(!this.gameObject.activeSelf) {
            return;
        } */
        Inputs();
        Movements();
        /* Animations(); */ // no longer needed because it gets handled automatically now
        CheckForWall();

        UpdateMomentum();
        UpdateStamina();

        /* textBox.text = grounded.ToString() + " " + rb.velocity.x.ToString("f2") + " " + rb.velocity.y.ToString("f2") + " " + rb.velocity.z.ToString("f2"); */
    }

    void Inputs() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        normalInput = new Vector2(input.x, input.z).normalized;

        if(normalInput != Vector2.zero && !player.IsWallrunning()) // change direction of player depending on rotation of camera
        {
            float targetRotation = Mathf.Atan2(normalInput.x, normalInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
            // transform.localRotation = Quaternion.Euler(0, cameraT.transform.localRotation.eulerAngles.y, 0);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ToggleSprinting();
        }

        if(Input.GetKey(KeyCode.Space) && player.stamina >= 15.0f && jumpCheck) {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && player.momentum >= 0.4f) {
            Slide();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && (isWallRight || isWallLeft)) {
            StartWallrun();
        }

        /* if(Input.GetKey(KeyCode.A) && isWallLeft) {
            StartWallrun();
        } */

        if(Input.GetKey(KeyCode.Period)) {
            cameraSettings.HitButton();
        }

        if(Input.GetKey(KeyCode.L)) {
            PrintDebugInfo();
        }
    }

    void Movements()
    {           
        if(rb.velocity.y <= -1.5f && !grounded && !player.IsFalling()) { // check before applying gravity?
            player.action = ActionHandler.ActionType.FALLING;
            Debug.Log("Falling trigger");
        }

        rb.AddForce(Vector3.down * Time.deltaTime * gravity); // gravity

        // get speed multiplier
        float targetSpeed = walkSpeed;
        if(player.IsSprinting()) {
            targetSpeed = runSpeed;
        }
        targetSpeed *= normalInput.magnitude;
        /* targetSpeed *= 1.0f + player.momentum; */
        /* targetSpeed *= normalInput.magnitude; */ // I don't think this does anything
        player.speed = Mathf.SmoothDamp(player.speed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime)) * normalInput.magnitude;
        /* Debug.Log("Player speed: " + player.GetSpeed()); */

        /* Vector2 relativeVel = GetRelativePlayerVelocity();
        CounterMove(input.x, input.y, relativeVel, targetSpeed); */
        
        // reset things if grounded
        if(grounded) {
            jumpCheck = true;
            if(player.IsFalling() || player.IsWallrunning())
                player.action = ActionHandler.ActionType.WALK_RUN;
        }
        
        // set velocity of rigidbody
        /* player.velocity = (player.IsWallrunning() ? transform.TransformDirection(input) : Vector3.zero) * player.speed; */
        Vector3 vel = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = vel + transform.forward * player.speed;
        player.velocity = rb.velocity;
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

    void Jump()
    {
        if(grounded && !player.IsJumping()) {
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
            //collider.ENABLED = false;
        }else{
            //collider.ENABLED = true;
            player.action = ActionHandler.ActionType.WALK_RUN;
        }
    }

    void StartWallrun() {
        rb.useGravity = false;
        player.action = ActionHandler.ActionType.WALLRUN;
        Debug.Log("Happens");

        // Collision wall = 

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
        // rb.velocity = Vector3.zero;
    }

    void CheckForWall() {
        isWallRight = Physics.Raycast(transform.position, transform.right, 1f, wallLayer);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, 1f, wallLayer);

        if(!(isWallLeft || isWallRight) && player.IsWallrunning())
            StopWallrun();
    }

    bool IsFloor(Vector3 vec) {
        float angle = Vector3.Angle(Vector3.up, vec); // check angle between (deg) between y-axis and object
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    // Build-in unity function (MonoBehaviour)
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
        float delay = 2f;
        if(!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void OnCollisionEnter(Collision other) {
        int layer = other.gameObject.layer;
        if(wallLayer != (wallLayer | (1 << layer))) return;

        Vector3 normal = other.contacts[0].normal;
        transform.Rotate(transform.eulerAngles - normal, Space.Self);
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

    void PrintDebugInfo() {
        Debug.Log("Player Current Info:\n"
                + "Current Action: " + player.action + "\n"
                + "Current Velocity: " + player.velocity + "\n"
                + "Sprinting?: " + player.IsSprinting() + "\n"
                + "Sliding?: " + player.IsSliding() + "\n"
                + "Jumping?: " + player.IsJumping() + "\n"
                + "Wallrunning?: " + player.IsWallrunning() + "\n"
                + "Falling?: " + player.IsFalling() + "\n");
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
