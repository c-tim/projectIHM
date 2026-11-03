using UnityEngine;

public class cameraBehaviour: MonoBehaviour
{

    public Transform target;

    [SerializeField]
    private float minDistToTeleport;

    public float distToCamera(Vector3 pos)
    {
        return Vector2.Distance(getVec2(transform.position), getVec2(pos));
    }

    public Vector2 getVec2(Vector3 pos)
    {
        return new Vector2(pos.x, pos.z);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveToTarget();
    }

    void moveToTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("No target for camera");
            return;
        }

        if (distToCamera(target.position) < minDistToTeleport)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);

        }
        else
        {
            transform.Translate(minDistToTeleport * (getVec2(transform.position) - getVec2(target.transform.position)));
        }
    }
}
