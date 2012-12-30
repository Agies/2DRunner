using UnityEngine;
using System.Collections;

public class Booster : MonoBehaviour
{

    public Vector3 offset;
    public Vector3 rotationVelocity;
    public float recycleOffset;
    public float spawnChance;
    private Transform _transform;
    private GameObject _gameObject;

    public void SpawnIfAvailable(Vector3 position)
    {
        if (gameObject.active || spawnChance <= Random.Range(0f, 100f))
        {
            return;
        }
        _transform.localPosition = position + offset;
        _gameObject.SetActive(true); 
    }

    void Awake()
    {
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
        _transform = transform;
        _gameObject = gameObject;
    }

    private void OnGameOver(GameOverMessage obj)
    {
        _gameObject.SetActive(false);
    }

    void Start () {
        _gameObject.SetActive(false);
	}
	
	void Update () {
	    if ((transform.localPosition.x + recycleOffset) < Runner.DistanceTraveled)
	    {
	        _gameObject.SetActive(false);
	        return;
	    }
        _transform.Rotate(rotationVelocity * Time.deltaTime);
	}

    void OnTriggerEnter()
    {
        Runner.AddBoost();
        gameObject.SetActive(false);
    }
}
