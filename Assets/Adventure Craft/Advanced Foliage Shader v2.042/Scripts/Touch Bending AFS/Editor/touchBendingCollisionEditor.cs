#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

[CustomEditor (typeof(touchBendingCollision))]
public class touchBendingCollisionEditor : Editor {

	public override void OnInspectorGUI () {
		touchBendingCollision script = (touchBendingCollision)target;
		
		EditorGUILayout.BeginVertical("Box");
		
		script.simpleBendingMaterial = (Material)EditorGUILayout.ObjectField("Regular material", script.simpleBendingMaterial, typeof(Material), false);
		script.touchBendingMaterial = (Material)EditorGUILayout.ObjectField("Touch bending material", script.touchBendingMaterial, typeof(Material), false);
		GUILayout.Space(5);
		if (GUILayout.Button("Sync Touch bending Material")) {
			syncTouchBendingMaterial();
		}
		
		GUILayout.Space(10);
		script.stiffness = EditorGUILayout.Slider("Bendability", script.stiffness, 0.01f, 50.0f);
		script.disturbance = EditorGUILayout.Slider("Disturbance", script.disturbance, 0.01f, 10.0f); 
		script.duration = EditorGUILayout.Slider("Duration", script.duration, 0.1f, 20.0f); 
		
		EditorGUILayout.EndVertical();
		EditorUtility.SetDirty(script);
	}
	
	public void syncTouchBendingMaterial() {
		touchBendingCollision script = (touchBendingCollision)target;
		script.touchBendingMaterial.SetTexture("_MainTex", script.simpleBendingMaterial.GetTexture ("_MainTex") );
		if (script.simpleBendingMaterial.GetTexture ("_BumpTransSpecMap")){
			script.touchBendingMaterial.SetTexture("_BumpTransSpecMap", script.simpleBendingMaterial.GetTexture ("_BumpTransSpecMap") );	
		}
		
		script.touchBendingMaterial.SetFloat("_Cutoff", script.simpleBendingMaterial.GetFloat ("_Cutoff") );
		script.touchBendingMaterial.SetFloat("_Shininess", script.simpleBendingMaterial.GetFloat ("_Shininess") );
		script.touchBendingMaterial.SetColor("_TranslucencyColor", script.simpleBendingMaterial.GetColor ("_TranslucencyColor") );
		script.touchBendingMaterial.SetFloat("_TranslucencyViewDependency", script.simpleBendingMaterial.GetFloat ("_TranslucencyViewDependency") );
		script.touchBendingMaterial.SetFloat("_ShadowStrength", script.simpleBendingMaterial.GetFloat ("_ShadowStrength") );
		script.touchBendingMaterial.SetFloat("_ShadowOffsetScale", script.simpleBendingMaterial.GetFloat ("_ShadowOffsetScale") );
		
		EditorUtility.SetDirty(script);
	}
}
#endif
