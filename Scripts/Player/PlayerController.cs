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
    bool isVaultable = false;
    RaycastHit rayHit;
    RaycastHit rayHitBack;
    bool rayTrigger = false;
    bool backRayTrigger = false;
    //Set on Awake
    Vector3 vaultingCastOffset;
    float vaultingCastDistance = 3.2f;

    Vector3 defaultCastOffset = new Vector3(0, 0.5f, 0);
    float defaultCastDistance = 2.2f;
    //Set on Awake
    float maxRaycastDistance;
    //Set on Awake
    Vector3 RaycastOffset;


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

    /** Vault **/
    public float vaultHeight = 85;

    /** World **/
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        vaultingCastOffset = -1 * transform.forward;
        maxRaycastDistance = vaultingCastDistance;
        RaycastOffset = defaultCastOffset;
    }

// TODO(Leo): Adjust vaulting and sliding to feel right
// PROBLEM WITH VAULT: As soon as the isVaultable Raycast no longer detects a vaulatble object in front of the player, ie. in the middle of the animation when the ray is passing through or
// over the vaultable object, isVaultable turns false and the player.action state gets set equal to a different state. The change of state here is an issue because it causes the animation to
// snap quickly instead of transition and it also swaps to the standingCollider too quickly

// SOLUTION: maybe during the vaulting state, set the position of the raycast further back and maybe lower. This might work because the ray would be lower than the capsule collider and wouldn't
// interact with it, and then when the player is over the object the ray would enter the object and no longer be able to detect it. THIS MAY NOT WORK 

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
            textBox.text = "debug";
        else
            textBox.text = " ";
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

        VaultCheck();

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ToggleSprinting();
        }

// TODO: Maybe switch GetKey to GetKeyDown and change the WALK_RUN to trigger closer to hitting the ground 
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
                isVaultable = false;
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
        if(player.IsVaulting()) {
            Vault();
        }
    }

    private void VaultCheck() {
        Ray ray = new Ray(transform.position + RaycastOffset, transform.forward);
        rayTrigger = Physics.Raycast(ray, out rayHit, maxRaycastDistance);

        Ray backRay = new Ray(transform.position + RaycastOffset, -1 * transform.forward);
        backRayTrigger = Physics.Raycast(backRay, out rayHitBack, maxRaycastDistance);

        if(rayTrigger && rayHit.collider.tag == "Vaultable")
            isVaultable = true;
        else if(backRayTrigger && rayHitBack.collider.tag == "Vaultable")
        {
            player.action = ActionHandler.ActionType.WALK_RUN;
            isVaultable = false;
        }
    }
    
    void OnDrawGizmos() {
        if(rayTrigger) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + RaycastOffset, transform.forward * rayHit.distance);
        }else{
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + RaycastOffset, transform.forward * maxRaycastDistance);
        }

        if(backRayTrigger) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + RaycastOffset, -1 * transform.forward * rayHitBack.distance);
        }else{
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + RaycastOffset, -1 * transform.forward * maxRaycastDistance);
        }
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

    public void Vault() {
        if(player.grounded)
        {
            jumpCheck = false;
            rb.AddForce(Vector3.up * vaultHeight);
            //rb.AddForce(Vector3.up * vaultHeight * 1.5f);
            //rb.AddForce(normalVector * vaultHeight * 0.5f);
            // rb.AddForce(transform.forward * 1000.0f);
            // transform.Translate(transform.forward);
        }
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

        if(isWallLeft || isWallRight) {
            player.onWall = true;
            player.wallRight = isWallRight;
        }else {
            player.onWall = false;
            player.wallRight = false;
        }

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
            Invoke(nameof(StopGrounded), Time.fixedDeltaTime * delay);
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
                    nm -= 0.01f;
                else
                    if(player.IsSprinting())
                    {
                        if(nm < 1.6f)
                        nm += 0.00175f;
                }      
                    else
                        nm -= 0.015f;
                break;
            case ActionHandler.ActionType.JUMP:
                if(nm < 1.6f)
                nm += 0.015f;
                break;
            case ActionHandler.ActionType.SLIDE:
                nm -= 0.0025f;
                break;
            case ActionHandler.ActionType.VAULT:
                nm += 0.0075f;
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
                    ns += 2.5f;
                else
                    if(player.IsSprinting())
                        ns -= 0.05f;
                    else
                        ns += 0.15f;
                break;
            case ActionHandler.ActionType.JUMP:
                ns -= 0.7f;
                break;
            case ActionHandler.ActionType.SLIDE:
                ns += 0.4f;
                break;
            case ActionHandler.ActionType.VAULT:
                ns -= 0.325f;
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
