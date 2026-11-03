using UnityEngine;

public class horseAnim : MonoBehaviour
{

    [SerializeField]
    private float frequency, amplitude;

    

    // Update is called once per frame
    void Update()
    {
        transform.Rotate( Mathf.Sin(frequency * Time.deltaTime) * amplitude,0,0);
    }
}
