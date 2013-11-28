using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RibbonTabs {
	Nature,
	Arcane,
	Fire,
	Frost
}

[System.Serializable]
public class Ribbons {
	public Texture2D normal;
	public Texture2D hover;
	public RibbonTabs tab;
}

public class SpellBook : MonoBehaviour {
	
	public GUISkin mySkin;						//The GUISkin to use

	public Texture2D cooldownTexture;			//The cooldown texture
	public Texture2D spellEmpty;				//The empty spell texture
	public Texture2D tome;						//The tome icon
	public Texture2D spellBookBackground;		//The spellbook image
	public Texture2D frame;						//The frame image

	public Material cooldownMaterial;			//The cooldown material
		
	public List<Spell> nature;					//The list of nature spells
	public List<Spell> arcane;					//The list of arcane spells
	public List<Spell> fire;					//The list of fire spells
	public List<Spell> frost;					//The list of frost spells
	
	public Ribbons[] ribbons;					//An array of ribbons
		
	public int spellBookCapacity;				//How many spells can each tab of the spellbook contain?
	public int spellBookPageWidth;				//The width of the spellbook
	
	[HideInInspector]
	public Spell draggedSpell;					//The spell that's being dragged
	[HideInInspector]
	public bool dragging;						//Is a spell being dragged?
	[HideInInspector]
	public List<ActionBars> actionbars;			//A list of the actionbars
	
	private List<Spell> spells;					//The list of spells
	private Rect spellBookRect;					//The size and postion of the spellbook
	private Rect spellIconArea;					//The area of where the spells are being draw
	private bool showSpellbook;					//Is the spellbook to be shown or not?
	private float tabSpaceAmount;				//The space between the tabs
	private RibbonTabs selectedTab = RibbonTabs.Nature;	//What tab is currently selected?
	private string hoveredSpellName = "";		//The name of the spell that contains the mouse position
	private float spellSpaceAmount;				//The space between the spells
	private float spellIconSize;				//The size of the slots
	private Rect tomeRect;						//The size and position of the tome icon
	
	private float screenWidth;
	private float screenHeight;
	
	// Use this for initialization
	void Start () {
		//Create a temp array of the actionbar gameobjects.
		GameObject[] temp = GameObject.FindGameObjectsWithTag("ActionBars");
		//Create a temp list of actionbars
		actionbars = new List<ActionBars>();
		//Run through all the gameobjects in array of the actionbar gameobjects
		foreach(GameObject obj in temp) {
			//Add the ActionBars component of the actionbar gameobjects to the list of actionbars
			actionbars.Add(obj.GetComponent<ActionBars>());
		}
	}
	
	void Update() {
		//If there size of the screen changes then resize the GUI
		if(screenWidth != Screen.width || screenHeight != Screen.height)
			ResizeGUI();
		
		//If the player presses the escape key close the spellbook
		if(Input.GetKeyDown(KeyCode.Escape))
			showSpellbook = false;
	}
	
	//This resizes the GUI when the size of the screen changes
	void ResizeGUI() {
		tabSpaceAmount = Screen.height * 0.065f;
		spellBookRect = new Rect(Screen.width * 0.15f, Screen.width * 0.1f, Screen.width * 0.375f, Screen.width * 0.3f);
		spellIconArea = new Rect(spellBookRect.width * 0.135f + spellBookRect.xMin, spellBookRect.height * 0.135f + spellBookRect.yMin, spellBookRect.width * 0.8f, spellBookRect.height * 0.8f);
		spellIconSize = spellIconArea.width * 0.5f * 0.3f;
		spellSpaceAmount = spellIconSize * 0.25f;
		tomeRect = new Rect(Screen.width * 0.907f, Screen.height * 0.925f, Screen.width * 0.04f, Screen.width * 0.04f);
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}
	
	void OnGUI () {
		
		bool hoverActionbar = false;
		
		foreach(ActionBars ab in actionbars) {
			if(ab.hoverActionBar)
				hoverActionbar = true;
		}
				
		hoveredSpellName = "";
				
		Event curEvent = Event.current;
		
		GUI.skin = mySkin;
		
		if(showSpellbook)
			DrawSpellbook(curEvent);
		
		//If the player is dragging a spell and releases the mouse remove the dragged spell
		if(curEvent.isMouse && (curEvent.type == EventType.MouseDown || curEvent.type == EventType.MouseUp) && draggedSpell && !hoverActionbar) {
			dragging = false;
			draggedSpell = null;
		}
		
		GUI.depth = 0;
		
		//If a spell is being dragged draw the spell at the position of the mouse
		if(draggedSpell)
			GUI.DrawTexture(new Rect(curEvent.mousePosition.x, curEvent.mousePosition.y, spellIconSize, spellIconSize), draggedSpell.icon);
				
		//Draw the tome icon
		GUI.DrawTexture(tomeRect, tome);
		//Draw a frame around the tome icon
		GUI.DrawTexture(tomeRect, frame);
		
		//If the player presses the p key or the clicks on the tome icon either close or open the spellbook
		if(tomeRect.Contains(curEvent.mousePosition) && curEvent.type == EventType.MouseDown || (curEvent.keyCode == KeyCode.P && curEvent.type == EventType.KeyDown)) {
			showSpellbook = !showSpellbook;
		}
		
		//If the mouse is over a spell draw the name of the spell as a box above it
		if(hoveredSpellName != "") {
			GUI.Box(new Rect(curEvent.mousePosition.x - mySkin.box.CalcSize(new GUIContent(hoveredSpellName)).x * 0.5f, curEvent.mousePosition.y - mySkin.box.CalcSize(new GUIContent(hoveredSpellName)).y , mySkin.box.CalcSize(new GUIContent(hoveredSpellName)).x, mySkin.box.CalcSize(new GUIContent(hoveredSpellName)).y),hoveredSpellName, "textArea");
		}
	}
	
