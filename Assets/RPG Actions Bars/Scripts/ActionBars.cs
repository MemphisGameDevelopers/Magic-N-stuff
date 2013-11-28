using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ActionBarSlot {
	public Spell spell;
	public Item item;
	public KeyCode keycode;
}

public class ActionBars : MonoBehaviour {
	
	public GUISkin mySkin;								//The GUISkin you want to use.
	
	public Texture2D cooldownTexture;					//The texture for cooldown overlay. 
	
	public Texture2D frame;								//The frame around the icons and slots
	
	public Material cooldownMaterial;					//The material for cooldown
	
	public int leftActionBarWidth;						//The width of the left actionbar
	public int rightActionBarWidth;						//The width of the right actionbar
	public int bottomActionBarWidth;					//The width of the right actionbar
	
	public List<ActionBarSlot> leftActionbarSlots;		//The list of the left actionbar slots
	public List<ActionBarSlot> rightActionbarSlots;		//The list of the right actionbar slots
	public List<ActionBarSlot> bottomActionbarSlots;	//The list of the bottom actionbar slots
	
	public bool left;									//Is the left actionbar?
	public bool right;									//Is the right actionbar?
	public bool bottom;									//Is the bottom actionbar?
	
	[HideInInspector]
	public bool hoverActionBar;							//Are we hovering over the actionbar?
	
	private SpellBook spellbook;						//Reference to the spellbook
	private InventoryManager inventory;					//Reference to the inventory
	private string hoverName;							//The name of the hovered item
	private float slotSize;								//The size of the slots
	private float slotBorder;							//How much space is there between the slots
	private Rect leftActionbarsRect;					//The size and position of the size where the left actionbars are drawn
	private Rect rightActionbarsRect;					//The size and position of the size where the right actionbars are drawn
	private Rect bottomActionbarsRect;					//The size and position of the size where the bottom actionbars are drawn
	private float screenWidth;							//The value to hold the width of the screen
	private float screenHeigth;							//The value to hold the height of the screen
	
	// Use this for initialization
	void Start () {
		//Find the spellbook and get spellbook script from it
		spellbook = GameObject.FindGameObjectWithTag("SpellBook").GetComponent<SpellBook>();
		//Find the inventory and get inventory script from it
		inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
		//Resize the GUI to fit the screen
		ResizeGUI();
	}
	
	// Update is called once per frame
	void Update () {
		
		//If the height or the width of the screen changes - resize the gui
		if(Screen.width != screenWidth || Screen.height != screenHeigth)
			ResizeGUI();
		
		//This part resizes the fontsize according to the size of the screen
		mySkin.box.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
		mySkin.label.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
		mySkin.textArea.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
		mySkin.GetStyle("Stacksize").fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
		mySkin.GetStyle("RibbonLabel").fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
	}
	
	void ResizeGUI() {
		//Calculate the size of the slots
		slotSize = Screen.width * 0.04f;
		//Calculate the size of the space between the slots
		slotBorder = slotSize * 0.075f;
		//Calculate the size and position of the left bars
		leftActionbarsRect = new Rect(slotBorder, Screen.height * 0.1f, Screen.width * 0.2f, Screen.height * 0.8f);
		//Calculate the size and position of the right bars
		rightActionbarsRect = new Rect(Screen.width - rightActionBarWidth * (slotSize + slotBorder), Screen.height * 0.1f, Screen.width * 0.2f, Screen.height * 0.8f);
		//Calculate the size and position of the bottom bars
		bottomActionbarsRect = new Rect(Screen.width * 0.5f - (bottomActionbarSlots.Count * (slotSize + slotBorder)) * 0.5f, Screen.height - (slotSize + slotBorder), Screen.width * 0.8f, (slotSize + slotBorder));
		//Store the width of the screen
		screenWidth = Screen.width;
		//Store the height of the screen
		screenHeigth = Screen.height;
	}
	
