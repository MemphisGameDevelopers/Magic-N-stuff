#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

[CustomEditor (typeof(CombineChildrenAFS))]
public class CombineChildrenAFSEditor : Editor {
	
	
	private bool hTerrain = false;
	private bool sGrass = true;
	private bool sFoliage = true;
	

	public override void OnInspectorGUI () {
		CombineChildrenAFS script = (CombineChildrenAFS)target;
		
		Color myBlue = new Color(0.5f,0.7f,1.0f,1.0f);
		Color myCol = Color.green;
		
		GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
		myFoldoutStyle.fontStyle = FontStyle.Bold;
		
		EditorGUILayout.BeginVertical();
		GUILayout.Space(10);
		GUI.color = myCol;
		GUILayout.Label ("Ground normal sampling", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		script.GroundMaxDistance = EditorGUILayout.Slider("Max Ground Distance", script.GroundMaxDistance, 0.0f, 4.0f );
		GUILayout.Space(4);
		// underlaying terrain
		script.UnderlayingTerrain = (Terrain)EditorGUILayout.ObjectField("Underlaying Terrain", script.UnderlayingTerrain, typeof(Terrain), true);
		GUI.color = myBlue;
		hTerrain = EditorGUILayout.Foldout(hTerrain," Help");
		GUI.color = Color.white;
		if(hTerrain){
			EditorGUILayout.HelpBox("If you place the objects of the cluster (all or just a few of them) on top of a terrain you will have to assign the according terrain in order to make lighting fit 100% the terrain lighting.", MessageType.None, true);
		}
		
		
		
		// bake grass
		GUILayout.Space(9);
		EditorGUILayout.BeginVertical();
		GUI.color = myCol;
		//script.bakeGroundLigthingGrass = EditorGUILayout.Toggle("", script.bakeGroundLigthingGrass, GUILayout.Width(14) );
		sGrass = EditorGUILayout.Foldout(sGrass," Grass Shader Settings", myFoldoutStyle );
		GUI.color = Color.white;
		GUILayout.Space(4);
		
		if (sGrass) {
			script.HealthyColor = EditorGUILayout.ColorField("Healthy Color", script.HealthyColor);
			script.DryColor = EditorGUILayout.ColorField("Dry Color", script.DryColor);
			script.NoiseSpread = EditorGUILayout.Slider("Noise Spread", script.NoiseSpread, 0.0f, 1.0f );
			
			//script.bakeGroundLightingFoliage = false;
		}
		EditorGUILayout.EndVertical();
		
		// bake foliage
		GUILayout.Space(9);
		EditorGUILayout.BeginVertical();
		//script.bakeGroundLightingFoliage = EditorGUILayout.Toggle("", script.bakeGroundLightingFoliage, GUILayout.Width(14) );
		GUI.color = myCol;
		sFoliage = EditorGUILayout.Foldout(sFoliage," Foliage Shader Settings", myFoldoutStyle);
		GUI.color = Color.white;
		GUILayout.Space(4);
		if (sFoliage) {
			script.randomBrightness = EditorGUILayout.Slider("Random Brightness", script.randomBrightness, 0.0f, 1.0f );
			script.randomPulse = EditorGUILayout.Slider("Random Pulse", script.randomPulse, 0.0f, 1.0f );
			script.randomBending = EditorGUILayout.Slider("Random Bending", script.randomBending, 0.0f, 1.0f );
			script.randomFluttering = EditorGUILayout.Slider("Random Fluttering", script.randomFluttering, 0.0f, 1.0f );
			script.NoiseSpreadFoliage = EditorGUILayout.Slider("Noise Spread", script.NoiseSpreadFoliage, 0.0f, 1.0f );
			
			EditorGUILayout.BeginHorizontal();
			script.bakeScale = EditorGUILayout.Toggle("", script.bakeScale, GUILayout.Width(14) );
			GUILayout.Label ("Bake Scale");
			EditorGUILayout.EndHorizontal();
			
			//script.bakeGroundLightingGrass = false;
			//script.simplyCombine = false;
		}
		EditorGUILayout.EndVertical();
		
		// simply combine foliage
		/*
		GUILayout.Space(9);
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		script.simplyCombine = EditorGUILayout.Toggle("", script.simplyCombine, GUILayout.Width(14) );
		GUI.color = myCol;
		GUILayout.Label ("Simply combine Objects using the Foliage Shader");
		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(4);
		if (script.simplyCombine) {
			script.randomBrightness = EditorGUILayout.Slider("Random Brightness", script.randomBrightness, 0.0f, 1.0f );
			script.randomPulse = EditorGUILayout.Slider("Random Pulse", script.randomPulse, 0.0f, 1.0f );
			script.randomBending = EditorGUILayout.Slider("Random Bending", script.randomBending, 0.0f, 1.0f );
			script.randomFluttering = EditorGUILayout.Slider("Random Fluttering", script.randomFluttering, 0.0f, 1.0f );
			EditorGUILayout.BeginHorizontal();
			script.bakeScale = EditorGUILayout.Toggle("", script.bakeScale, GUILayout.Width(14) );
			GUILayout.Label ("Bake Scale");
			EditorGUILayout.EndHorizontal();
			
			//script.bakeGroundLightingGrass = false;
			//script.bakeGroundLightingFoliage = false;
			
		}
		EditorGUILayout.EndVertical();
		*/
		
		// overall settings
		GUILayout.Space(9);
		GUI.color = myCol;
		GUILayout.Label ("Overall Settings", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginHorizontal();
			script.CastShadows = EditorGUILayout.Toggle("", script.CastShadows, GUILayout.Width(14) );
			GUILayout.Label ("Cast Shadows" );
			EditorGUILayout.EndHorizontal();
		GUILayout.Space(4);
			EditorGUILayout.BeginHorizontal();
			script.destroyChildObjectsInPlaymode = EditorGUILayout.Toggle("", script.destroyChildObjectsInPlaymode, GUILayout.Width(14) );
			GUILayout.Label ("Destroy Children" );
			EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(4);
		//if (script.destroyChildObjectsInPlaymode) {
		//	EditorGUILayout.BeginHorizontal();
		//	GUILayout.Label ("", GUILayout.Width(16) );
		//	EditorGUILayout.HelpBox("If this option is checked, child objects will also be destroyed if you hit 'Combine statically'.", MessageType.Warning, true);	
		//	EditorGUILayout.EndHorizontal();
		//}
		
	
		
		
		// debugging settings
		GUILayout.Space(9);
		GUI.color = myCol;
		GUILayout.Label ("Debugging", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		EditorGUILayout.BeginHorizontal();
		script.debugNormals = EditorGUILayout.Toggle("", script.debugNormals, GUILayout.Width(14) );
		GUILayout.Label ("Debug sampled Ground Normals" );
		EditorGUILayout.EndHorizontal();
		if (script.debugNormals) {
			script.EnableDebugging();
		}
		else {
			script.DisableDebugging();
		}
		

		// functions
		GUILayout.Space(9);
		GUI.color = myCol;
		GUILayout.Label ("Functions", EditorStyles.boldLabel);
		GUI.color = Color.white;
		GUILayout.Space(4);
		script.RealignGroundMaxDistance = EditorGUILayout.Slider("Realign Ground max Dist.", script.RealignGroundMaxDistance, 0.0f, 10.0f );
		EditorGUILayout.BeginHorizontal();
			script.ForceRealignment = EditorGUILayout.Toggle("", script.ForceRealignment, GUILayout.Width(14) );
			GUILayout.Label ("Force Realignment" );
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(9);
		EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button( "Realign Objects") ) {
					script.Realign();
			}
			if (GUILayout.Button( "Combine statically") ) {
				if (script.destroyChildObjectsInPlaymode) {
					if ( EditorUtility.DisplayDialog("Combine statically?", "Are you sure you want to combine and destroy all child objects? If you want to be able to edit child objects please uncheck 'Destroy Children' first.", "Combine", "Do not Combine" ) ) {
						script.Combine();
					}
				}
				else {
					if ( EditorUtility.DisplayDialog("Combine statically?", "All child objects will be deactivated.", "Combine", "Do not Combine" ) ) {
						script.Combine();
					}
				}
			}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(9);
		EditorGUILayout.BeginHorizontal();
			script.isStaticallyCombined = EditorGUILayout.Toggle("", script.isStaticallyCombined, GUILayout.Width(14) );
			GUILayout.Label ("Has been statically combined" );
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(9);
		EditorGUILayout.EndVertical();
		
	}
	
	// if the editor looses focus
	void OnDisable() {
		
	}
	

		
}
#endif