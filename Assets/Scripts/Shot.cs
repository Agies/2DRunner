using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {
    
    public Vector3 direction;
    public float speed = 10;

    void Start ()
	{
	    enabled = false;
	}
	
	void Update () {
        if (!enabled) return; 
        rigidbody.AddForce(direction * speed, ForceMode.Impulse);
	}

    public void Shoot(Vector3 direction)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.inertiaTensorRotation = Quaternion.identity;
        rigidbody.inertiaTensor = Vector3.zero;
        rigidbody.isKinematic = true;
        rigidbody.isKinematic = false;
        this.direction = direction;
        enabled = true;
    }

    void FixedUpdate()
    {
        //if (!enabled) return;
        //rigidbody.MovePosition(rigidbody.position + direction * 10 * Time.deltaTime);
    }
}
