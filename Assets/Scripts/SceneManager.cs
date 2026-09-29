using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Vector3 playerSpawnPosition = new Vector3(-7f, 0f, 0f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
