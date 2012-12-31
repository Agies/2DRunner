using UnityEngine;
using System.Collections;

public class SkylineManager : RecyclingBlockManager
{
    protected override void OnRecycle(Transform block, Vector3 scale, Vector3 position)
    {
        base.OnRecycle(block, scale, position);

        nextPosition.x += scale.x;
    }
}