	void DrawSpellbook(Event curEvent) {
		//Draw the spellbook background
		GUI.DrawTexture(spellBookRect, spellBookBackground);
		
		for(int i = 0; i < ribbons.Length; i++) {
			//Calculate the size and position for the ribbons
			Rect currentRect = new Rect( spellBookRect.xMax - Screen.width * 0.023f + i * Screen.width * 0.001f, spellBookRect.y + Screen.height * 0.05f + i * tabSpaceAmount, Screen.width * 0.06f, Screen.height * 0.05f);
			//If the mouse is hovering over the ribbon
			if(currentRect.Contains(curEvent.mousePosition)) {
				//Draw the ribbon hover texture
				GUI.DrawTexture(currentRect, ribbons[i].hover);
				//If the player clicks on a ribbon with the left mouse button set the selected tab to the one pressed
				if(curEvent.button == 0 && curEvent.type == EventType.mouseDown) {
					selectedTab = ribbons[i].tab;
				}
			}
			//If the mouse isn't hovering over the ribbon then draw the normal ribbon texture
			else {
				GUI.DrawTexture(currentRect, ribbons[i].normal);
			}
			//Make a label containing the name of the tab over the ribbon
			GUI.Label(new Rect(currentRect.x + currentRect.width * 0.1f, currentRect.y, currentRect.width, currentRect.height), ribbons[i].tab.ToString(), "RibbonLabel");
		}
		
		int j;
		int k;
		GUILayout.BeginArea(spellIconArea);
		for(int i = 0; i < spellBookCapacity; i++) {
			j = i /	spellBookPageWidth;
			k = i % spellBookPageWidth;
			Rect currentRect;
			Spell spell = null;
			
			//If the code has runned through less than half of the spells in the spellbook draw it on the first page of the book
			if(i < spellBookCapacity * 0.5f) {
				currentRect = new Rect(k * (spellIconSize + spellSpaceAmount),j * (spellIconSize + spellSpaceAmount) ,spellIconSize, spellIconSize);
			}
			//Else if it has runned through over half draw the spells on the second page
			else {
				currentRect = new Rect(spellBookRect.width * 0.45f + k * (spellIconSize + spellSpaceAmount),j * (spellIconSize + spellSpaceAmount) - spellBookCapacity * 0.25f * spellIconSize - (spellSpaceAmount * spellBookCapacity * 0.25f) ,spellIconSize, spellIconSize);
			}
			
			if(selectedTab == RibbonTabs.Nature)
				spells = nature;
			else if(selectedTab == RibbonTabs.Arcane)
				spells = arcane;
			else if(selectedTab == RibbonTabs.Fire)
				spells = fire;
			else if(selectedTab == RibbonTabs.Frost)
				spells = frost;
			
			if(spells.Count > i) {
				spell = spells[i];
				//If there's a spell and the spell has a icon applied then draw the icon
				if(spell && spell.icon) {
					GUI.DrawTexture(currentRect,spell.icon);
				}
				//If the mouse is hovering over the spell
				if(currentRect.Contains(curEvent.mousePosition)) {
					//Set the name of the hovered spell
					hoveredSpellName = spells[i].spellName;
					//If the player drags a spell and there's no spell currently being dragged
					//Then set it as the dragged spell
					if(curEvent.type == EventType.MouseDrag && !dragging) {
						dragging = true;
						draggedSpell = spell;
					}
					//If the player presses on the spell while not dragging and the spell isn't on cooldown
					//Then cast the spell
					else if(curEvent.type == EventType.MouseUp && !dragging && !spell.onCooldown) {
						spell.Use();
					}
				}
				//If the spell is on cooldown
				if(spell.onCooldown) {
					if(Event.current.type.Equals(EventType.Repaint)) {
						//Set the cutoff of the cooldown material to match the state of the cooldown
						cooldownMaterial.SetFloat("_Cutoff", spell.cooldownTimer / spell.cooldown);
						//Draw the cooldown texture over the item
			            Graphics.DrawTexture(currentRect, cooldownTexture, cooldownMaterial);
			    	}
				}
			}
			//The slot are empty
			//Draw the texture for the empty slots with a opacity of 70%
			else {
				GUI.color = new Color(0.4f,0.4f,0.4f,0.7f);
				GUI.DrawTexture(currentRect, spellEmpty);
				GUI.color = new Color(1f,1f,1f,1f);
			}
		}
		GUILayout.EndArea();
	}
}
