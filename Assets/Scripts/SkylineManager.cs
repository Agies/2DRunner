using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class RecyclingBlockManager : MonoBehaviour
{
    public Transform prefab;
    public int numberOfObjects;
    public float recycleOffset;
    public Vector3 minsize;
    public Vector3 maxsize;
    protected Vector3 nextPosition;
    private Queue<Transform> objectQueue;

    protected virtual void Start ()
    {
        objectQueue = new Queue<Transform>(numberOfObjects);
        for (int i = 0; i < numberOfObjects; i++)
        {
            objectQueue.Enqueue((Transform)Instantiate(prefab));
        }
        nextPosition = transform.localPosition;
        for (int i = 0; i < numberOfObjects; i++)
        {
            Recycle();
        }
    }

    protected virtual void Update ()
    {
        var offset = objectQueue.Peek().localPosition.x + recycleOffset;
        Debug.Log("Offset is " + offset);
        if (offset < Runner.distanceTraveled)
        {
            Recycle();
        }
    }

    private void Recycle()
    {
        var scale = new Vector3(
            Random.Range(minsize.x, maxsize.x),
            Random.Range(minsize.y, maxsize.y),
            Random.Range(minsize.z, maxsize.z)
            );

        var position = nextPosition;
        position.x += scale.x*0.5f;
        position.y += scale.y*0.5f;

        var o = objectQueue.Dequeue();
        objectQueue.Enqueue(o);

        OnRecycle(o, scale, position);
    }

    protected virtual void OnRecycle(Transform block, Vector3 scale, Vector3 position)
    {
        block.localScale = scale;
        block.localPosition = position;
    }
}

public class SkylineManager : RecyclingBlockManager
{
    protected override void OnRecycle(Transform block, Vector3 scale, Vector3 position)
    {
        base.OnRecycle(block, scale, position);

        nextPosition.x += scale.x;
    }
}
