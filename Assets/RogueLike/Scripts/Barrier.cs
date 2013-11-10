using UnityEngine;
using System.Collections;

public class Barrier : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider obj)
    {
        
        if (obj.gameObject.name != "Player") return;
        Debug.Log("BONK!!");
        Destroy(gameObject);
        
    }
}
