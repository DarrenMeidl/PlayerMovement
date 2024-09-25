using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// SOURCE (24/09/2024): https://discussions.unity.com/t/why-does-rigidbody-3d-not-have-a-gravity-scale/645511/2
[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    //Scriptable object which holds all the player's movement parameters
    public GravityData Data; // the player data scriptable object   
    public PlayerData pData; // the player data scriptable object 
    // Gravity Scale editable on the inspector
    // providing a gravity scale per object
    private float gravityScale;

    // Global Gravity doesn't appear in the inspector. Modify it here in the code
    // (or via scripting) to define a different default gravity for all objects.

    public static float globalGravity = -9.81f;

    Rigidbody rb;
    PlayerMovement pm;
    private bool onPlayer;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        pm = GetComponent<PlayerMovement>();
        if (pm != null)
        {
            onPlayer = true;
        }
        
    }

    void Update()
    {
        if (onPlayer)
        {
            if (pm.isSliding)
            {
                gravityScale = 0;
            }
            else if (pm.isJumpCut)
            {
                //Higher gravity if jump button released
                gravityScale = (Data.gravityScale * Data.jumpCutGravityMult);
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed), rb.velocity.z);
                gravityScale = Data.jumpCutGravityMult;
            }
            //Higher gravity if we've released the jump input or are falling
            else if ((pm.isJumping || pm.isJumpFalling) && Mathf.Abs(rb.velocity.y) < pData.jumpHangTimeThreshold)
            {
                gravityScale = (Data.gravityScale * pData.jumpHangGravityMult);
            }
            // if the player is falling
            else if (rb.velocity.y < -.1f)
            {
                //Higher gravity if falling
                gravityScale = (Data.gravityScale * Data.fallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed), rb.velocity.z);
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                gravityScale = Data.fallGravityMult;
            }
                
        }
    }
    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
