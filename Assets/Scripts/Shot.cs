using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {
    
    public Vector3 speed;

    // Use this for initialization
	void Start ()
	{
	    enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!enabled) return; 
        rigidbody.AddForce(speed * 10, ForceMode.Impulse);
	}

    public void Shoot(Vector3 shotSpeed)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.inertiaTensorRotation = Quaternion.identity;
        rigidbody.inertiaTensor = Vector3.zero;
        rigidbody.isKinematic = true;
        rigidbody.isKinematic = false;
        speed = shotSpeed;
        enabled = true;
    }

    void FixedUpdate()
    {
        if (!enabled) return;
        //rigidbody.MovePosition(rigidbody.position + speed * 10 * Time.deltaTime);
    }
}
