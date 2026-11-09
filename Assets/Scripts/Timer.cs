using System.Threading;
using UnityEngine;

public class Timer : MonoBehaviour
{

     

    double hour, minutes, seconds;
    float floatTimer;

    bool isOn = false;


    public void resetTimer()
    {
        hour = 0;
        minutes = 0;
        seconds = 0;
    }
    public void startTimer()
    {
        resetTimer();
        floatTimer = 0;
        isOn = true;
    }


    public void stop()
    {
        isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOn)
        {
            return;
        }


        floatTimer += Time.deltaTime;
        float intTimer = Mathf.FloorToInt(floatTimer);
        floatTimer -= intTimer;

        seconds += intTimer;
        if (seconds >= 60) { 
        
            seconds -= 60;
            minutes++;
        }

        if (minutes >= 60) { 
        
            minutes -= 60;
            hour++;
        }

    }

    public string extraDigit(double value)
    {
        if (value >= 10)
        {
            return  value.ToString();
        }

        return "0" + value.ToString();
    }

    public string toString()
    {
        return extraDigit(hour) + ":" + extraDigit(minutes) + ":" + extraDigit(seconds);
    }

    public double getTotal()
    {
        return hour * 60 * 60 + minutes * 60 + seconds;
    }

  
}