	void OnGUI() {
		hoverActionBar = false;
		
		hoverName = "";
		
		GUI.depth = 1;
		
		GUI.skin = mySkin;
				
		Event curEvent = Event.current;
				
		int j;
		int k;
		
		Spell spell = null;
		Item item = null;
		
		Rect actionBarRect = new Rect(0,0,0,0);
		int actionBarAmount = 0;
		int actionBarWidth = 0;
		List<ActionBarSlot> actionBarSlots = new List<ActionBarSlot>();
		
		if(right){
			actionBarRect = rightActionbarsRect;
			actionBarWidth = rightActionBarWidth;
			actionBarSlots = rightActionbarSlots;
			actionBarAmount = rightActionbarSlots.Count;
		}
		if(bottom) {
			actionBarRect = bottomActionbarsRect;
			actionBarWidth = bottomActionBarWidth;
			actionBarSlots = bottomActionbarSlots;
			actionBarAmount = bottomActionbarSlots.Count;
		}
		if(left) {
			actionBarRect = leftActionbarsRect;
			actionBarWidth = leftActionBarWidth;
			actionBarSlots = leftActionbarSlots;
			actionBarAmount = leftActionbarSlots.Count;
		}
		
		GUILayout.BeginArea(actionBarRect);
		for(int i = 0; i < actionBarAmount; i++) {
			j = i /	actionBarWidth;
			k = i % actionBarWidth;
			//Calculate the size and position of the currentRect
			Rect currentRect = (new Rect(k * (slotSize + slotBorder), j * (slotSize + slotBorder), slotSize, slotSize));
			//If we're hovering over a actionbar slot
			if(currentRect.Contains(curEvent.mousePosition)) {
				hoverActionBar = true;
			}
			//If the slots contains an item
			if(actionBarSlots[i].item) {
				//store the item of the slot
				item = actionBarSlots[i].item;
				//If the item has an icon then draw the icon as the slot
				if(item.icon)
					GUI.DrawTexture(currentRect, item.icon);
				//If the stacksize of the item is greater than zero
				//Draw a label saying the size of the stack
				if(actionBarSlots[i].item.stackSize > 0)
					GUI.Label(currentRect, actionBarSlots[i].item.stackSize.ToString(),"Stacksize");	
			}
			//Else if the actionbar contains a spell
			else if(actionBarSlots[i].spell) {
				//Store the spell of the slot
				spell = actionBarSlots[i].spell;
				//If the spell has an icon
				//Draw the icon as the slot
				if(spell.icon)
					GUI.DrawTexture(currentRect, spell.icon);
			}
			//If there's no item or spell in the slot
			else {
				//Just draw a simple box
				GUI.Box(currentRect, "");
			}
			
			//Store the keycode of the slot as a string
			string keycodeString = actionBarSlots[i].keycode.ToString();
			
			//If the keycode is 6 characters long then assume it's of type "Alpha + number" example: Alpha1
			if(keycodeString.Length == 6) {
				//Remove the first 5 characters so that it only contains the number.
				keycodeString = keycodeString.Substring(5, 1);
			}
			//If keycode isn't "None" then draw a label containing the keycode
			if(keycodeString != "None")
				GUI.Label(currentRect, keycodeString);
			
			//If the mouse is hovering over the slot
			if(currentRect.Contains(curEvent.mousePosition)) {
				//If the slot contains a spell
				if(actionBarSlots[i].spell)
					//Set the hovername equal to the name of the spell
					hoverName = spell.spellName;
				//Else if the slot contains an item
				else if(actionBarSlots[i].item)
					//Set the hovername equal to the name of the spell
					hoverName = item.itemName;
				//If the player release the mouse over the slot
				if((curEvent.type == EventType.MouseUp)) {
					//If we're dragging a spell
					if(spellbook.draggedSpell) {
						//If there's allready a spell in the slot
						//Then replace the dragged spell with the one in the slot
						if(actionBarSlots[i].spell) {
							actionBarSlots[i].spell = spellbook.draggedSpell;
							spellbook.draggedSpell = spell;
							spellbook.dragging = true;
						}
						//Else if there's an item in the slot
						//Replace the item in the slot with the spell and set the item to be dragged
						else if(actionBarSlots[i].item) {
							actionBarSlots[i].spell = spellbook.draggedSpell;
							spellbook.draggedSpell = null;
							spellbook.dragging = false;
							inventory.draggedItem = item;
							inventory.dragging = true;
							if(actionBarSlots[i].item)
								actionBarSlots[i].item = null;
							else if(actionBarSlots[i].spell)
								actionBarSlots[i].spell = null;
						}
						//If there's nothing in the slot then just put the spell in the slot
						else {
							actionBarSlots[i].spell = spellbook.draggedSpell;
							spellbook.draggedSpell = null;
							spellbook.dragging = false;
						}
					}
					//Else if we're dragging an item
					else if(inventory.draggedItem) {
						//If we the slot contains either a spell or an item
						if(actionBarSlots[i].spell || actionBarSlots[i].item) {
							//If it's a spell then replace it with the dragged item
							if(actionBarSlots[i].spell) {
								actionBarSlots[i].item = inventory.draggedItem;
								inventory.draggedItem = null;
								inventory.items[inventory.dragOrigin] = inventory.draggedItem;
								spellbook.draggedSpell = spell;
								spellbook.dragging = true;
								actionBarSlots[i].spell = null;
							}
							//Else if there's an item in the slot replace it with the dragged item
							else {
								actionBarSlots[i].item = inventory.draggedItem;
								inventory.items[inventory.dragOrigin] = inventory.draggedItem;
								inventory.draggedItem = item;
								inventory.dragging = true;
							}
						}
						//If there's nothing in the slot then put the dragged item in the slot
						else {
							actionBarSlots[i].item = inventory.draggedItem;
							inventory.items[inventory.dragOrigin] = inventory.draggedItem;
							inventory.draggedItem = null;
							inventory.dragging = false;
						}
					}
				}
				//If the player presses the mouse on a slot and not pressing the shift key and the inventory or the spell is dragging something
				else if(curEvent.type == EventType.MouseDown && !curEvent.shift && !inventory.dragging && !spellbook.dragging) {
					//If there's a spell in the slot use the spell
					if(actionBarSlots[i].spell) {
						if(!actionBarSlots[i].spell.onCooldown)
							actionBarSlots[i].spell.Use();
					}
					//If there's an item in the slot use the item
					else if(actionBarSlots[i].item) {
						if(!actionBarSlots[i].item.onCooldown)
							actionBarSlots[i].item.Use();
					}
				}
				//If the player drags the mouse and is holding the shift key and we're not currently dragging anything and the player isn't releasing the mouse or pressing it down
				else if(curEvent.type == EventType.MouseDrag && curEvent.shift && (!spellbook.draggedSpell || !inventory.draggedItem) && (curEvent.type != EventType.MouseDown || curEvent.type != EventType.MouseUp)) {
					//If there's a spell in the slot
					//Set the spell in the slot to being dragged and remove it from the slot
					if(actionBarSlots[i].spell) {
						spellbook.dragging = true;
						spellbook.draggedSpell = actionBarSlots[i].spell;
						actionBarSlots[i].spell = null;
					}
					//If there's an item in the slot
					//Set the item in the slot to being dragged and remove it from the slot
					else if(actionBarSlots[i].item) {
						inventory.dragging = true;
						inventory.draggedItem = actionBarSlots[i].item;
						actionBarSlots[i].item = null;
					}
				}
			}
			//If there's a keycode set for the slot and player pressing the button
			if(curEvent.keyCode == actionBarSlots[i].keycode && curEvent.keyCode != KeyCode.None && curEvent.type == EventType.KeyDown) {
				//If there's a spell in the slot
				if(actionBarSlots[i].spell) {
					//If the spell is not on cooldown use the spell
					if(!actionBarSlots[i].spell.onCooldown) {
						actionBarSlots[i].spell.Use();
					}
				}
				//Else if there's an item in the slot
				else if(actionBarSlots[i].item && actionBarSlots[i].item.itemType == EquipmentSlotType.consumable) {
					//If the item isn't on cooldown use the item
					if(!actionBarSlots[i].item.onCooldown)
						actionBarSlots[i].item.Use();
				}
			}
			//If there's a spell in the slot and the spell is on cooldown
			if(actionBarSlots[i].spell && actionBarSlots[i].spell.onCooldown) {
				if(Event.current.type.Equals(EventType.Repaint)) {
					//Set the cutoff of the cooldown material to match the state of the cooldown
					cooldownMaterial.SetFloat("_Cutoff", actionBarSlots[i].spell.cooldownTimer / actionBarSlots[i].spell.cooldown);
					//Draw the cooldown texture over the spell
		            Graphics.DrawTexture(currentRect, cooldownTexture, cooldownMaterial);
		    	}
			}
			//If there's an item in the slot and the item is on cooldown
			else if(actionBarSlots[i].item && actionBarSlots[i].item.onCooldown) {
				if(Event.current.type.Equals(EventType.Repaint)) {
					//Set the cutoff of the cooldown material to match the state of the cooldown
					cooldownMaterial.SetFloat("_Cutoff", actionBarSlots[i].item.cooldownTimer / actionBarSlots[i].item.cooldown);
					//Draw the cooldown texture over the item
		            Graphics.DrawTexture(currentRect, cooldownTexture, cooldownMaterial);
		    	}
			}
			//Draw the frame over the slot
			GUI.DrawTexture(currentRect, frame);
			
		}
		GUILayout.EndArea();
		//If there's something set as the hovername
		if(hoverName != "")
			//Draw a box containing the hovername
			GUI.Box(new Rect(curEvent.mousePosition.x - mySkin.box.CalcSize(new GUIContent(hoverName)).x * 0.5f, curEvent.mousePosition.y - mySkin.box.CalcSize(new GUIContent(hoverName)).y , mySkin.box.CalcSize(new GUIContent(hoverName)).x, mySkin.box.CalcSize(new GUIContent(hoverName)).y),hoverName, "textArea");
	}
}
