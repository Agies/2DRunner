using Pathfinding;
using Pathfinding.Nodes;
using UnityEngine;
using System.Collections;

public class MyGridGraph : GridGraph, ISerializableGraph
{
    public MyGridNode[] myGridNodes;
    public LayerMask levelLayerMask;

    public MyGridNode[] CreateMyNodes(int number)
    {
        var tmp = new MyGridNode[number];
        for (int i = 0; i < number; i++)
        {
            tmp[i] = new MyGridNode();
        }
        return tmp;
    }

    public override void Scan()
    {
        AstarPath.OnPostScan += OnPostScan;

        scanns++;

        if (nodeSize <= 0)
        {
            return;
        }

        GenerateMatrix();

        if (width > 1024 || depth > 1024)
        {
            Debug.LogError("One of the grid's sides is longer than 1024 nodes");
            return;
        }

        SetUpOffsetsAndCosts();

        int gridIndex = GridNode.SetGridGraph(this);

        myGridNodes = CreateMyNodes(width * depth);
        nodes = myGridNodes;
        graphNodes = nodes as GridNode[];

        // important to scan top-down so each node can easily check the type of nodes above it
        for (int y = depth - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                MyGridNode node = graphNodes[y * width + x] as MyGridNode;
                node.SetIndex(y * width + x);
                node.SetGridIndex(gridIndex);

                MyUpdateNodePositionCollision(node, x, y);
            }
        }

