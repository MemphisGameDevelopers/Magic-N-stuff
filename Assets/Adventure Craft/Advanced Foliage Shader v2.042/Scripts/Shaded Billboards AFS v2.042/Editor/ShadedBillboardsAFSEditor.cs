#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

[CustomEditor (typeof(ShadedBillboardsAFS))]
public class ShadedBillboardsAFSEditor : Editor {
	
	private int treesPerFrame;
	private float changeStateSpeed;
	public GameObject newShadowCaster;

	public override void OnInspectorGUI () {
		ShadedBillboardsAFS script = (ShadedBillboardsAFS)target;
		
		//Color myBlue = new Color(0.5f,0.7f,1.0f,1.0f);
		Color myCol = Color.green;

// Assigned light source
		GUILayout.Space(10);
		GUI.color = myCol;
		GUILayout.Label ("Assigned light source", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		
		script.lightRef = (GameObject)EditorGUILayout.ObjectField("Light Reference", script.lightRef, typeof(GameObject), true);
		
//	Assigned shadow casters		
		GUILayout.Space(10);
		GUI.color = myCol;
		GUILayout.Label ("Assigned shadow casters", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		
		for(int i = 0; i < script.shadowCasters.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			script.shadowCasters[i] = (GameObject)EditorGUILayout.ObjectField("Shadow Caster " + (i+1), script.shadowCasters[i], typeof(GameObject), true);
			if (GUILayout.Button("Remove", GUILayout.Width(60) )) {
				script.shadowCasters.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}
		
		GUILayout.Space(9);
		EditorGUILayout.BeginHorizontal();
		newShadowCaster = (GameObject)EditorGUILayout.ObjectField("Add Shadow Caster", newShadowCaster, typeof(GameObject), true);
		if (GUILayout.Button("Add", GUILayout.Width(60) )) {
			if (newShadowCaster) {
				script.shadowCasters.Add(newShadowCaster);
				newShadowCaster = null;
			}
		}
		EditorGUILayout.EndHorizontal();

//	Processing and rendering settings		
		GUILayout.Space(10);
		GUI.color = myCol;
		GUILayout.Label ("Processing and rendering settings", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		
		script.treesPerFrame = EditorGUILayout.IntField("Trees per Frame", script.treesPerFrame);
		script.changeStateSpeed = EditorGUILayout.Slider("Change Shading Speed", script.changeStateSpeed, 0.0f, 10.0f );
		
		script.treeYOffset = EditorGUILayout.Slider("Sample Y-Offset", script.treeYOffset, 0.0f, 10.0f );

		
		EditorGUILayout.BeginHorizontal();
			script.shadeOnlyWithinShadowRange = EditorGUILayout.Toggle("", script.shadeOnlyWithinShadowRange, GUILayout.Width(14) );
			GUILayout.Label ("Shade Billboards only within real time shadow distance");
		EditorGUILayout.EndHorizontal();

//	Debugging
		GUILayout.Space(10);
		GUI.color = myCol;
		GUILayout.Label ("Debugging", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		EditorGUILayout.BeginHorizontal();
			script.showRays = EditorGUILayout.Toggle("", script.showRays, GUILayout.Width(14) );
			GUILayout.Label ("Show Rays");
		EditorGUILayout.EndHorizontal();

//	Update Trees
		GUILayout.Space(10);
		if (GUILayout.Button("Update Trees", GUILayout.Height(34) )) {
			script.UpdateTrees();
			//store and adjust settings
			treesPerFrame = script.treesPerFrame;
			script.treesPerFrame = 100000;
			changeStateSpeed = script.changeStateSpeed;
			script.changeStateSpeed = 100;
			script.curIdx = 0;
			script.ShadeTrees();
			//restore settings
			script.treesPerFrame = treesPerFrame;
			script.changeStateSpeed = changeStateSpeed;
		}
		
		GUILayout.Space(5);
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Reset raycasted shadows" )) {
			script.RemoveShadowing();
		}
		if (GUILayout.Button("Reset lightmapped shadows" )) {
			script.RemoveLightMap();
		}
		EditorGUILayout.EndHorizontal();		
	}
	
	// if the editor looses focus
	void OnDisable() {		
	}
		
}
#endif