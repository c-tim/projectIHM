using UnityEngine;

public class horseAnim : MonoBehaviour
{

    [SerializeField]
    private float frequency, amplitude;

    float time = 0;

    

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        //transform.rotation = Quaternion.Euler(  Mathf.Sin(2*Mathf.PI*frequency * Time.deltaTime) * amplitude,0,0);
        transform.Rotate(Mathf.Sin( frequency * time/ (2*Mathf.PI)) * amplitude, 0, 0);
    }
}
