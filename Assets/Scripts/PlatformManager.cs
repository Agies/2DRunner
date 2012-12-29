using UnityEngine;
using System.Collections;

public class PlatformManager : RecyclingBlockManager
{
    public Vector3 minGap;
    public Vector3 maxGap;
    public float minY;
    public float maxY;

    protected override void OnRecycle(Transform block, Vector3 scale, Vector3 position)
    {
        base.OnRecycle(block, scale, position);

        nextPosition += new Vector3(
                Random.Range(minGap.x, maxGap.x) + scale.x,
                Random.Range(minGap.y, maxGap.y),
                Random.Range(minGap.z, maxGap.z));
        if (nextPosition.y < minY)
        {
            nextPosition.y = minY + maxGap.y;
        }
        else if (nextPosition.y > maxY)
        {
            nextPosition.y = maxY - maxGap.y;
        }
    }
}
