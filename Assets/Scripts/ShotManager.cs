using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ShotManager : BlockManager {
    private Transform _transform;
    public float shotForwardVelocity;
    public Transform shotRadius;
    private Plane clickPlane;

    protected override void Awake()
    {
        base.Awake();
        _transform = transform;
    }

    protected override void Start()
    {
        clickPlane = new Plane(-Vector3.forward, Vector3.zero);
        base.Start();
    }

    void Update() {
	    if (Input.GetButtonDown("Fire1"))
	    {
	        if (objectQueue.Peek() != null)
	        {
	            
	            var clickLocation = Input.mousePosition;
                Debug.Log("Mouse location " + clickLocation);
                Ray ray = Camera.main.ScreenPointToRay(clickLocation);
	            float hitDistance;
	            if (clickPlane.Raycast(ray, out hitDistance))
	            {
                    var shot = objectQueue.Dequeue();
                    objectQueue.Enqueue(shot);
	                var direction = (ray.GetPoint(hitDistance) - _transform.position).normalized;
                    RaycastHit sphereHit;
	                Physics.Raycast(new Vector3(_transform.position.x, _transform.position.y), Vector3.up, out sphereHit, .7f);
	                Ray sphereRay = Camera.main.ScreenPointToRay(clickLocation);
                    Physics.Raycast(sphereRay, out sphereHit);
                    Debug.Log("Sphere was hit " + sphereHit.point);
	            
                    shot.position = direction;
                    Debug.Log(direction);
	                shot.GetComponent<Shot>().Shoot(direction);
	            }
	        }
	    }
	}
}
