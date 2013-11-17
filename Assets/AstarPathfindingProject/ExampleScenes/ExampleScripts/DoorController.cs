using UnityEngine;
using System.Collections;
using Pathfinding;

public class DoorController : MonoBehaviour {
	
	private bool open = false;
	
	public int opentag = 1;
	public int closedtag = 1;
	
	Bounds bounds;
	
	public void Start () {
		bounds = collider.bounds;
		SetState (open);
	}
	
	// Use this for initialization
	void OnGUI () {
		
		if (GUILayout.Button ("Toggle Door")) {
			SetState (!open);
		}
	}
	
	public void SetState (bool open) {
		this.open = open;
		
		GraphUpdateObject guo = new GraphUpdateObject(bounds);
#if ConfigureTagsAsMultiple
		guo.tags = new TagMask ();
		guo.tags.tagsChange = 1 << bitToChange;
		guo.tags.tagsSet = open ? 1 << bitToChange : 0;
#else
		int tag = open ? opentag : closedtag;
		if (tag > 31) { Debug.LogError ("tag > 31"); return; }
		guo.modifyTag = true;
		guo.setTag = tag;
#endif
		
		AstarPath.active.UpdateGraphs (guo);
		
		if (open) {
			animation.Play ("Open");
		} else {
			animation.Play ("Close");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
