using UnityEngine;
using System.Collections;

public class Runner : MonoBehaviour {
    private Transform _transform;
    public static float distanceTraveled;
    public float acceleration;
    private bool touchingPlatform;
    private Rigidbody _rigidBody;

    void Awake()
    {
        _transform = transform;
        _rigidBody = rigidbody;
    }

	// Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
	void Update ()
	{
	    distanceTraveled = transform.localPosition.x;
        Debug.Log("Distance Traveled " + distanceTraveled);
	}

    void FixedUpdate()
    {
        if (touchingPlatform)
        {
            _rigidBody.AddForce(acceleration, 0f,0f, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter()
    {
        touchingPlatform = true;
    }

    void OnCollisionExit()
    {
        touchingPlatform = false;
    }
}
