using UnityEngine;

public class bumpbehaviour : MonoBehaviour
{

    

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("inverse !!");

       Vector3 force =  collision.gameObject.GetComponent<Rigidbody>().GetAccumulatedForce();
        //collision.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.back * 10000, collision.transform.position, ForceMode.Acceleration);
      // collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(1000, collision.transform.position - force, 50);
        collision.gameObject.GetComponent<Rigidbody>().linearVelocity = -10*collision.gameObject.GetComponent<Rigidbody>().linearVelocity;
    }
}
