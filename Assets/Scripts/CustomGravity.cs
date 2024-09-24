using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// SOURCE (24/09/2024): https://discussions.unity.com/t/why-does-rigidbody-3d-not-have-a-gravity-scale/645511/2
[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    // Gravity Scale editable on the inspector
    // providing a gravity scale per object

    [SerializeField] private float gravityScale = 1.0f;

    // Global Gravity doesn't appear in the inspector. Modify it here in the code
    // (or via scripting) to define a different default gravity for all objects.

    public static float globalGravity = -9.81f;

    Rigidbody m_rb;

    void OnEnable()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.useGravity = false;
    }

    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        m_rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
