using UnityEngine;
using System.Collections;

public class VitalGlobe : MonoBehaviour {
	
	public GUISkin mySkin;					//The GUISkin that you want to use
	
	public Material globeMaterial;			//The material for the globes

	public Texture2D globeBackground;		//The background for the globes
	public Texture2D globeTexture;			//The globe foreground texture
	
	public float minHealth;					//Min health			NOTE! You should replace this value with the one on your player
	public float maxHealth;					//Max health			NOTE! You should replace this value with the one on your player
	
	public float minMana;					//Min mana				NOTE! You should replace this value with the one on your player
	public float maxMana;					//Max mana				NOTE! You should replace this value with the one on your player
	
	[HideInInspector]
	public Rect globeRect;					//Rect that holds the size and position of the globes
	
	public bool healthGlobe;				//Is it a healthGlobe?
	public bool manaGlobe;					//Is it a manaGlobe?
	
	// Use this for initialization
	void Start () {
		if(healthGlobe)
			//Calculate the size and position of the health globe
			globeRect = new Rect(Screen.width * 0.2f, Screen.height - Screen.width * 0.105f, Screen.width * 0.1f, Screen.width * 0.1f);
		else if(manaGlobe)
			//Calculate the size and position of the mana globe
			globeRect = new Rect(Screen.width * 0.65f, Screen.height - Screen.width * 0.105f, Screen.width * 0.1f, Screen.width * 0.1f);
		
		//Set the size of the labels so that it fits the current size of the screen
		mySkin.label.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
	}
	
	void Update() {
		if(healthGlobe)
			//Set the amount of the bar that's going to be shown based on the min health and max health
			globeMaterial.SetFloat("_Cutoff", -(minHealth/maxHealth) + 1.05f);
		else if(manaGlobe)
			//Set the amount of the bar that's going to be shown based on the min mana and max mana
			globeMaterial.SetFloat("_Cutoff", -(minMana/maxMana) + 1.05f);
	}

	void OnGUI() {
		
		GUI.skin = mySkin;
		
		//NOTE! The following Graphics.DrawTexture is used because GUI.DrawTexture doesn't support materials
		 if(Event.current.type.Equals(EventType.Repaint)){
		//Draw the background for the globes
			Graphics.DrawTexture(globeRect, globeBackground, globeMaterial);
		}
		//Draw the foreground texture of the globe
		GUI.DrawTexture(globeRect, globeTexture);
		
		if(healthGlobe)
			//Draw a label over the health globe saying how much life the player has
			GUI.Label(globeRect, minHealth.ToString() + "/" + maxHealth.ToString());
		else if(manaGlobe)
			//Draw a label over the mana globe saying how much mana the player has
			GUI.Label(globeRect, minMana.ToString() + "/" + maxMana.ToString());
	}
}
