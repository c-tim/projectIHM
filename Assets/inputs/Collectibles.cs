using Unity.VisualScripting;
using UnityEngine;

public class Collectible:MonoBehaviour
{
    public delegate void EventWithPlayer(GameObject player);
    public event EventWithPlayer OnTriggered;


    private void OnTriggerEnter(Collider other)
    {
        //if(other.tag != "player")
        //{
        //    return;
        //}
        if (other.gameObject.CompareTag("Player"))
        {
            print("Collectible collected!");
            Destroy(this.gameObject);
        }

        //TODO incomment OnTriggered(other.GetComponent<Car>());

    }

}
