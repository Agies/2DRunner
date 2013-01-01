using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ShotManager : BlockManager {
    private Transform _transform;
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
        var mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        float hitDistance;
        clickPlane.Raycast(ray, out hitDistance);
        var direction = (ray.GetPoint(hitDistance) - _transform.position).normalized;
        _transform.localPosition = (_transform.localPosition + direction).normalized;
        Debug.Log(direction);
        if (Input.GetButtonDown("Fire1"))
	    {
	        if (objectQueue.Peek() != null)
	        {
	            var shot = objectQueue.Dequeue();
	            objectQueue.Enqueue(shot);
	            shot.position = _transform.position;
	            Debug.Log("Shot position is " + shot.position);
	            shot.GetComponent<Shot>().Shoot(direction);
	        }
	    }
	}
}
