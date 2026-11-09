using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class inputs : MonoBehaviour
{

    [SerializeField]
    InputActionAsset map;

    [SerializeField]
    Boolean debugInput, activateVibration;

    [SerializeField]

    float powerVibration;

    AnimationCurve curveVibrate;

    float countStartPress = 0;

    public delegate void onButtonPressed();

    public delegate void onJoystickMove(Vector2 value, bool isKeyboard);

    public onJoystickMove joystickLeftMoved,joystickRightMoved;
    public onButtonPressed northPressed, southPressed, westPressed, eastPressed;
    public onButtonPressed triggerLeftPressed, triggerRightPressed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static inputs instance;



    public static inputs getInputs()
    {
        return instance;
    }
    
    // Awake (not Start) so the instance exists before any car's Start asks for it
    void Awake()
    {
        instance = this;
        map.Enable();

    }

    void updatePosInputs()
    {
        Vector2 posKey = map.FindAction("moveRightKeyboard").ReadValue<Vector2>();
        if (posKey != Vector2.zero)
        {
            printIfEnabled("pos updated keyboard" + posKey);

            if (joystickRightMoved != null)
            {
                joystickRightMoved(posKey, true);
            }
             return;
        }

            
        //If no Inputs on keyboard check on joystick
        Vector2 posJoy = map.FindAction("moveRightJoystick").ReadValue<Vector2>();
        if (posJoy != Vector2.zero)
        {
            printIfEnabled("pos updated joystick" + posJoy);
            if (joystickRightMoved != null)
            {
                joystickRightMoved(posJoy, false);
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        updatePosInputs();
        //Gamepad.current.SetMotorSpeeds(0f, 0f);
        vibrateIfEnabled(activateVibration, 0f);
    

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

        if (map.FindAction("triggerLeft").IsPressed())
        {
            printIfEnabled("triggerLeft pressed");
            if (triggerLeftPressed != null)
            {
                triggerLeftPressed();
            }

        }
        if (map.FindAction("triggerRight").IsPressed())
        {
            printIfEnabled("triggerRight pressed");
            vibrateIfEnabled(activateVibration, powerVibration);

            if (triggerRightPressed != null)
            {
                triggerRightPressed();
            }
        }
    }

    void printIfEnabled(String s)
    {
        if (!debugInput)
        {
            return;
        }
        //Debug.Log(s);
    }

    void vibrateIfEnabled(Boolean activate, float value)
    {
        if (!activate)
        {
            return;
        }
        Gamepad.current.SetMotorSpeeds(value, value);
    }


    
}
