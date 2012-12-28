using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour
{
    public static Scoring sc;
    public LayerMask GroundMask;
    public LayerMask LadderAndRopeMask;
    public LayerMask EnemyMask;

    public static float orthSize;
    public static float orthSizeX;
    public static float orthSizeY;
    public static float camRatio;

    public static LayerMask groundMask;
    public static LayerMask ladderAndRopeMask;
    public static LayerMask enemyMask;

    public void Awake()
    {
        groundMask = GroundMask;
        ladderAndRopeMask = LadderAndRopeMask;
        enemyMask = EnemyMask;
    }

    void Start ()
    {
        sc = (Scoring)gameObject.GetComponent("Scoring");

        camRatio = 1.333f;
        orthSize = Camera.mainCamera.camera.orthographicSize;
        orthSizeX = orthSize*camRatio;
    }

    private void Update()
    {
        
    }

    public static bool IsGroundBetween(Vector3 pos1, Vector3 pos2)
    {
        pos1.z = 0; pos2.z = 0;
        return Physics.Linecast(pos1, pos2, groundMask.value);
    }

    public void OnDrawGizmosSelected()
    {
        if (AstarPath.active && AstarPath.active.graphs != null && AstarPath.active.graphs.Length > 0)
        {
            MyGridGraph gg = (AstarPath.active.graphs[0] as MyGridGraph);

            if (gg.graphNodes.Length > 0)
            {
                for (int y = gg.depth - 1; y >= 0; y--)
                {
                    for (int x = 0; x < gg.width; x++)
                    {
                        MyGridNode node = gg.graphNodes[y * gg.width + x] as MyGridNode;

                        if (node.isTrueWalkable && node.isFallLane)
                        {
                            Gizmos.color = new Color(1, 1, 0, 0.5F);

                            Gizmos.DrawSphere((Vector3)node.position, 0.3f);
                        }

                        else if (node.isTrueWalkable)
                        {
                            Gizmos.color = new Color(0, 1, 0, 0.5F);

                            Gizmos.DrawSphere((Vector3)node.position, 0.3f);
                        }

                        else if (node.isFallLane)
                        {
                            Gizmos.color = new Color(0, 0, 1, 0.5F);

                            Gizmos.DrawSphere((Vector3)node.position, 0.3f);
                        }
                    }
                }
            }
        }
    }
}

public enum Facing
{
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4
}
