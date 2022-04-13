using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayerData;

public class PlayerController : MonoBehaviour
{
    /** Debug **/
    public Text textBox;
    public bool DebugMode = true;

    /** Raycast **/
    float maxRaycastDistance = 2f;
    RaycastHit RayHit;
    bool isVaultable = false;
    bool rayTrigger = false;
    //Vector3 RaycastOffset = new Vector3(0, 1, 0);
    Vector3 RaycastOffset = new Vector3(0, 0, 0);

    /** Camera **/
    Transform cameraT;
    [SerializeField] camswitch cameraSettings;

    /** Player **/
    Animator animator;
    // Transform orientation;
    Rigidbody rb;
    Player player = new Player();

    /** Movement **/
    Vector3 normalVector = Vector3.up;
    Vector3 input;
    Vector2 normalInput; // normalized x and z movement

    float maxSlopeAngle = 35f;
    public bool grounded;
    float turnSmoothTime = .2f;
    float turnSmoothVelocity;
    float speedSmoothTime = .1f;
    float speedSmoothVelocity;

    float walkSpeed = 2f;
    float runSpeed = 5f;
    float gravity = 10f; // now positive because Vector3.down

    /** Jump **/
    bool jumpCheck = true;
    float jumpHeight = 150.0f;

    [Range(0,1)]
    public float airControlPercent;

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

//TODO: Create Vaulting mechanic using a raycast and tags for vaultable objects. 
// https://youtu.be/na96A7V6qbM
// https://www.youtube.com/watch?v=Hbo7vmsrABU

// RAYCAST TUTORIAL
// https://www.youtube.com/watch?v=CoTK39SZft8


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

        if(DebugMode)
            textBox.text = "rayTrigger = " + rayTrigger + ". isVaultable = " + isVaultable; 
    }

    public void Inputs() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        normalInput = new Vector2(input.x, input.z).normalized;

        if(normalInput != Vector2.zero) // change direction of player depending on rotation of camera
        {
            float targetRotation = Mathf.Atan2(normalInput.x, normalInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        Ray ray = new Ray(transform.position + RaycastOffset, transform.forward);
        rayTrigger = Physics.Raycast(ray, out RayHit, maxRaycastDistance);

        if(rayTrigger)
        {
            if(RayHit.collider.tag == "Vaultable")
                isVaultable = true;
            else
                isVaultable = false;
        }
        else
            isVaultable = false;

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ToggleSprinting();
        }

        if(Input.GetKey(KeyCode.Space) && player.stamina >= 15.0f && jumpCheck) {
            if(isVaultable)
                Vault();
            else
                Jump();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && player.momentum >= 0.4f) {
            Slide();
        }

        if(Input.GetKey(KeyCode.D) && isWallRight) {
            StartWallrun();
        }

        if(Input.GetKey(KeyCode.A) && isWallLeft) {
            StartWallrun();
        }

        // if(Input.GetKey(KeyCode.Period)) {
        //     cameraSettings.HitButton();
        // }
    }
    public void camOnClick(){
        cameraSettings.HitButton();
        Debug.Log("reached");
    }

    void Movements()
    {           
        if(rb.velocity.y <= -1.1f && !grounded) { // check before applying gravity?
            player.action = ActionHandler.ActionType.FALLING;
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
        Vector3 vel = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = vel + transform.forward * player.speed;
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

    void OnDrawGizmos()
    {
        if(DebugMode)
        {
            if(rayTrigger)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position + RaycastOffset, transform.forward * RayHit.distance);
            }
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + RaycastOffset, transform.forward * maxRaycastDistance);
            }
        }
    }
    void Vault()
    {
        if(player.action != ActionHandler.ActionType.VAULTING && player.action != ActionHandler.ActionType.JUMP && grounded)
        {
            player.action = ActionHandler.ActionType.VAULTING;

            rb.AddForce(Vector3.up * jumpHeight);
            rb.AddForce(normalVector * jumpHeight * 0.5f);
            rb.AddForce(transform.forward * 100.0f);

            //in the actionhandler it should play the animation and change colliders
        }
    }
    void Jump()
    {
        if(player.action != ActionHandler.ActionType.JUMP && player.action != ActionHandler.ActionType.VAULTING && grounded)
        {
            jumpCheck = false;
            player.action = ActionHandler.ActionType.JUMP;

            rb.AddForce(Vector3.up * jumpHeight);
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
