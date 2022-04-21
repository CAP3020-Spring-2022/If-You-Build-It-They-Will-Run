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
    RaycastHit rayHit;
    bool rayTrigger = false;

    RaycastHit rayHitBack;
    bool backRayTrigger = false;

    RaycastHit rayHitDown;
    bool downRayTrigger = false;

    float maxRaycastDistance = 3.2f;
    float downRaycastDistance = 3f;
    Vector3 RaycastOffset = new Vector3(0, 0.5f, 0);


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

    float maxSlopeAngle = 45f;
    float speedSmoothTime = .1f;
    float speedSmoothVelocity;

    float walkSpeed = 2f;
    float runSpeed = 5f;
    float gravity = 10f; // now positive because Vector3.down

    float momentumMin = 0.01f;

    /** Jump **/
    bool jumpCheck = true;
    float jumpHeight = 25.0f;
    float turnSmoothTime = .2f;
    float turnSmoothVelocity;

    [Range(0,1)]
    public float airControlPercent = .125f;

    /** Slide **/
    float slideForce = 30f;

    /** Wallrun **/
    float wallrunForce = 200f;
    float maxWallrunTime;
    float maxWallrunSpeed;
    bool isWallRight, isWallLeft;

    /** Vault **/
    bool isVaultable = false;
    float vaultHeight = 85;

    /** Roll **/
    bool isRollable = false;


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
            textBox.text = "downRayTrigger = " + downRayTrigger + " isRollable = " + isRollable;
        else
            textBox.text = " ";
    }

    public void Inputs() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        normalInput = new Vector2(input.x, input.z).normalized;

        /* rb.velocity = Vector3.zero; */

        if(normalInput != Vector2.zero && !player.IsWallrunning()) // change direction of player depending on rotation of camera
        {
            if(player.action == ActionHandler.ActionType.SLIDE || player.action == ActionHandler.ActionType.VAULT || player.action == ActionHandler.ActionType.ROLL)
                turnSmoothTime = .5f;
            else
                turnSmoothTime = .1f;
            float targetRotation = Mathf.Atan2(normalInput.x, normalInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        VaultCheck();
        RollCheck();

        if(Input.GetKeyDown(KeyCode.LeftShift))
            player.ToggleSprinting();
        if(player.stamina < 15f)
            player.SetSprinting(false);
        

// TODO: Maybe switch GetKey to GetKeyDown and change the WALK_RUN to trigger closer to hitting the ground 
        if(Input.GetKey(KeyCode.Space) && player.stamina >= 15.0f && (jumpCheck || player.onWall)) {
            if(isVaultable)
                player.SetVaulting(true);
            else
                player.SetJumping(true);
        }

//TODO: MAKE THESE TWO IF's, ONE IS KEYDOWN OTHER IS KEY
        if(Input.GetKeyDown(KeyCode.LeftControl) && player.momentum >= momentumMin && player.grounded && player.action != ActionHandler.ActionType.ROLL)
            player.SetSliding(true);
        else if(Input.GetKeyDown(KeyCode.LeftControl) && player.momentum >= momentumMin && isRollable && player.action != ActionHandler.ActionType.SLIDE)
            player.SetRolling(true);

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
            isVaultable = false;
            //isRollable = false;
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
        if(player.IsVaulting()) {
            Vault();
        }
        if(player.IsRolling()) {
            Roll();
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

    private void RollCheck()
    {
        Ray downRay = new Ray(transform.position + RaycastOffset, -2 * RaycastOffset);
        downRayTrigger = Physics.Raycast(downRay, out rayHitDown, downRaycastDistance);

//YOU CAN'T EXCLUDE ISGROUNDED FROM THE CONDITION BECAUSE IF NOT THE ANIMATION WON'T TRANSITION PROPERLY
        if(downRayTrigger && rayHitDown.transform.gameObject.layer == 8 && rayHitDown.distance <= downRaycastDistance)
            isRollable = true;
        else
            isRollable = false;
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

        if(downRayTrigger)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + RaycastOffset, -2 * RaycastOffset * rayHitDown.distance);
        }else{
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + RaycastOffset, -2 * RaycastOffset * downRaycastDistance);
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
          && !player.IsWallrunning()
          && !player.IsRolling()) {
            player.SetFalling(true);
        }
    }

    public void Vault() {
        if(player.grounded)
        {
            jumpCheck = false;
            rb.AddForce(Vector3.up * vaultHeight);
        }
    }

    public void Roll() {
        //stamina boost
        //momentum lost
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
