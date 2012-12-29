using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class PlatformManager : RecyclingBlockManager
{
    public Vector3 minGap;
    public Vector3 maxGap;
    public float minY;
    public float maxY;

    public PlatformMod[] platforms;
    private List<PlatformMod> _platforms;

    void Awake()
    {
        
    }

    protected override void Start()
    {
        _platforms = new List<PlatformMod>(platforms.Sum(p => p.factor));
        foreach (var mod in platforms)
        {
            for (int i = 0; i < mod.factor; i++)
            {
                _platforms.Add(mod);
            }
        }

        base.Start();
    }

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

        var index = Random.Range(0, _platforms.Count);
        var mod = _platforms[index];
        Debug.Log("Choose platform " + mod.name + " at position " + index);
        block.renderer.material = mod.material;
        block.collider.material = mod.physicMaterial;
    }
}

[Serializable]
public class PlatformMod
{
    public Material material;
    public PhysicMaterial physicMaterial;
    public string name;
    public int factor = 1;
}
