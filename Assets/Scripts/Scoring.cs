using UnityEngine;
using System.Collections;

public class Scoring : MonoBehaviour 
{
	
	public GUIText scoreLabelText;
	public GUIText scoreValueText;
	
	public GUIText livesLabelText;
	public GUIText livesValueText;
	
	public GUIText levelLabelText;
	public GUIText levelValueText;
	
	// initial score, lives and level values
	private int scoreValue = 0;
	public int lifeValue = 5;
	private int levelValue = 1;
	
	private int pickupValue = 250; // pickups will score this many points
	
	public Color materialColor = new Color(0.62f, 0f, 0f, 1F); // define our red color to match the level borders
	
	// Use this for initialization
	void Start () 
	{
		// change the color of the interface lables to match the color of the top and bottom borders
		scoreLabelText.material.color = materialColor;
		livesLabelText.material.color = materialColor;
		levelLabelText.material.color = materialColor;
		
		// set lives and levels to initial values and add leading zeroes
		livesValueText.text = lifeValue.ToString("D3");
		levelValueText.text = levelValue.ToString("D3");
	}
	
	// this function is called by Player.cs when the player touches an object with the Pickup tag
	public void Pickup() 
	{
		scoreValue += pickupValue;
		scoreValueText.text = scoreValue.ToString("D7"); // leading zeroes!
	}
	
	public void LifeAdd() 
	{
	}
	
	public void LifeSubtract() 
	{
		lifeValue -= 1;
		if(lifeValue >= 0) 
		{
			livesValueText.text = lifeValue.ToString("D3");
		}
	}
	
	public void LevelAdd() 
	{
	}
}