        for (int y = 0; y < depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GridNode node = graphNodes[y * width + x];
                CalculateConnections(graphNodes, x, y, node);
            }
        }

        ErodeWalkableArea();
    }

    /** Updates position and walkability for the node. */
    void MyUpdateNodePositionCollision(MyGridNode node, int x, int y)
    {

        node.position = new Int3(matrix.MultiplyPoint3x4(new Vector3(x + 0.5F, 0, y + 0.5F)));
        node.penalty = 0;

        RaycastHit hit;

        // make node non-walkable by default
        bool walkable = false;
        bool isFallLane = false;

        //The position of a node is an Int3, convert it to world coordinate
        Vector3 pos = (Vector3)node.position;
        pos.z -= 1; // translate backward so we can raycast forward onto a tile

        // if Raycast hit a tile, check it's tag
        if (Physics.Raycast(pos, Vector3.forward, out hit, 5f, levelLayerMask))
        {
            //Debug.Log("Found " + hit.transform.gameObject.tag + " at " + node.GetIndex());
            if (hit.transform.gameObject.tag == "Ground") // this node is ground. can't travel in it
            {
                walkable = false;
            }
            else if (hit.transform.gameObject.tag == "Ladder")
            {
                walkable = true;
                node.isLadder = true;
            }
            else if (hit.transform.gameObject.tag == "Rope")
            {
                walkable = true;
                node.isRope = true;
            }
        }
        else // didn't hit anything, so must be air.  Check if walkable or fall lane.
        {
            // Check if we can walk on the ground under this row
            // check tile 1 row down.
            if (Physics.Raycast(pos + Vector3.down, Vector3.forward, out hit, 5f, levelLayerMask))
            {
                if (hit.transform.gameObject.tag == "Ground")
                {
                    walkable = true;
                    node.isSurface = true;
                }
            }

            if (!walkable)
            {
                // Check for cliff drop-off toward right from platforms
                // check tile 1-row down, and 1-colum left is ground
                if (Physics.Raycast(pos + Vector3.down + Vector3.left, Vector3.forward, out hit, 5f, levelLayerMask))
                {
                    if (hit.transform.gameObject.tag == "Ground")
                    {
                        // Check if tile 1-colum left is walkable air
                        if (false == Physics.Raycast(pos + Vector3.left, Vector3.forward, out hit, 5f, levelLayerMask))
                        {
                            isFallLane = true;
                        }
                    }
                }

                // Check for cliff drop-off toward left from platforms
                // check tile 1-row down, and 1-colum right is ground
                if (Physics.Raycast(pos + Vector3.down + Vector3.right, Vector3.forward, out hit, 5f, levelLayerMask))
                {
                    if (hit.transform.gameObject.tag == "Ground")
                    {
                        // Check if tile 1-colum right is walkable air
                        if (false == Physics.Raycast(pos + Vector3.right, Vector3.forward, out hit, 5f, levelLayerMask))
                        {
                            isFallLane = true;
                        }
                    }
                }

                // Check for verticial fall-lane on side of ladders
                // check tile 1 col left is a ladder
                // Commented out because Aron's A* seems to have a bug where it incorrectly thinks it can travel
                // up the one-way fall-lane on the side of a ladder when the ladder is also adjacent to a rope
                /*	if(Physics.Raycast(pos + Vector3.left, Vector3.forward, out hit, 5f, levelLayerMask))
                    {
                        if(hit.transform.gameObject.tag == "Ladder")
                        {
                            isFallLane = true;
                        }
                    }
                    // check tile 1 col right is a ladder
                    if(Physics.Raycast(pos + Vector3.right, Vector3.forward, out hit, 5f, levelLayerMask))
                    {
                        if(hit.transform.gameObject.tag == "Ladder")
                        {
                            isFallLane = true;
                        }
                    }*/

                // Check for verticial fall-lane under ropes
                // check tile 1 row up is a rope
                if (Physics.Raycast(pos + Vector3.up, Vector3.forward, out hit, 5f, levelLayerMask))
                {
                    if (hit.transform.gameObject.tag == "Rope")
                    {
                        isFallLane = true;
                    }
                }

                // Check if node above was already marked as a fall lane so we can continue it
                if (y < depth - 1)
                {
                    var nodeAbove = (MyGridNode)graphNodes[(y + 1) * width + x];
                    if (nodeAbove.isFallLane)
                    {
                        isFallLane = true;
                    }
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////

        node.walkable = walkable || isFallLane;
        node.isTrueWalkable = walkable;
        node.isFallLane = isFallLane;
    }

    public override bool IsValidConnection(GridNode n1, GridNode n2)
    {
        MyGridNode gnode1 = (MyGridNode)n1;
        MyGridNode gnode2 = (MyGridNode)n2;

        Vector3 npos1 = (Vector3)n1.position;
        Vector3 npos2 = (Vector3)n2.position;

        if (gnode1.isTrueWalkable && gnode2.isTrueWalkable)
            return true;
        else if (gnode1.isTrueWalkable && gnode2.isFallLane)
        {
            // connection from true walkable to fall lane below it. i.e. under ropes
            if (npos1.x == npos2.x && npos1.y > npos2.y)
                return true;

            // cliff drop offs
            if (gnode1.isSurface)
            {
                if (npos1.x > npos2.x && npos1.y == npos2.y)
                    return true;
                if (npos1.x < npos2.x && npos1.y == npos2.y)
                    return true;
            }
        }
        else if (gnode1.isFallLane && gnode2.isFallLane)
        {
            if (npos1.x == npos2.x && npos1.y > npos2.y)
                return true;
        }
        else if (gnode1.isFallLane && gnode2.isTrueWalkable)
        {
            // connection for falling down and landing on a rope, ladder or platform
            if (npos1.x == npos2.x && npos1.y > npos2.y)
                return true;
        }

        return false;
    }

    // you'll get warnings about hiding inherited methods, because base class does not
    // declare these as virtual
    public new void SerializeSettings(AstarSerializer serializer)
    {
        base.SerializeSettings(serializer);
        //Save the current values
        serializer.AddValue("levelLayerMask", levelLayerMask.value);
    }

    public new void DeSerializeSettings(AstarSerializer serializer)
    {
        base.DeSerializeSettings(serializer);
        levelLayerMask.value = (int)serializer.GetValue("levelLayerMask", typeof(int));
    }

}

public class MyGridNode : GridNode
{
    public bool isFallLane;
    public bool isTrueWalkable; // true if ground || rope || ladder
    public bool isSurface; // the walkable area along top of ground
    public bool isRope;
    public bool isLadder;
}
