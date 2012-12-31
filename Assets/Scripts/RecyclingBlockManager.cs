using System.Collections.Generic;
using UnityEngine;

public class RecyclingBlockManager : BlockManager
{
    public float recycleOffset;
    public Vector3 minsize;
    public Vector3 maxsize;
    protected Vector3 nextPosition;
    
    protected override void OnGameStart(GameStartMessage obj)
    {
        nextPosition = transform.localPosition;
        for (int i = 0; i < numberOfObjects; i++)
        {
            Recycle();
        }

        base.OnGameStart(obj);
    }

    protected virtual void Update ()
    {
        var offset = objectQueue.Peek().localPosition.x + recycleOffset;
        if (offset < Runner.DistanceTraveled)
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