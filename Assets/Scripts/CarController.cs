using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class CarController : MonoBehaviour
{
    [Range(0f, 0.5f)] public float steerDeadZone = 0.15f;

    private CarMovement car;
    private inputs input;

    private float steer;
    private bool accelPressed;
    private bool reversePressed;
    private bool brakePressed;

    void Awake()
    {
        car = GetComponent<CarMovement>();

    }

    void Start()
    {
        input = inputs.getInputs();
        if (input == null)
        {
            Debug.LogError("CarController: no 'inputs' object in the scene, the car can't be controlled.");
            enabled = false;
            return;
        }

        input.joystickRightMoved += OnJoystick;
        input.southPressed += OnAccelerate;
        input.westPressed += OnReverse;
        input.eastPressed += OnBrake;
    }

    void OnDestroy()
    {
        if (input != null)
        {
            input.joystickRightMoved -= OnJoystick;
            input.southPressed -= OnAccelerate;
            input.westPressed -= OnReverse;
            input.eastPressed -= OnBrake;
        }
    }

    void Update()
    {
        float throttle = 0f;
        
        if (accelPressed) throttle += 1;
        if (reversePressed) throttle -= 1f;

        car.SetInput(throttle, steer);
        car.SetBrake(brakePressed);

        accelPressed = false;
        reversePressed = false;
        brakePressed = false;
    }

    private void OnJoystick(Vector2 value)
    {
        steer = Mathf.Abs(value.x) < steerDeadZone ? 0f : value.x;
    }

    private void OnAccelerate() { accelPressed = true; }
    private void OnReverse() { reversePressed = true; }
    private void OnBrake() { brakePressed = true; }
}
