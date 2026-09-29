using UnityEngine;

// Arcade car movement. This script does NOT read any input by itself:
// the controller code calls SetInput() / SetBrake() every frame and this
// script turns those values into movement (acceleration, top speed, braking,
// coasting, reverse and steering).
[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    [Header("Speed")]
    public float maxSpeed = 20f;            // top speed going forward (units/s)
    public float maxReverseSpeed = 6f;      // top speed going backwards (units/s)
    public float timeToMaxSpeed = 3f;       // seconds from 0 to maxSpeed at full throttle

    // How strong the acceleration is depending on the current speed (0 = stopped, 1 = max speed).
    // Default: strong at the start, weaker near top speed, like a real car.
    public AnimationCurve accelerationCurve = new AnimationCurve(
        new Keyframe(0f, 1.5f),
        new Keyframe(1f, 0.5f));

    [Header("Deceleration")]
    public float brakeDeceleration = 25f;   // units/s² when braking (or pressing the opposite direction)
    public float coastDeceleration = 5f;    // units/s² when no throttle (engine brake / friction)

    [Header("Steering")]
    public float turnSpeed = 120f;          // degrees/s at full steer
    public float speedForFullSteering = 5f; // below this speed the car turns less (can't turn in place)

    [Header("Input (set by the controller, visible here for testing)")]
    [Range(-1f, 1f)] [SerializeField] private float throttleInput;  // 1 = forward, -1 = reverse
    [Range(-1f, 1f)] [SerializeField] private float steerInput;     // -1 = left, 1 = right
    [SerializeField] private bool brakeInput;

    private Rigidbody rb;
    private float currentSpeed;             // signed speed along the car's forward axis

    // Read-only info, useful for UI (speedometer) or sounds
    public float CurrentSpeed => currentSpeed;
    public float SpeedNormalized => Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);

    // ---- API for the controller ----

    // throttle: -1..1 (negative = reverse), steer: -1..1 (negative = left)
    public void SetInput(float throttle, float steer)
    {
        throttleInput = Mathf.Clamp(throttle, -1f, 1f);
        steerInput = Mathf.Clamp(steer, -1f, 1f);
    }

    public void SetBrake(bool braking)
    {
        brakeInput = braking;
    }

    // Instantly stops the car (e.g. respawn, race start)
    public void ResetMovement()
    {
        currentSpeed = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // ---- Movement logic ----

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        // Keep the car upright, we only rotate around Y when steering
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // Start from the real physics speed, so hitting a wall actually slows the car down
        currentSpeed = Vector3.Dot(rb.linearVelocity, rb.rotation * Vector3.forward);

        UpdateSpeed(dt);
        Quaternion newRotation = UpdateSteering(dt);

        // Apply: move along the (new) forward direction, keep vertical velocity for gravity
        Vector3 velocity = newRotation * Vector3.forward * currentSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    private void UpdateSpeed(float dt)
    {
        if (brakeInput)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeDeceleration * dt);
            return;
        }

        bool goingForward = currentSpeed > 0.1f;
        bool goingBackward = currentSpeed < -0.1f;

        if (throttleInput > 0f && goingBackward || throttleInput < 0f && goingForward)
        {
            // Pressing the opposite direction: brake first before changing direction
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeDeceleration * dt);
        }
        else if (throttleInput != 0f)
        {
            float targetSpeed = throttleInput > 0f
                ? throttleInput * maxSpeed
                : throttleInput * maxReverseSpeed;

            if (Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed))
            {
                // Accelerate: base rate reaches maxSpeed in timeToMaxSpeed, shaped by the curve
                float baseAccel = maxSpeed / timeToMaxSpeed;
                float accel = baseAccel * accelerationCurve.Evaluate(SpeedNormalized);
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * dt);
            }
            else
            {
                // Faster than what the throttle asks for (released a bit): slow down gently
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, coastDeceleration * dt);
            }
        }
        else
        {
            // No throttle: the car slowly rolls to a stop
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, coastDeceleration * dt);
        }
    }

    // Returns the car's rotation after steering this step
    private Quaternion UpdateSteering(float dt)
    {
        if (steerInput == 0f || currentSpeed == 0f)
            return rb.rotation;

        // The car can only turn while moving, and turns less at very low speed
        float steerFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / speedForFullSteering);
        // When reversing, steering is inverted (like a real car)
        float direction = Mathf.Sign(currentSpeed);

        float yaw = steerInput * turnSpeed * steerFactor * direction * dt;
        Quaternion newRotation = rb.rotation * Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(newRotation);
        return newRotation;
    }
}
