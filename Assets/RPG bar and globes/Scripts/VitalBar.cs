using UnityEngine;
using System.Collections;

public class VitalBar : MonoBehaviour {
	
	public GUISkin mySkin;						//The GUISkin that you want to use.
			
	public Material manaBarMaterial;			//The material for the mana bar
	public Material healthBarMaterial;			//The material for the health bar
	public Texture2D bar;						//The texture for the bars
	public Texture2D BarForegroundTexture;		//The foreground texture for the health bar
	public Texture2D barBackgroundTexture;		//The texture behind the bars
		
	public float minHealth;						//Min health			NOTE! You should replace this value with the one on your player
	public float maxHealth;						//Max health			NOTE! You should replace this value with the one on your player
	
	public float minMana;						//Min mana				NOTE! You should replace this value with the one on your player
	public float maxMana;						//Max mana				NOTE! You should replace this value with the one on your player
	
	private Rect barRect;						//Rect to hold the size and position of the bar

	void Start () {

		//Calculate the size and position of the bar
		barRect = new Rect(Screen.width * 0.01f, Screen.height * 0.01f, Screen.width * 0.3f, (Screen.width * 0.3f) / ((float)BarForegroundTexture.width/(float)BarForegroundTexture.height));
		
		//Set the fontsize so that it scale depending on the screen size
		mySkin.label.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
	}
	
	void Update() {
		//Set the amount of the bar that's going to be shown based on the min health and max health
		healthBarMaterial.SetFloat("_Cutoff", -(minHealth/maxHealth) + 1.05f);
		//Set the amount of the bar that's going to be shown based on the min mana and max mana
		manaBarMaterial.SetFloat("_Cutoff", -(minMana/maxMana) + 1.05f);
	}
	
	void OnGUI() {
		
		GUI.skin = mySkin;
		
		GUILayout.BeginArea(barRect);
		
		//Calculate the size and position of the mana bar
		Rect manaRect = new Rect(barRect.width * 0.3f, barRect.height * 0.54f, barRect.width * 0.51f, barRect.height * 0.19f);
		//Calculate the size and positon of the health bar
		Rect healthRect = new Rect(barRect.width * 0.3f, barRect.height * 0.27f, barRect.width * 0.69f, barRect.height * 0.18f);
		
		//Draw the mana bar background
		GUI.DrawTexture(manaRect, barBackgroundTexture);
		//Draw the healht bar background
		GUI.DrawTexture(healthRect, barBackgroundTexture);
		
		//NOTE! The following Graphics.DrawTexture is used because GUI.DrawTexture doesn't support materials
		 if(Event.current.type.Equals(EventType.Repaint)) {
			
			//Draw the health bar
			Graphics.DrawTexture(healthRect,bar, healthBarMaterial);
			//Draw the mana bar
			Graphics.DrawTexture(manaRect,bar, manaBarMaterial);
			
		}
		//Draw a label over the health bar saying how much life the player has
		GUI.Label(healthRect, minHealth.ToString() + "/" + maxHealth.ToString());
		//Draw a label over the mana bar saying how much mana the player has
		GUI.Label(manaRect, minMana.ToString() + "/" + maxMana.ToString());
				
				
		GUILayout.EndArea();
		
		//Draw the vital bar foreground
		GUI.DrawTexture(barRect, BarForegroundTexture);
	}
}