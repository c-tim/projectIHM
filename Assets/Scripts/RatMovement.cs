using UnityEngine;

public class RatMovement : MonoBehaviour
{
    public Vector3 start;
    public Vector3 finish;
    public float speed = 5f;

    private Vector3 target;

    void Start()
    {
        transform.position = start;
        target = finish;
        transform.LookAt(target);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position == target)
        {
            target = target == finish ? start : finish;
            transform.LookAt(target);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TurnCar(other.attachedRigidbody);
            Destroy(gameObject);
        }
    }
    private void TurnCar(Rigidbody car)
    {
        float angle = Random.Range(20f, 40f);
        if (Random.value < 0.5f)
            angle = -angle;

        Quaternion turn = Quaternion.Euler(0f, angle, 0f);
        car.rotation = turn * car.rotation;
        car.linearVelocity = turn * car.linearVelocity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, finish);
        Gizmos.DrawWireSphere(start, 0.3f);
        Gizmos.DrawWireSphere(finish, 0.3f);
    }
}
