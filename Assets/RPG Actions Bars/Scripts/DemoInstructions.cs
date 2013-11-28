using UnityEngine;
using System.Collections;

public class DemoInstructions : MonoBehaviour {

	void OnGUI() {
		GUILayout.BeginArea(new Rect(Screen.width * 0.15f, Screen.height * 0.025f, Screen.width * 0.7f, Screen.height * 0.3f));
		GUILayout.Box("" +
			"The I key opens the inventory. The P key opens the spellbook. The escape closes the windows. \n" +
			"You can drag spells and items from the windows and drop them on the action bars. \n" +
			"Pressing shift and dragging the spells or items on the actionbars will drag the item or spell. \n" +
			"Right clicking on an item or spell will use it.");
		GUILayout.EndArea();
	}
}
