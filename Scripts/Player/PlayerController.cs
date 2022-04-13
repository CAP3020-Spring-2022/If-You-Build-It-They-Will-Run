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
    public bool debugMode = true;

    /** Raycast **/
    float maxRaycastDistance = 2.2f;
    RaycastHit rayHit;
    bool isVaultable = false;
    bool rayTrigger = false;
    Vector3 RaycastOffset = Vector3.zero;

    /** Camera **/
    Transform cameraT;
    [SerializeField] camswitch cameraSettings;

    /** Player **/
    Animator animator;
    Rigidbody rb;
    Collider capsuleCollider;
    Player player = new Player();

    /** Movement **/
    Vector3 normalVector = Vector3.up;
    Vector3 input;
    Vector2 normalInput; // normalized x and z movement

    float maxSlopeAngle = 35f;
    float speedSmoothTime = .1f;
    float speedSmoothVelocity;

    float walkSpeed = 2f;
    float runSpeed = 5f;
    float gravity = 10f; // now positive because Vector3.down

    /** Jump **/
    bool jumpCheck = true;
    float jumpHeight = 25.0f;
    float turnSmoothTime = .2f;
    float turnSmoothVelocity;

    [Range(0,1)]
    public float airControlPercent;

    /** Slide **/
    float slideForce = 30f;

    /** Wallrun **/
    float wallrunForce = 200f;
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

    void FixedUpdate() {
        Movements();

        UpdateMomentum();
        UpdateStamina();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        CheckForWall();

        if(debugMode)
            textBox.text = "rayTrigger = " + rayTrigger + ". isVaultable = " + isVaultable;
    }

    public void Inputs() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        normalInput = new Vector2(input.x, input.z).normalized;

        /* rb.velocity = Vector3.zero; */

        if(normalInput != Vector2.zero && !player.IsWallrunning()) // change direction of player depending on rotation of camera
        {
            float targetRotation = Mathf.Atan2(normalInput.x, normalInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        Ray ray = new Ray(transform.position + RaycastOffset, transform.forward);
        rayTrigger = Physics.Raycast(ray, out rayHit, maxRaycastDistance);

        if(rayTrigger)
        {
            if(rayHit.collider.tag == "Vaultable")
                isVaultable = true;
            else
                isVaultable = false;
        }
        else
            isVaultable = false;

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ToggleSprinting();
        }

        if(Input.GetKey(KeyCode.Space) && player.stamina >= 15.0f && (jumpCheck 
        || player.onWall)) {
            if(isVaultable)
                player.SetVaulting(true);
            else
                player.SetJumping(true);
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && player.momentum >= 0.1f && player.grounded) {
            player.SetSliding(true);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && player.onWall) {
            player.SetWallrunning(true);
        }

        if(Input.GetKey(KeyCode.L)) {
            PrintDebugInfo();
        }
    }

    void Movements()
    {           
        HandleInputs();
        ApplyGravity();
        FallingCheck();
        
        float targetSpeed = GetTargetSpeed();
        targetSpeed *= normalInput.magnitude;
        
        /* targetSpeed *= 1.0f + player.momentum; */
        /* targetSpeed *= normalInput.magnitude; */ // I don't think this does anything
        player.speed = Mathf.SmoothDamp(player.speed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime)) * normalInput.magnitude;

        /* Vector2 relativeVel = GetRelativePlayerVelocity();
        CounterMove(input.x, input.y, relativeVel, targetSpeed); */
        
        // reset things if grounded
        if(player.grounded) {
            jumpCheck = true;
            if(player.IsFalling() || player.IsWallrunning()) {
                player.action = ActionHandler.ActionType.WALK_RUN;
                rb.useGravity = true;
            }
        }
        
        // set velocity of rigidbody
        player.velocity = transform.TransformDirection(normalInput) * player.speed;
        Vector3 vel = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = vel + transform.forward * player.speed;
    }

    private float GetTargetSpeed() {
        if(player.IsSprinting())
            return runSpeed;
        else
            return walkSpeed;
    }

    private void HandleInputs() {
        if(player.IsJumping()) {
            Jump();
        }
        if(player.IsSliding()) {
            Slide();
        }
        if(player.IsWallrunning()) {
            Wallrun();
        }
    }

    private void VaultCheck() {
        Ray ray = new Ray(transform.position + RaycastOffset, transform.forward);
        rayTrigger = Physics.Raycast(ray, out rayHit, maxRaycastDistance);

        if(rayTrigger) {
            if(rayHit.collider.tag == "Vaultable")
                isVaultable = true;
            else
                isVaultable = false;
        }else
            isVaultable = false;
    }

    private void ApplyGravity() {
        if(!player.grounded) {
            rb.AddForce(Vector3.down * Time.fixedDeltaTime * gravity);
        }
    }

    private void FallingCheck() {
        if(rb.velocity.y >= -1.1f 
          && !player.grounded 
          && !player.IsFalling() 
          && !player.IsWallrunning()) {
            player.SetFalling(true);
        }
    }

    void OnDrawGizmos() {
        if(debugMode) {
            if(rayTrigger) {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position + RaycastOffset, transform.forward * rayHit.distance);
            }else{
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + RaycastOffset, transform.forward * maxRaycastDistance);
            }
        }
    }

    public void Vault() {
        rb.AddForce(Vector3.up * jumpHeight);
        rb.AddForce(normalVector * jumpHeight * 0.5f);
        //rb.AddForce(transform.forward * 1000.0f);
        transform.Translate(transform.forward);
    }
    
    void Jump()
    {
        if(player.grounded) {
            jumpCheck = false;
            rb.AddForce(Vector3.up * jumpHeight * 1.5f);
            rb.AddForce(normalVector * jumpHeight * 0.5f);
        }else if(player.onWall) {
            rb.AddForce(Vector3.up * jumpHeight * 0.5f);
            rb.AddForce(normalVector * jumpHeight * 1.5f);
        }
    }

    void Slide() {
        if(/* rb.velocity.magnitude > 0.5f &&  */player.grounded) {
            // player.action = ActionHandler.ActionType.SLIDE;
            rb.AddForce(transform.forward * slideForce);
        }else{
            //collider.ENABLED = true;
            // player.action = ActionHandler.ActionType.WALK_RUN;
        }
    }

    void Wallrun() {
        rb.AddForce(new Vector3(0.0f, 1.0f, 0.0f));
        rb.useGravity = false;
        // player.action = ActionHandler.ActionType.WALLRUN;
         // Debug.Log("Happens");
        
        /* if(rb.velocity.magnitude <= maxWallrunSpeed) { */
            rb.AddForce(transform.forward * wallrunForce);

            // stick character to wall
            if(isWallRight)
                rb.AddForce(transform.right * wallrunForce / 10 * Time.deltaTime);
            else if(isWallLeft)
                rb.AddForce(-transform.right * wallrunForce / 10 * Time.deltaTime);
        // }

        if(rb.velocity.x == 0.0f && rb.velocity.z == 0.0f)
            StopWallrun();
    }

    void StopWallrun() {
        player.action = ActionHandler.ActionType.WALK_RUN;
        rb.useGravity = true;
    }

    void CheckForWall() {
        isWallRight = Physics.Raycast(transform.position, transform.right, 1f, wallLayer);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, 1f, wallLayer);

        if(isWallLeft || isWallRight)
            player.onWall = true;
        else
            player.onWall = false;

        if(!player.onWall && player.IsWallrunning())
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
                player.grounded = true;
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
        player.grounded = false;
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
        Debug.Log(
                  "-------PLAYER INFO-------\n"
                + "ACTION       : " + player.action + "\n"
                + "VELOCITY     : " + player.velocity + "\n"
                + "GROUNDED     : " + player.grounded + "\n"
                + "EULER ANGLES : " + transform.eulerAngles + "\n"
                + "------MOVEMENT TYPES------\n"
                + "SPRINTING   : " + player.IsSprinting() + "\n"
                + "SLIDING     : " + player.IsSliding() + "\n"
                + "JUMPING     : " + player.IsJumping() + "\n"
                + "WALLRUNNING : " + player.IsWallrunning() + "\n"
                + "FALLING     : " + player.IsFalling() + "\n");
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if(player.grounded)
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
