using UnityEngine;

public class finishLine : MonoBehaviour
{

    public gameManager instance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            instance.endGame();
        }
    }


}
