using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class CarController : MonoBehaviour
{
    [Range(0f, 0.5f)] public float steerDeadZone = 0.15f;

    [SerializeField]
    private bool UseGlobalInputs;
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

    private void OnJoystick(Vector2 value, bool isKeyboard)
    {
        if (GuiParameters.useGlobalPOVInputs )
        {
            value = MapInputWithCameraOrientation(value);
        }


        steer = Mathf.Abs(value.x) < steerDeadZone ? 0f : value.x;
        if (isKeyboard)
        {
            if (value.y > 0)
            {
                OnAccelerate();
            }
            else if(value.y <0)
            {
                OnReverse();
            }
        }
    }


    /** Get the direction of the input, from the POV of the car. Ex : car going down + joystick towards left => car should move to its right **/
    private Vector2 MapInputWithCameraOrientation(Vector2 value)
    {

       
        float alpha = (transform.rotation.y);
        Debug.Log("rotation :" + alpha);
        //Quaternion rotation =   Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector2.up);

        //   transform.eulerAngles = ;
        //  return rotation * value;
        // return new Vector2(transform.eulerAngles.x, Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
        //return new Vector2(Mathf.Cos(alpha) * value.x - Mathf.Sin(alpha) * value.y, Mathf.Cos(alpha) * value.y + Mathf.Sin(alpha) * value.x);

        Vector3 orientedVec3= (Quaternion.Inverse(Quaternion.Euler(0, transform.localEulerAngles.y, 0)) * new Vector3(value.x, 0, value.y) ) ;
        return new Vector2(orientedVec3.x, orientedVec3.z);
    }
    private void OnAccelerate() { accelPressed = true; }
    private void OnReverse() { reversePressed = true; }
    private void OnBrake() { brakePressed = true; }
}
