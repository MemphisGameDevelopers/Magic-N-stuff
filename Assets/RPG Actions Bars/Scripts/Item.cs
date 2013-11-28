using UnityEngine;
using System.Collections;

public enum EquipmentSlotType {
	consumable
}

public class Item : MonoBehaviour {
	
	public EquipmentSlotType itemType;
	
	public Texture2D icon;
	
	public int healAmount;
	public int manaAmount;

	public string useEffect;
	
	public float cooldown;
	[HideInInspector]
	public float cooldownTimer;
	[HideInInspector]
	public bool onCooldown;
	
	public int stackSize;
	
	public string itemName;
	
	void Update () {
		if(cooldownTimer < cooldown)
			cooldownTimer += Time.deltaTime;
		if(cooldownTimer >= cooldown)
			onCooldown = false;
	}
	
	public void Use() {
		cooldownTimer = 0;
		onCooldown = true;
		
		//Here you could implement potions use effect or townportals? Use your imagination :)
		//An example could be:
		
		//Rejuvenation potion
		if(healAmount > 0 && manaAmount > 0) {
		//	Vitals.currentHealth += healAmount;
		//	Vitals.currentMana += manaAmount;
			print("Health increased by: " + healAmount + "and mana increased by: " + manaAmount);
		}
		//Healing potion
		else if(healAmount > 0) {
		//	Vitals.currentHealth += healAmount;
			print("Health increased by: " + healAmount);
		}
		//Mana potion
		else if(manaAmount > 0) {
		//	Vitals.currentMana += manaAmount;
			print("Mana increased by: " + manaAmount);
		}
		//Maybe even through in an drinking sound or a particle effect
		
		stackSize--;
		if(stackSize == 0)
			Destroy(gameObject);
	}
}
