using UnityEngine;

public class finishLine : MonoBehaviour
{

    public SceneManager instance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            instance.endGame();
        }
    }


}
