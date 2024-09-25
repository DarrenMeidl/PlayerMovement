using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//using Cinemachine;
public class PlayerMovement : MonoBehaviour
{
    //Scriptable object which holds all the player's movement parameters
    public PlayerData Data; // the player data scriptable object   

#region Player Components
    public Rigidbody rb { get; private set; } // rigidbody of the player
    public CapsuleCollider playerCollider { get; private set; } // only this script can access the box collider of the player
    public PlayerInput input { get; private set; }
    #endregion

#region Serialized Variables / Components
    // [Header("Player Particles")]
    // [SerializeField] private ParticleSystem jumpDust; // particle system for the jump dust
    // [SerializeField] private ParticleSystem walkDust; // particle system for the walk dust
    // [SerializeField] private ParticleSystem landDust; // particle system for the land dust
    // [SerializeField] private ParticleSystem deathParticle; // particle system for the death particle
    // [SerializeField] private ParticleSystem wallJumpDust; // particle system for the wall jump dust

    // [Header("Player Animation")] 
    // [SerializeField] private Animator anim; // only this script can access the animator of the player
    public Transform orientation;
    
#endregion

#region State Variables
    // Variables control the various actions the player can perform at any time.
    // These are fields which can are public allowing for other sctipts to read them
    // but can only be privately written to.
    public bool isJumping { get; private set; } // is the player jumping?
    public bool isWallJumping { get; private set; } // is the player wall jumping?
    public bool isSliding { get; private set; } // is the player sliding?

    // Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; } // last time the player was on the ground
    public float LastOnWallTime { get; private set; }

    // Jump Variables
    [HideInInspector] public bool isJumpCut; // is the player jump cutting? - public so other scripts can read & set it
    private bool isJumpFalling; // is the player jump falling?
    
    public float airMultiplier;

    // Wall Jump
    private float _wallJumpStartTime;
	private int _lastWallJumpDir;

    private float accelRate; // the rate of acceleration of the player
    private float force; // the force of the player
    
    // Wall Slide
    private float wallSlideSpeed; // the speed of the wall slide
    
    // Wall Jump
    private float wallJumpingDirection; // the direction of the wall jump
    private float wallJumpingCounter; // the counter of the wall jump
    private Vector2 wallJumpingPower = new Vector2(10f, 14f); // the power of the wall jump
#endregion

#region Input Variables
    InputAction walkAction; // access to walk actions
    public float LastPressedJumpTime { get; private set; } // last time the player pressed the jump button
    Vector3 walkDir; // determine direction to walk in

    //private CharacterController _characterController;
    #endregion

#region Check Variables
    //   [Header("Player Checks")] 
    //   [SerializeField] private Transform wallCheck; // the point where the player checks if there is a wall
    //   [SerializeField] private Transform crouchCeilingPoint; // the point where the player's head is when crouched
    //   [SerializeField] private float ceillingRadius = .2f; // the radius of the circle that checks if the player is touching the ceiling
    //   [Space(5)]
    //   [SerializeField] private Transform _frontWallCheckPoint;
    //[SerializeField] private Transform _backWallCheckPoint;
    //[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
#endregion

#region Layers & Tags

    [Header("Layers & Tags")] 
    [SerializeField] private LayerMask jumpableGround; // layermask for the ground we can jump on
    [SerializeField] private LayerMask jumpableWall; // layermask for the wall we can jump on

#endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // get the rigidbody of the player object
        playerCollider = GetComponent<CapsuleCollider>(); // get the box collider of the player object
        input = GetComponent<PlayerInput>(); // get PlayerInput of the player object
        #region Actions
        walkAction = input.actions.FindAction("Walk");
        #endregion
    }   
   
    private void Update()
    {
        #region Timers
        LastOnGroundTime -= Time.deltaTime; // decrease the last on ground time by the time
        LastOnWallTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime; // decrease the last pressed jump time by the time
        #endregion

        #region Input Handler

        // code for handling movement like walk, jump, wall jump, slide etc.
        if (Keyboard.current.spaceKey.isPressed)
        {
            OnJumpDown();
        }
        else if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            OnJumpUp();
        }
       
        #endregion
        
        #region Collision Checks
        // if the player is not jumping
        if (!isJumping){
            // if the player is walking, not crouched and the walk dust particle isn't playing
            
                //walkDust.Play(); // play the walk dust particle
            
            // if the player isn't walking or in the air, stop the particle
            
                //walkDust.Stop(); // stop the walk dust particle
            
            // if the player is touching the ground, then the last time the player was on the ground is the coyote time
            if (IsGrounded()){
                // if the player has just landed
                if(LastOnGroundTime < -0.1f){ 
                    //landDust.Play(); // play the landing particle
                    //AudioManager.Instance.PlayPlayerSFX("Player Land"); // play the landing sound
                    // possible code for a just landed animation
                }
                LastOnGroundTime = Data.coyoteTime; // set the last on ground time to the coyote time
            }

            //Right Wall Check
			
			//Left Wall Check
            
        }
        #endregion
        
        #region Ground Jump Checks
        // if the player is considered jumping but is falling
        if (isJumping && rb.velocity.y < 0){
            isJumping = false; // set the player to not be jumping
            isJumpFalling = true; // set the player to be jump falling
        }
        // if the player is not jumping and the last time the player was on the ground is bigger than 0
        if (LastOnGroundTime > 0 && !isJumping && !isWallJumping){ 
            isJumpCut = false; // set the player to not be jump cutting
            isJumpFalling = false; // set the player to not be jump falling
        }
        // Set the jump bools & check if the player can actually jump
        // if they can jump then perform the jump function
        if (CanJump() && LastPressedJumpTime > 0)
        { 
            isJumping = true; // player is now jumping
            isWallJumping = false; // player is still not wall jumping
            isJumpCut = false; // player still can't jump cut
            isJumpFalling = false; // player isn't jump falling
            Jump(); // perform the jump
        }
        #endregion

        #region Slide Checks
        // if the player can slide and is moving in the direction of the wall
        // isSliding = true;
        // else
        // isSliding = false;
        #endregion

        #region Walk Checks
        SpeedControl(); 
        #endregion      

        #region Drag Checks
        rb.drag = Data.groundDrag;
        if (IsGrounded())
            rb.drag = Data.groundDrag;       
        else
            rb.drag = 0;
        #endregion
        
        #region Gravity
        /*//Higher gravity if we've released the jump input or are falling
        if (isSliding)
        {
            SetGravityScale(0);
        }
        else if (isJumpCut)
        {
            //Higher gravity if jump button released
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed), rb.velocity.z);
        }
        else if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        // if the player is falling
        else if (rb.velocity.y < -.1f)
        {
            //Higher gravity if falling
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed), rb.velocity.z);
        }
        else
        {
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(Data.gravityScale);
        }
        */
        #endregion

    }

    private void FixedUpdate() {
        HandleWalk();
        HandleSlide();
    }

    

