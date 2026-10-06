using Unity.VisualScripting;
using UnityEngine;

public class Collectible:MonoBehaviour
{
    public delegate void EventWithPlayer(GameObject player);
    public event EventWithPlayer OnTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //print("Collectible collected!");
            if (this.gameObject.CompareTag("PowerUp/Turbo"))
            {
                //print("Turbo collected!");
                //other.GetComponent<CarController>().BoostAmount += 10f;
                other.GetComponent<CarMovement>().ApplyBoost(10f, 5f);
                //StartCoroutine(other.GetComponent<CarController>().BoostCoroutine(5f));
            }
            Destroy(this.gameObject);
        }



        //TODO incomment OnTriggered(other.GetComponent<Car>());

    }

}
