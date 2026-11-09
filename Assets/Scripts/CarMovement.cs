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

        if (!SceneManager.gameRunning)
        {
            return;
        }
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

    // Multiplier of the speed effect currently active: 1 = none, > 1 = boost, < 1 = debuff
    private float activeMultiplier = 1f;

    /// <summary>
    /// Applies a speed effect: multiplies the current forward speed and the max speed
    /// by <paramref name="multiplier"/> for <paramref name="duration"/> seconds.
    /// Above 1 is a boost (1.4 = +40%), below 1 is a debuff (0.6 = -40%).
    /// - Same kind while one is active (boost + boost, debuff + debuff): only restarts the timer, never stacks.
    /// - Opposite kind (boost + debuff, debuff + boost): they cancel out and the car goes back to normal.
    /// </summary>
    public void ApplyBoost(float multiplier, float duration)
    {
        if (Mathf.Approximately(multiplier, 1f)) return;

        if (boostCoroutine != null)
        {
            bool sameKind = (multiplier > 1f) == (activeMultiplier > 1f);
            if (sameKind)
            {
                // Already have this effect: restart the timer so it doesn't stack (1.4 x 1.4 = +96%)
                StopCoroutine(boostCoroutine);
                boostCoroutine = StartCoroutine(BoostCoroutine(duration));
            }
            else
            {
                // Boost + debuff (or the other way round): they cancel each other out
                CancelSpeedEffect();
            }
            return;
        }

        OldMaxSpeed = maxSpeed; // Store the original maxSpeed
        maxSpeed = OldMaxSpeed * multiplier;
        activeMultiplier = multiplier;

        // Instant kick (or slow down) on the current speed
        ScaleForwardSpeed(multiplier);

        boostCoroutine = StartCoroutine(BoostCoroutine(duration));
    }

    /// <summary>
    /// Removes the active boost/debuff right away: max speed goes back to normal and the
    /// instant speed change of the effect is undone.
    /// </summary>
    private void CancelSpeedEffect()
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }

        maxSpeed = OldMaxSpeed;
        ScaleForwardSpeed(1f / activeMultiplier); // e.g. 56 / 1.4 = 40, or 24 / 0.6 = 40
        activeMultiplier = 1f;
    }

    // Scales the forward speed by factor, capped at maxSpeed. currentSpeed is re-read from the
    // rigidbody every FixedUpdate, so the velocity itself has to change.
    private void ScaleForwardSpeed(float factor)
    {
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, rb.rotation * Vector3.forward);
        if (forwardSpeed <= 0f) return;

        Vector3 v = rb.linearVelocity;
        float newSpeed = Mathf.Min(forwardSpeed * factor, maxSpeed);
        float scale = newSpeed / forwardSpeed;
        rb.linearVelocity = new Vector3(v.x * scale, v.y, v.z * scale);
        currentSpeed = newSpeed;
    }

    IEnumerator BoostCoroutine(float boostDuration)
    {
        yield return new WaitForSeconds(boostDuration);
        maxSpeed = OldMaxSpeed; // Reset maxSpeed; the car then goes back to normal speed on its own
        activeMultiplier = 1f;
        boostCoroutine = null;
    }

    public bool IsBoosting => boostCoroutine != null && activeMultiplier > 1f;
    public bool IsSlowed => boostCoroutine != null && activeMultiplier < 1f;

    void OnDisable()
    {
        // Coroutines stop when the component is disabled, so undo the effect here
        // or maxSpeed would stay changed forever.
        if (boostCoroutine != null)
        {
            maxSpeed = OldMaxSpeed;
            activeMultiplier = 1f;
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
