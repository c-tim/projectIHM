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
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        GameObject g =  Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        g.transform.position = startPos.transform.position;
        cam.target = g.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
