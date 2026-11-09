using UnityEngine;

public class Collectible:MonoBehaviour
{
    public delegate void EventWithPlayer(GameObject player);
    public event EventWithPlayer OnTriggered;

    [Header("Turbo")]
    [Tooltip("1.4 = +40% to current speed and max speed")]
    [SerializeField] private float boostMultiplier = 1.4f;
    [SerializeField] private float boostDuration = 4f;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // The player has several colliders (body + child cubes), so look at the rigidbody's
        // object rather than the collider that happened to touch the pickup.
        GameObject player = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!player.CompareTag("Player")) return;

        if (gameObject.CompareTag("PowerUp/Turbo"))
        {
            CarMovement car = player.GetComponentInParent<CarMovement>();
            if (car != null)
            {
                car.ApplyBoost(boostMultiplier, boostDuration);
            }
            else
            {
                Debug.LogWarning("Turbo picked up but no CarMovement found on " + player.name);
            }
        }

        collected = true;
        OnTriggered?.Invoke(player);
        Destroy(gameObject);
    }
}