#region Handle Inputs
    //Methods which whandle inputs detected & called from Update()
    public void OnJumpDown() { // if the player is pressing down the jump button
        LastPressedJumpTime = Data.jumpInputBufferTime; // set the last pressed jump time to the time of the jump input buffer
    }

    // If this function is being called it means the player is releasing the jump button
    public void OnJumpUp()
    {
        Debug.Log("CANCELLING JUMP..");
        // so if the player is rising up in the air & their bool says they're currently gronund jumping
        // that means you can cut the ground jump
        if (CanJumpCut())
        {
            // because the player has already released the jump button mid-ground jump,
            // they want to cancel the ground jump,
            // so we set the jump cut bool to true straight away
            Debug.Log("CANCEL JUMP..");
            isJumpCut = true;
        }
    }
#endregion
#region General Functions
    private void Sleep(float duration) {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration) {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
	}
#endregion
#region Bool Functions
    private bool CanJump() // checks if player can ground jump
    {
		return LastOnGroundTime > 0 && !isJumping;
    }
    private bool CanJumpCut() { // checks if player can cancel the ground jump early (jump cutting)
		return isJumping && rb.velocity.y > .1f;
    }
    private bool IsGrounded(){ // checks if the player is grounded
        // creates a new box the position & size of the player's crouching collider, 0 means it can't rotate, moves down by .1f, is it colliding with jumpable ground?
        return Physics.Raycast(transform.position, Vector3.down, Data.height * .5f + .2f, jumpableGround); // check if the player is grounded (if the player is touching the ground, return true, else return false)
    }
    public bool CanSlide()
    {
		if (LastOnWallTime > 0 && !isJumping && !isWallJumping && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}
    
#endregion

// MOVEMENT FUNCTIONS
#region Walk Functions
    // Function to handle the walking of the player
    private void HandleWalk(){
        Walk(1); // walk the player            
    }
    // Walk function
    private void Walk(float lerpAmount){
        CalculateMoveSpeed(); // calculate the movement speed of the player

        Vector2 direction = walkAction.ReadValue<Vector2>();
        /*// OLD WALK CALCULATIONS
        float targetSpeed = direction.x * Data.walkMaxSpeed; // set the target speed to the direction the player is moving times the move speed
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount); // lerp the target speed

        #region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
		#endregion
        #region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion
        #region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if(Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0; 
		}
		#endregion

        float speedDif = targetSpeed - rb.velocity.x; // the difference between the move speed and the velocity of the player
        float movement = speedDif * accelRate; // the movement of the player
        */
        walkDir = orientation.forward * direction.y + orientation.right * direction.x;
        if (LastOnGroundTime >= 0)
            rb.AddForce(walkDir.normalized * Data.walkMaxSpeed * 10f, ForceMode.Force); // add force to the player in the x direction
        else if (LastOnGroundTime < 0)
            rb.AddForce(walkDir.normalized * Data.walkMaxSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    // Function to calculate the moveSpeed variable of the player
    private void CalculateMoveSpeed(){
        Data.walkMaxSpeed = Data.moveSpeed; // if the player is crouched, set the max speed to crouch speed, else set it to move speed
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > Data.walkMaxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * Data.walkMaxSpeed; // calculate what max velocity would be
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); // apply it
        }
    }
    #endregion
#region Jump Functions
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press during small time window of the coyote or jump buffer time
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * Data.jumpForce, ForceMode.Impulse); // add force to the player in the y direction
        //AudioManager.Instance.PlayPlayerSFX("Player Jump"); // play the jump sound
        Debug.Log("JUMPED");
        //jumpDust.Play(); // play the jump dust particle
    }
#endregion
#region Other Movement Functions
    private void HandleSlide()
    {
        // Handle Slide
        if (isSliding)
        {
            Slide();
        }
    }
    private void Slide(){
        //We remove the remaining upwards Impulse to prevent upwards sliding
		if(rb.velocity.y > .1f)
		{
		    rb.AddForce(-rb.velocity.y * Vector2.up,ForceMode.Impulse);
		}
	
		//Works the same as the Run but only in the y-axis
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		float speedDif = Data.slideSpeed - rb.velocity.y;	
		float movement = speedDif * -Data.slideAccel;
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		rb.AddForce(movement * Vector2.up);
    }
#endregion

}