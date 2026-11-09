using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    [Header("Speed")]
    public float maxSpeed = 20f;            
    public float maxReverseSpeed = 6f;      
    public float timeToMaxSpeed = 3f;

    public float timeToTurnMaxSpeed = 3f;

    [SerializeField]
    horseAnim anim;

    [SerializeField]
    [Range(0, 1f)]
    private float inertia; 

    public AnimationCurve accelerationCurve = new AnimationCurve(
        new Keyframe(0f, 1.5f),
        new Keyframe(1f, 0.5f));

    [Header("Deceleration")]
    public float brakeDeceleration = 25f;   
    public float coastDeceleration = 5f;

    
    [SerializeField]
    private float OldMaxSpeed = 0;

    [Header("Steering")]
    public float turnSpeed = 70f, maxTurnSpeed=500f;          
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


    [SerializeField]
    private float maxSpeedAnim, amplitudeAnim;

    private Rigidbody rb;
    private float currentSpeed;
    private Coroutine boostCoroutine;
    private float steeringLockTimer;

    /// <summary>
    /// Blocks the player's steering for <paramref name="duration"/> seconds
    /// (used e.g. when a rat knocks the car so the player can't instantly correct the turn).
    /// </summary>
    public void LockSteering(float duration)
    {
        steeringLockTimer = Mathf.Max(steeringLockTimer, duration);
    }

    public bool IsSteeringLocked => steeringLockTimer > 0f;

    // Read only, for it to convert to units we can see and use in the future
    public float getCurrentSpeed()
    {
        return currentSpeed;
    }
    public float SpeedNormalized => Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);

    //public float TurnSpeedNormalized => Mathf.Clamp((turnSpeed) / maxTurnSpeed, -1,1);



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
        anim.set_amplitude(amplitudeAnim);
       //already done in inspector rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Ensures the car is not tipping over
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        if (steeringLockTimer > 0f)
        {
            steeringLockTimer -= dt;
        }

        currentSpeed = Vector3.Dot(rb.linearVelocity, rb.rotation * Vector3.forward);

        anim.set_frequency(maxSpeedAnim * currentSpeed / maxSpeed);
        UpdateSpeed(dt);
        Quaternion newRotation = UpdateSteering(dt);

        Vector3 newVelocity = newRotation * Vector3.forward * currentSpeed;


        newVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity =  newVelocity;
    }

    /// <summary>
    /// Boosts the car: multiplies its current forward speed and its max speed
    /// by <paramref name="multiplier"/> (1.4 = +40%) for <paramref name="duration"/> seconds.
    /// Picking up a boost while already boosted only restarts the timer: the boost never stacks.
    /// </summary>
    public void ApplyBoost(float multiplier, float duration)
    {
        if (boostCoroutine != null)
        {
            // Already boosted: only restart the timer. The speed and max speed already have
            // their +40%, so applying it again would stack (1.4 x 1.4 = +96%).
            StopCoroutine(boostCoroutine);
            boostCoroutine = StartCoroutine(BoostCoroutine(duration));
            return;
        }

        OldMaxSpeed = maxSpeed; // Store the original maxSpeed
        maxSpeed = OldMaxSpeed * multiplier;

        // Instant kick: scale the current horizontal velocity (currentSpeed is re-read from
        // the rigidbody every FixedUpdate, so the velocity itself has to change).
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, rb.rotation * Vector3.forward);
        if (forwardSpeed > 0f)
        {
            Vector3 v = rb.linearVelocity;
            float boosted = Mathf.Min(forwardSpeed * multiplier, maxSpeed);
            float scale = boosted / forwardSpeed;
            rb.linearVelocity = new Vector3(v.x * scale, v.y, v.z * scale);
            currentSpeed = boosted;
        }

        boostCoroutine = StartCoroutine(BoostCoroutine(duration));
    }

    IEnumerator BoostCoroutine(float boostDuration)
    {
        yield return new WaitForSeconds(boostDuration);
        maxSpeed = OldMaxSpeed; // Reset maxSpeed; the car then coasts back down to normal speed
        boostCoroutine = null;
    }

    public bool IsBoosting => boostCoroutine != null;

    void OnDisable()
    {
        // Coroutines stop when the component is disabled, so undo the boost here
        // or maxSpeed would stay boosted forever.
        if (boostCoroutine != null)
        {
            maxSpeed = OldMaxSpeed;
            boostCoroutine = null;
        }
    }

    private void UpdateSpeed(float dt)
    {
        //Si freine ralentit de brakeDeceleration
        if (brakeInput)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeDeceleration * dt);
            return;
        }


        //vitesse avec boost
        float BoostedSpeed = currentSpeed + BoostAmount;

        //BoostedSpeed = Mathf.Round(BoostedSpeed * 1f) / 1f; // Round to 2 decimal places
        //Debug.Log("Boosted Speed: " + BoostedSpeed);

        

        bool goingForward = currentSpeed > 0.1f * BoostedSpeed;
        bool goingBackward = currentSpeed < -0.1f;

        //Si accelere dans le mauvais sens, ralentit de brakeDeceleration
        if (throttleInput > 0f && goingBackward || throttleInput < 0f && goingForward)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeDeceleration * dt);
        }
        //sinon on accelere dans le bon sens
        else if (throttleInput != 0f)
        {
            float targetSpeed = throttleInput > 0f ? throttleInput * maxSpeed: throttleInput * maxReverseSpeed;

            if (Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed))
            {
                float deltaAcceleration = maxSpeed / timeToMaxSpeed;
                float accel = deltaAcceleration * accelerationCurve.Evaluate(SpeedNormalized);
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
        if (steerInput == 0f || currentSpeed == 0f || IsSteeringLocked)
        {
           // rb.rotation = new Quaternion(rb.rotation.x, Mathf.MoveTowards(rb.rotation.y, 0f, coastDeceleration * dt), rb.rotation.z, rb.rotation.w);
            return rb.rotation;
            //return Quaternion.identity;
        }

        float steerFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / minSpeedForFullSteering);
        float direction = Mathf.Sign(currentSpeed);
        
        float speedNextRotation = turnSpeed * steerFactor * dt;


        float deltaTurnAcceleration = maxTurnSpeed / timeToTurnMaxSpeed;
      //  float TurnAccel = deltaTurnAcceleration * accelerationCurve.Evaluate(TurnSpeedNormalized);
        speedNextRotation = Mathf.MoveTowards(Mathf.Abs(speedNextRotation), maxTurnSpeed, deltaTurnAcceleration * dt);

        float orientedSpeed = speedNextRotation * direction * steerInput;

        Quaternion newRotation = rb.rotation * Quaternion.Euler(transform.rotation.eulerAngles.x, orientedSpeed, transform.rotation.eulerAngles.z);
        rb.MoveRotation(newRotation);
        return newRotation;
    }
}
