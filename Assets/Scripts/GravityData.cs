using UnityEngine;

[CreateAssetMenu(menuName = "Gravity Data")]
public class GravityData : ScriptableObject {
    [Header("Gravity")]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
    [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
    [HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).

    [Space(10)] // Space between the two headers

	[Header("Player Jumps")]
	public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
	//[Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump

    private void OnValidate()
	{ 
		// OLD GRAVITY
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		//gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		//gravityScale = gravityStrength / Physics2D.gravity.y;
	}
}
