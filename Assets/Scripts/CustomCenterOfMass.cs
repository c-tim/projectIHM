using UnityEngine;

public class CustomCenterOfMass : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        // Set the center of mass relative to the transform's origin
        rb.centerOfMass = new Vector3(0f, -1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
