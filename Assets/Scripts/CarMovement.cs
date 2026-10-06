using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    [Header("Speed")]
    public float maxSpeed = 20f;            
    public float maxReverseSpeed = 6f;      
    public float timeToMaxSpeed = 3f;       

    public AnimationCurve accelerationCurve = new AnimationCurve(
        new Keyframe(0f, 1.5f),
        new Keyframe(1f, 0.5f));

    [Header("Deceleration")]
    public float brakeDeceleration = 25f;   
    public float coastDeceleration = 5f;

    
    [SerializeField]
    private float OldMaxSpeed = 0;

    [Header("Steering")]
    public float turnSpeed = 120f;          
    public float minSpeedForFullSteering = 5f; 

    //[Header("Input")]
    //[Range(-1f, 1f)] [SerializeField] private float throttleInput;  
    //[Range(-1f, 1f)] [SerializeField] private float steerInput;     
    //[SerializeField] private bool brakeInput;

    [Header("Input")]
    [Range(-1f, 1f)] private float throttleInput;  
    [Range(-1f, 1f)] private float steerInput;     
    private bool brakeInput;

    [SerializeField]
    public float BoostAmount = 0f;

    private Rigidbody rb;
    private float currentSpeed;             

    // Read only, for it to convert to units we can see and use in the future
    public float CurrentSpeed => currentSpeed;
    public float SpeedNormalized => Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);

    
    public void SetInput(float throttle, float steer)
    {
        throttleInput = Mathf.Clamp(throttle, -1f, 1f);
        steerInput = Mathf.Clamp(steer, -1f, 1f);
    }

    public void SetBrake(bool braking)
    {
        brakeInput = braking;
    }

    public void ResetMovement()
    {
        currentSpeed = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Ensures the car is not tipping over
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        currentSpeed = Vector3.Dot(rb.linearVelocity, rb.rotation * Vector3.forward);

        UpdateSpeed(dt);
        Quaternion newRotation = UpdateSteering(dt);

        Vector3 velocity = newRotation * Vector3.forward * currentSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    public void ApplyBoost(float boostAmount, float boostDuration)
    {
        OldMaxSpeed = maxSpeed; // Store the original maxSpeed
        maxSpeed += boostAmount; // Increase maxSpeed by the boost amount
        StartCoroutine(BoostCoroutine(boostDuration)); // Start the coroutine to reset the boost after the specified duration

    }

    IEnumerator BoostCoroutine(float boostDuration)
    {
        yield return new WaitForSeconds(boostDuration); // Wait for the specified duration
        maxSpeed = OldMaxSpeed; // Reset maxSpeed to its original value
    }

    private void UpdateSpeed(float dt)
    {
        if (brakeInput)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeDeceleration * dt);
            return;
        }
        float BoostedSpeed = currentSpeed + BoostAmount;

        //BoostedSpeed = Mathf.Round(BoostedSpeed * 1f) / 1f; // Round to 2 decimal places
        //Debug.Log("Boosted Speed: " + BoostedSpeed);

        

        bool goingForward = currentSpeed > 0.1f * BoostedSpeed;
        bool goingBackward = currentSpeed < -0.1f;

        if (throttleInput > 0f && goingBackward || throttleInput < 0f && goingForward)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeDeceleration * dt);
        }
        else if (throttleInput != 0f)
        {
            float targetSpeed = throttleInput > 0f ? throttleInput * maxSpeed: throttleInput * maxReverseSpeed;

            if (Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed))
            {
                float baseAccel = maxSpeed / timeToMaxSpeed;
                float accel = baseAccel * accelerationCurve.Evaluate(SpeedNormalized);
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * dt);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, coastDeceleration * dt);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, coastDeceleration * dt);
        }
    }

    private Quaternion UpdateSteering(float dt)
    {
        if (steerInput == 0f || currentSpeed == 0f)
            return rb.rotation;

        float steerFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / minSpeedForFullSteering);
        float direction = Mathf.Sign(currentSpeed);

        float yaw = steerInput * turnSpeed * steerFactor * direction * dt;
        Quaternion newRotation = rb.rotation * Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(newRotation);
        return newRotation;
    }
}
