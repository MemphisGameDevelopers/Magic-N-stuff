using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour {
	
	public GameObject healthGlobe;
	public GameObject manaGlobe;
	public GameObject bar;
	
	bool showBar;
	bool showGlobes;

	// Use this for initialization
	void Start () {
		showBar = true;
	}
	
	// Update is called once per frame
	void Update () {
		healthGlobe.SetActive(showGlobes);
		manaGlobe.SetActive(showGlobes);
		bar.SetActive(showBar);
	}
	
	void OnGUI() {
		if(showBar = GUI.Toggle(new Rect(Screen.width * 0.7f, Screen.height * 0.3f, Screen.width * 0.2f, Screen.height * 0.03f), showBar, "Toggle bar")) {
			showGlobes = !showBar;
		}
		if(showGlobes = GUI.Toggle(new Rect(Screen.width * 0.7f, Screen.height * 0.34f, Screen.width * 0.2f, Screen.height * 0.03f), showGlobes, "Toggle globes")) {
			showBar = !showGlobes;
		}
		
		if(showGlobes) {
			VitalGlobe mGlobe = manaGlobe.GetComponent<VitalGlobe>();
			VitalGlobe hGlobe = healthGlobe.GetComponent<VitalGlobe>();
			
			mGlobe.minMana = GUI.HorizontalSlider(new Rect(mGlobe.globeRect.x, mGlobe.globeRect.y - Screen.height * 0.05f, mGlobe.globeRect.width, Screen.height * 0.1f),Mathf.Round( mGlobe.minMana ), 0, mGlobe.maxMana);
			hGlobe.minHealth = GUI.HorizontalSlider(new Rect(hGlobe.globeRect.x, hGlobe.globeRect.y - Screen.height * 0.05f, hGlobe.globeRect.width, Screen.height * 0.1f),Mathf.Round( hGlobe.minHealth ), 0, hGlobe.maxHealth); 
		}
		else if(showBar) {
			VitalBar bars = bar.GetComponent<VitalBar>();
			
			bars.minHealth = GUI.HorizontalSlider(new Rect(Screen.width * 0.1f, Screen.height * 0.3f, Screen.width * 0.2f, Screen.height * 0.02f), Mathf.Round(bars.minHealth), 0, bars.maxHealth);
			bars.minMana = GUI.HorizontalSlider(new Rect(Screen.width * 0.1f, Screen.height * 0.33f, Screen.width * 0.2f, Screen.height * 0.02f), Mathf.Round(bars.minMana), 0, bars.maxMana);
		}
	}
}
