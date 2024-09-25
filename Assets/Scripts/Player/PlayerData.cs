using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class PlayerData : ScriptableObject {
    [Header("Walk")]
    public float walkMaxSpeed; //Target speed we want the player to reach.
	public float moveSpeed; //The speed at which our player moves.
	public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    [HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
    public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	public float groundDrag;
    [Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	[Range(0f, 1)] public float deccelInAir;
	public bool doConserveMomentum = true;

    [Space(20)] // Space between the two headers

	[Header("All Jumps")]
	public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
	[Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
	public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	[Space(0.5f)]
	public float jumpHangAccelerationMult; 
	public float jumpHangMaxSpeedMult; 

    [Header("Jump")]
	public float jumpHeight; //Height of the player's jump
	public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
	//[HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.	
	public float jumpForce;

	[Header("Slide")]
	public float slideSpeed;
	public float slideAccel;
	
	[Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Space(20)]

	[Header("Other")]
	public float height;
    //Unity Callback, called when the inspector updates
    private void OnValidate()
    {
		// RUN
		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / walkMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / walkMaxSpeed;
        
        // JUMP
        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		//jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        
		#region Acceleration Ranges
        // These clamps ensure that the acceleration values are within a certain range
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, walkMaxSpeed); //Clamp the runAcceleration value between 0.01 and walkMaxSpeed
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, walkMaxSpeed); //Clamp the runDecceleration value between 0.01 and walkMaxSpeed
		#endregion
	}
}
