using UnityEngine;
using UnityEngine.Rendering;

public class horseAnim : MonoBehaviour
{

    [SerializeField]
    private float frequency, amplitude;

    float time = 0;

    public void set_frequency(float f)
    {
        return;
        //reset timer with new frequency to prevent offset of rotation
        frequency = f;
        //time = 0;
        //transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void set_amplitude(float a)
    {
        return;
        //reset timer with new frequency to prevent offset of rotation
        amplitude = a;
        //time = 0;
        //transform.rotation = new Quaternion(0, 0, 0, 0);
    }


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        //transform.rotation = Quaternion.Euler(  Mathf.Sin(2*Mathf.PI*frequency * Time.deltaTime) * amplitude,0,0);
        transform.Rotate(Mathf.Sin( frequency * time/ (2*Mathf.PI)) * amplitude, 0, 0);

        //transform.rotation = new Quaternion(0,0,Mathf.Sin(frequency * time / (2 * Mathf.PI)) * amplitude, 0);


       // transform.rotation = new Quaternion(Mathf.Clamp(transform.rotation.x, -amplitude, amplitude), transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }
}
