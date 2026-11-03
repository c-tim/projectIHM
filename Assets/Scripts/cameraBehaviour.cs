using Unity.VisualScripting;
using UnityEngine;

public class cameraBehaviour: MonoBehaviour
{

    public Transform target;

    [SerializeField]
    private float minDistToTeleport;

    [SerializeField]
    private float height;

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
            transform.position = new Vector3(target.position.x,height, target.position.z);

        }
        else
        {
            Vector2 translation = (getVec2(transform.position) - getVec2(target.transform.position));
            translation.x /= Mathf.Abs(translation.x);
            translation.y /= Mathf.Abs(translation.y);

            Debug.Log("trasnlate is " + translation + " height :" + height);
            transform.Translate(translation.x, height,translation.y);

         
        }
    }
}
