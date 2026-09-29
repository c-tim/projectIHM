using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;

public class inputs : MonoBehaviour
{

    [SerializeField]
    InputActionAsset map;

    [SerializeField]
    Boolean debugInput;

    float countStartPress = 0;

    public delegate void onButtonPressed();

    public delegate void onJoystickMove(Vector2 value);

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
            Debug.LogWarning("No input attached for " + eventPressed.ToString());
            return;
        }
        eventPressed.Invoke();
    }

    void callEvent(onJoystickMove eventPressed, Vector2 args)
    {
        if (eventPressed == null)
        {
            Debug.LogWarning("No input attached for " + eventPressed.ToString());
            return;
        }
        eventPressed.Invoke(args);
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 pos = map.FindAction("moveRightJoystick").ReadValue<Vector2>();
        if (joystickRightMoved != null)
        {
            joystickRightMoved(pos);
        }
        if (pos != Vector2.zero)
        {
            printIfEnabled("pos updated " + pos);
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
