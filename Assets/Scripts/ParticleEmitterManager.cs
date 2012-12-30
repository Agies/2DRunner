using UnityEngine;
using System.Collections;

public class ParticleEmitterManager : MonoBehaviour
{
    public ParticleEmitter[] emitters;
    
    void Awake()
    {
        Messenger.Default.Register<GameStartMessage>(this, OnGameStart);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
    }

    private void OnGameOver(GameOverMessage obj)
    {
        foreach (var emitter in emitters)
        {
            emitter.ClearParticles();
            emitter.emit = false;
        }
    }

    private void OnGameStart(GameStartMessage obj)
    {
        foreach (var emitter in emitters)
        {
            emitter.emit = true;
        }
    }

    // Use this for initialization
	void Start () {
	    OnGameOver(new GameOverMessage());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
