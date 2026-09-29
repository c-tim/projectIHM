using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform[] target;
    public float speed;

    private int current;
    private float currentSpeed = 0;    // 0-1 from no speed to full speed
    private float accel = 0.5f;        // accelerate to full speed in 2 seconds (1 / accel)


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    void Update()
    {
        if (transform.position != target[current].position)
        {
            currentSpeed += Mathf.Min(accel * Time.deltaTime, 1);    // limit to 1 for "full speed"
            Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * currentSpeed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos);
        }
    }


}
