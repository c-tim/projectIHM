using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    [SerializeField]
    private GameObject playerPrefab, startPos, endPos;

    [SerializeField]
    cameraBehaviour cam;

    [SerializeField]
    private Vector3 playerSpawnPosition = new Vector3(-7f, 0f, 0f);

    GameObject player;

    void Start()
    {
       player =  Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        cam.target = player.transform;
        restart();
    }

    public void restart()
    {
        player.transform.position = startPos.transform.position;
        player.transform.rotation = startPos.transform.rotation;

    }

  
}
