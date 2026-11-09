using System;
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

    [SerializeField]
    [Tooltip("Rotation (Euler angles) the player spawns with. Y = 180 makes the player face down the screen.")]
    private Vector3 playerSpawnRotation = new Vector3(0f, 180f, 0f);

    GameObject player;

    [SerializeField]
    public Timer timer;

    [SerializeField]
    GUIManager gui;


    void Start()
    {
        player = Instantiate(playerPrefab, GetSpawnPosition(), GetSpawnRotation());
        cam.target = player.transform;

        restart();
    }

    public void restart()
    {
        Vector3 pos = GetSpawnPosition();
        Quaternion rot = GetSpawnRotation();

        // The player has a Rigidbody, so move the body itself; otherwise physics/interpolation
        // can overwrite a plain transform change and the rotation gets lost.
        CarMovement car = player.GetComponent<CarMovement>();
        if (car != null)
        {
            car.ResetMovement();
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.position = pos;
            rb.rotation = rot;
        }
        player.transform.SetPositionAndRotation(pos, rot);
        timer.startTimer();
    }

    private Vector3 GetSpawnPosition()
    {
        return playerSpawnPosition;
    }

    private Quaternion GetSpawnRotation()
    {
        return Quaternion.Euler(playerSpawnRotation);
    }

    private void Update()
    {
        gui.updateTextTimer(timer.toString());
    }

    public void endGame()
    {
        timer.stop();
    }


}
