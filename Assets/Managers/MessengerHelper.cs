using UnityEngine;
using System.Collections;

public class MessengerHelper : MonoBehaviour {
    
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnDisable()
    {
        Messenger.Default.Cleanup();
    }
}
