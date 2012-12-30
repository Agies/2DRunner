using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour
{
    public GUIText gameOverText;
    public GUIText instructionText;
    public GUIText runnerText;
    public GUIText boostText;
    public GUIText distanceText;

    void Awake()
    {
        Messenger.Default.Register<GameStartMessage>(this, OnGameStart);  
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
        Messenger.Default.Register<DistanceChangedMessage>(this, OnDistanceChanged);
        Messenger.Default.Register<BoostChangedMessage>(this, OnBoostChanged);
    }

    private void OnBoostChanged(BoostChangedMessage obj)
    {
        boostText.text = obj.Boosts.ToString();
    }

    private void OnDistanceChanged(DistanceChangedMessage obj)
    {
        distanceText.text = obj.Distance.ToString("f0");
    }

    private void OnGameOver(GameOverMessage obj)
    {
        gameOverText.enabled = true;
        instructionText.enabled = true;
        enabled = true;
    }

    void Start ()
	{
	    gameOverText.enabled = false;
        runnerText.enabled = true;
        instructionText.enabled = true;
	}

    private void OnGameStart(GameStartMessage obj)
    {
        gameOverText.enabled = false;
        instructionText.enabled = false;
        runnerText.enabled = false;
        enabled = false;
    }

    void Update () {
	    if (Input.GetButtonDown("Jump"))
            Messenger.Default.Send(new GameStartMessage());
	}
}

public class BoostChangedMessage
{
    public int Boosts;

    public BoostChangedMessage(int boosts)
    {
        Boosts = boosts;
    }
}

public class DistanceChangedMessage
{
    public float Distance;

    public DistanceChangedMessage(float distanceTraveled)
    {
        Distance = distanceTraveled;
    }
}

public class GameStartMessage
{
}

public class GameOverMessage
{
}
