using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour {
	
	public GUISkin mySkin;						//The GUISkin to be used
	
	public Texture2D frame;						//The frame texture
	public Texture2D cooldownTexture;			//The cooldown texture
	public Material cooldownMaterial;			//The cooldown material
	
	public Texture2D bagpackImage;				//The image of the backpack
	
	public int inventoryWidth;					//How many slots wide will the inventory be?
	
	public List<Item> items;					//The list of the items in the inventory
	
	[HideInInspector]
	public int dragOrigin;						//The origin of the dragged item
	[HideInInspector]
	public Item draggedItem;					//The dragged item
	[HideInInspector]
	public bool dragging;						//Are the player dragging or not?
	private float itemIconSize;					//The size of the slots
	[HideInInspector]
	public List<ActionBars> actionbars;			//A list of the actionbars
		
	private bool showInventory;					//Are the inventory to be shown or not?
	private Rect bagpackRect;					//The size of the backpack image
	private string hoverName = "";				//The name of the hovered item
	private float slotBorder;					//The size of the border between slots
	private Rect inventoryRect;					//The size and position of the inventory
	
	private float screenWidth;
	private float screenHeight;
	
	void Start() {
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
	
	// Update is called once per frame
	void Update () {
		//If there size of the screen changes resize the GUI
		if(screenWidth != Screen.width || screenHeight != Screen.height)
			ResizeGUI();
		
		//If the player presses the escape key don't show the inventory any more
		if(Input.GetKeyDown(KeyCode.Escape))
			showInventory = false;
	}
	
	void ResizeGUI() {
		//Calculate the size of the slots
		itemIconSize = Screen.width * 0.035f;
		//Calculate the size of the border
		slotBorder = itemIconSize * 0.05f;
		//Calculate the size and position of the inventory
		inventoryRect = new Rect(Screen.width * 0.6f, Screen.height * 0.15f, itemIconSize * inventoryWidth + slotBorder * (inventoryWidth + 4), itemIconSize * (items.Count / inventoryWidth) + slotBorder * ((items.Count / inventoryWidth) + 4) + Screen.height * 0.035f);
		//Calculate the size and position of the backpack image
		bagpackRect = new Rect(Screen.width * 0.95f, Screen.height * 0.925f, Screen.width * 0.04f, Screen.width * 0.04f);
		//Store the width of the screen
		screenWidth = Screen.width;
		//Store the height of the screen
		screenHeight = Screen.height;
	}
	
	void OnGUI() {
		
		bool hoverActionbar = false;
		
		foreach(ActionBars ab in actionbars) {
			if(ab.hoverActionBar)
				hoverActionbar = true;
		}
				
		hoverName = "";
		
		GUI.skin = mySkin;
		
		Event curEvent = Event.current;
		
		if(showInventory)
			DrawInventory(curEvent);
		
		if(curEvent.isMouse && (curEvent.type == EventType.MouseDown || curEvent.type == EventType.MouseUp) && draggedItem && !hoverActionbar) {
			dragging = false;
			print("Dropped: " + draggedItem.itemName);
			foreach(ActionBars ab in actionbars) {
				if(ab.left) {
					for(int i = 0; i < ab.leftActionbarSlots.Count; i++) {
						if(ab.leftActionbarSlots[i].item == draggedItem)
							ab.leftActionbarSlots[i].item = null;
					}
				}
				else if(ab.right) {
					for(int i = 0; i < ab.rightActionbarSlots.Count; i++) {
						if(ab.rightActionbarSlots[i].item == draggedItem)
							ab.rightActionbarSlots[i].item = null;
					}
				}
				else if(ab.bottom) {
					for(int i = 0; i < ab.bottomActionbarSlots.Count; i++) {
						if(ab.bottomActionbarSlots[i].item == draggedItem)
							ab.bottomActionbarSlots[i].item = null;
					}
				}
			}
			draggedItem = null;
		}
		
		GUI.depth = 0;
		
		//If we're dragging an item draw the icon of the item and if the stacksize is greater than zero draw a label containing the size of the stack
		if(draggedItem) {
			GUI.DrawTexture(new Rect(curEvent.mousePosition.x, curEvent.mousePosition.y, itemIconSize, itemIconSize), draggedItem.icon);
			if(draggedItem.stackSize > 0)
				GUI.Label(new Rect(curEvent.mousePosition.x, curEvent.mousePosition.y, itemIconSize, itemIconSize), draggedItem.stackSize.ToString());
		}
		//Draw the backpack image
		GUI.DrawTexture(bagpackRect, bagpackImage);
		//Draw a frame over the backpack image
		GUI.DrawTexture(bagpackRect, frame);
		
		//If the mouse is over the backpack image and the player presses down the mouse or the player presses the I key
		//Either show or hide the inventory
		if(bagpackRect.Contains(curEvent.mousePosition) && curEvent.type == EventType.MouseDown || (curEvent.keyCode == KeyCode.I && curEvent.type == EventType.KeyDown)) {
			showInventory = !showInventory;
		}
		//If the hovername isn't equal to nothing then draw a box containing the hovername
		if(hoverName != "")
			GUI.Box(new Rect(curEvent.mousePosition.x - mySkin.box.CalcSize(new GUIContent(hoverName)).x * 0.5f, curEvent.mousePosition.y - mySkin.box.CalcSize(new GUIContent(hoverName)).y , mySkin.box.CalcSize(new GUIContent(hoverName)).x, mySkin.box.CalcSize(new GUIContent(hoverName)).y),hoverName, "textArea");
	}
	
	void DrawInventory(Event curEvent) {
		
		//Draw a box as the background for the inventory
		GUI.Box(inventoryRect, "inventory");
		
		//Begin the area where the slots should be drawn
		GUILayout.BeginArea(new Rect(inventoryRect.xMin, inventoryRect.yMin + Screen.height * 0.035f, inventoryRect.width, inventoryRect.height));
		
		int j;
		int k;
		for(int i = 0; i < items.Count; i++) {
			Item item = items[i];
			j = i /	inventoryWidth;
			k = i % inventoryWidth;
			//Calculate the size and position of the current slot
			Rect currentRect = (new Rect(k * (itemIconSize + slotBorder) + slotBorder * 2.5f, j * (itemIconSize + slotBorder) + slotBorder * 2, itemIconSize, itemIconSize));
			
			//If there's nothing in the slot
			if(!item) {
				//Draw a box
				GUI.Box(currentRect, "");
				//If slot contains the mouse
				if(currentRect.Contains(curEvent.mousePosition)) {
					//If the player releases the mouse and an item is being dragged
					if(curEvent.type == EventType.MouseUp && dragging) {
						//If the inventory doesn't allready contains the item that's being dragged
						//Add the item to the inventory
						if(!items.Contains(draggedItem)) {
							items[i] = draggedItem;
							dragging = false;
							draggedItem = null;
						}
					}
				}
				//Draw a frame around the slot
				GUI.DrawTexture(currentRect, frame);
			}
			//Else if the item contains an item
			else {
				//Draw the icon of the item
				GUI.DrawTexture(currentRect, item.icon);
				//If the slot contains the position of the mouse
				if(currentRect.Contains(curEvent.mousePosition)) {
					hoverName = item.itemName;
					//If the player is dragging and we're not allready dragging
					//Set the item in the slot to be dragged
					if(curEvent.type == EventType.MouseDrag && !dragging) {
						draggedItem = item;
						dragging = true;
						items[i] = null;
						dragOrigin = i;
					}
					//If the player presses the right mouse button on the item and the item is not on cooldown then use the item
					else if(curEvent.type == EventType.MouseDown && curEvent.button == 1) {
						if(!item.onCooldown && item.itemType == EquipmentSlotType.consumable) {
							item.Use();
						}
					}
					//If the player releases the mouse and an item is being dragged replace the item in the slot with the one being dragged
					else if(curEvent.type == EventType.MouseUp && dragging) {
						items[dragOrigin] = item;
						items[i] = draggedItem;
						dragging = false;
					}
				}
				//Draw a frame around the item
				GUI.DrawTexture(currentRect, frame);
				
				//If the stacksize of the item in the slot is greater than zero draw a label containing the stacksize
				if(item.stackSize > 0) {
					GUI.Label(currentRect, item.stackSize.ToString());
				}
				//If the item is on cooldown
				if(item.onCooldown) {
					if(Event.current.type.Equals(EventType.Repaint)) {
						//Set the cutoff of the cooldown material to match the state of the cooldown
						cooldownMaterial.SetFloat("_Cutoff", item.cooldownTimer / item.cooldown);
						//Draw the cooldown texture over the item
		           		Graphics.DrawTexture(currentRect, cooldownTexture, cooldownMaterial);
		    		}
				}
			}
		}
		GUILayout.EndArea();
	}
}
