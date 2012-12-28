using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour 
{
	
	private GameObject _gameObject;
	private Renderer _renderer;
	
	void Awake() 
	{
		_gameObject = gameObject;
		_renderer = renderer;
	}
	
	// Called from Player.cs when the player enteres the pickup
	public void PickMeUp()
	{
		_renderer.enabled = false; // hide the pickup
		_gameObject.tag = "Untagged"; // untag the pickup so it won't get triggered again
	}
}
