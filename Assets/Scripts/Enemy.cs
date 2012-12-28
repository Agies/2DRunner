using Pathfinding;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class Enemy : Character {
    public EnemyAI enemyAI = EnemyAI.Runner;

    public Player player;
    public Transform playerTransform;
    public Transform target;

    public float repathRate = 0.1F;
    public float pickNextWaypointDistance = 1F;

    protected Seeker seeker;
    protected float lastPathSearch = -9999;
    protected int pathIndex = 0;

    protected Vector3[] path;

    public override void Start () {
	    base.Start();

        seeker = GetComponent<Seeker>();

        Repath();
    }

    public virtual void Repath()
    {
        lastPathSearch = Time.time;

        if (seeker == null || target == null)
        {
            StartCoroutine(WaitToRepath());
            return;
        }

        if (enemyAI == EnemyAI.AstarToPlayer)
        {	// vanilla A* to player
            target.position = playerTransform.position;
        }
        else if (enemyAI == EnemyAI.Runner)
        {
            // Recreate Lode Runner AI

            bool foundWalkableNode = false;

            var gridGraph = (MyGridGraph)AstarPath.active.graphs[0];

            int playerNodeRow = PlayerNodeRow();

            // Attempt to match vertical height of player
            if (Mathf.Abs(characterTransform.position.y - playerTransform.position.y) > 0.1f)
            {
                float minDist = Mathf.Infinity;
                int i, j;
                // scan the row the player is on for the nearest walkable node
                for (i = playerNodeRow * gridGraph.width, j = (playerNodeRow + 1) * gridGraph.width;
                    i < j; i++)
                {
                    if (gridGraph.myGridNodes[i].walkable && !gridGraph.myGridNodes[i].isFallLane)
                    {
                        Vector3 nodePosV3 = (Vector3)gridGraph.nodes[i].position;
                        float dist = Vector3.Distance(characterTransform.position, nodePosV3);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            foundWalkableNode = true;
                            target.position = nodePosV3;
                        }
                    }
                }
            }

            if (!foundWalkableNode)	// target player when vertical height matched 
                target.position = playerTransform.position;
        }

        //Start a new path from transform.positon to target.position, return the result to the function 	OnPathComplete
        seeker.StartPath(characterTransform.position, target.position, OnPathComplete);
    }

    private int PlayerNodeRow()
    {
        GridGraph gridGraph = AstarPath.active.graphs[0] as GridGraph;
        return Mathf.RoundToInt(playerTransform.position.y - (float)gridGraph.nodes[0].position.y / 100f); // node.position is an Int3
    }

    public void OnPathComplete(Path p)
    {
        StartCoroutine(WaitToRepath());

        if (p.error)
        {
            return;
        }

        path = p.vectorPath;

        float minDist = Mathf.Infinity;
        int notCloserHits = 0;

        for (int i = 0; i < path.Length - 1; i++)
        {
            float dist = Mathfx.DistancePointSegmentStrict(path[i], path[i + 1], characterTransform.position);

            if (dist < minDist)
            {
                notCloserHits = 0;
                minDist = dist;
                pathIndex = i + 1;
            }
            else if (notCloserHits > 6)
            {
                break;
            }
        }
    }

    public IEnumerator WaitToRepath()
    {
        float timeLeft = repathRate - (Time.time - lastPathSearch);

        yield return new WaitForSeconds(timeLeft);
        Repath();
    }

    public void Stop()
    {
        pathIndex = -1;
    }
    
    public virtual void ReachedEndOfPath()
    {
    }

    public void Update ()
    {
        bool followPath = true; // follow path given by A*

        // implement behaviour specific to lode runner
        if (enemyAI == EnemyAI.Runner)
        {
            // if we are on a ladder, and at the same vertical height as player
            if ((!onRope && onLadder) && Mathf.Abs(characterTransform.position.y - playerTransform.position.y) < 0.1f)
            {
                // if there is no wall between us, then jump off the ladder in direction of player
                if (GameMaster.IsGroundBetween(characterTransform.position, playerTransform.position) == false)
                {
                    isUp = false;
                    isDown = false;
                    isLeft = false;
                    isRight = false;

                    if (characterTransform.position.x - playerTransform.position.x > 0)
                        isLeft = true;
                    isRight = !isLeft;

                    followPath = false; // don't follow A* path for this Update
                }
            }
            else if (onRope)
            {
                // on rope and player is lower than us
                if (characterTransform.position.y - playerTransform.position.y >= 1) // jump off rope if player is >= 1 below 
                {
                    isDown = true;
                    followPath = false; // don't follow A* path for this Update
                }
            }
        }

        // standard A* path following
        if (followPath && path != null && pathIndex < path.Length && pathIndex >= 0)
        {
            //Change target to the next waypoint if the current one is close enough
            Vector3 currentWaypoint = path[pathIndex];
            currentWaypoint.z = characterTransform.position.z;

            while ((currentWaypoint - characterTransform.position).sqrMagnitude < pickNextWaypointDistance * pickNextWaypointDistance)
            {
                pathIndex++;
                if (pathIndex >= path.Length)
                {
                    //Use a lower pickNextWaypointDistance for the last point. If it isn't that close, then decrement the pathIndex to the previous value and break the loop
                    if ((currentWaypoint - characterTransform.position).sqrMagnitude < (pickNextWaypointDistance * 0.2) * (pickNextWaypointDistance * 0.2))
                    {
                        ReachedEndOfPath();
                        break;
                    }
                    else
                    {
                        pathIndex--;
                        //Break the loop, otherwise it will try to check for the last point in an infinite loop
                        break;
                    }
                }
                currentWaypoint = path[pathIndex];
                currentWaypoint.z = characterTransform.position.z;
            }

            Vector3 dir = currentWaypoint - characterTransform.position;

            isUp = false;
            isDown = false;
            isLeft = false;
            isRight = false;

            if (dir.magnitude > 0)
            {
                if (onRope)
                    isDown = dir.y <= -0.1f;
                else
                    isDown = dir.y < -0f; // don't fall off rope but do descend ladders

                isUp = dir.y > 0f;
                isLeft = dir.x < -0f;
                isRight = dir.x > 0f;
            }
        }

        // sctual Character movement 
        UpdateMovement();
    }
}

public enum EnemyAI
{
    AstarToPlayer, 
    Runner
}
