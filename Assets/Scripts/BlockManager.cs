using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public Transform prefab;
    public int numberOfObjects;
    protected Queue<Transform> objectQueue;

    protected virtual void Start()
    {
        objectQueue = new Queue<Transform>(numberOfObjects);
        for (int i = 0; i < numberOfObjects; i++)
        {
            objectQueue.Enqueue((Transform)Instantiate(prefab, new Vector3(0f, 0f, -100f), Quaternion.identity));
        }
        enabled = false;
    }

    protected virtual void Awake()
    {
        Messenger.Default.Register<GameStartMessage>(this, OnGameStart);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
    }

    protected virtual void OnGameOver(GameOverMessage obj)
    {
        enabled = false;
    }

    protected virtual void OnGameStart(GameStartMessage obj)
    {
        enabled = true;
    }
}