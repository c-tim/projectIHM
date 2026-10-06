using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class inputs : MonoBehaviour
{

    [SerializeField]
    InputActionAsset map;

    [SerializeField]
    Boolean debugInput;

    float countStartPress = 0;

    public delegate void onButtonPressed();

    public delegate void onJoystickMove(Vector2 value, Boolean isKeyBoard);

    public onJoystickMove joystickLeftMoved,joystickRightMoved;
    public onButtonPressed northPressed, southPressed, westPressed, eastPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static inputs instance;



    public static inputs getInputs()
    {
        return instance;
    }
    
    void Start()
    {
        instance = this;
        map.Enable();

    }

    void callEvent(onButtonPressed eventPressed)
    {
        if (eventPressed == null)
        {
            Debug.LogWarning("No iinput attached for " + eventPressed.ToString());
            return;
        }
        eventPressed.Invoke();
    }

   /* void callEvent(onJoystickMove eventPressed, Vector2 args)
    {
        if (eventPressed == null)
        {
            Debug.LogWarning("No iinput attached for " + eventPressed.ToString());
            return;
        }
        eventPressed.Invoke(args);
    }*/


    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        Vector2 pos = map.FindAction("moveRightJoystick").ReadValue<Vector2>();
        if (joystickRightMoved != null)
=======



        Vector2 posKeyBoard = map.FindAction("moveRightJoystickKeyboard").ReadValue<Vector2>();

        Vector2 posGamepad = map.FindAction("moveRightJoystick").ReadValue<Vector2>();
        if (activateVibration)
>>>>>>> Stashed changes
        {
            vibrateIfEnabled(activateVibration, 0f);

        }


        if(posKeyBoard!= Vector2.zero)
        {
            if (joystickRightMoved != null)
            {
                joystickRightMoved(posKeyBoard, true);
            }
        }
        else
        {

            if (joystickRightMoved != null)
            {
                joystickRightMoved(posGamepad, false);
            }
        }


        if (posGamepad != Vector2.zero)
        {
            printIfEnabled("pos gamepad updated " + posGamepad);
        }

        if (map.FindAction("north").IsPressed())
        {
            printIfEnabled("north pressed");
            if (northPressed != null)
            {
                northPressed();
            }

        }
        if (map.FindAction("south").IsPressed())
        {
            printIfEnabled("south pressed");
            if (southPressed != null)
            {
                southPressed();
            }

        }
        if (map.FindAction("west").IsPressed())
        {
            printIfEnabled("west pressed");
            if (westPressed != null)
            {
                westPressed();
            }

        }
        if (map.FindAction("east").IsPressed())
        {
            printIfEnabled("east pressed");
            if (eastPressed != null)
            {
                eastPressed();
            }

        }
       
      

    }

    void printIfEnabled(String s)
    {
        if (!debugInput)
        {
            return;
        }
        Debug.Log(s);
    }


    
}
