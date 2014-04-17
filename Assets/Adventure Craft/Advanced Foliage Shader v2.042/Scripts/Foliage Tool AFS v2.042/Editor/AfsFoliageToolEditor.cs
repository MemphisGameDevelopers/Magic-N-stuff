#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

[CustomEditor (typeof(AfsFoliageTool))]
public class AfsFoliageToolEditor : Editor {
	
	
	//private bool hScale = false;
	private bool hVertexColors = false;
	private bool hSetBending = false;
	
	public Color leColor;
	public string toolTip;



	
	public override void OnInspectorGUI () {
		AfsFoliageTool script = (AfsFoliageTool)target;
		Color myBlue = new Color(0.5f,0.7f,1.0f,1.0f);
		Color myCol = Color.green;
		
		//EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.BeginVertical();
		GUILayout.Space(10);
		GUI.color = myCol;
		GUILayout.Label ("Rework your plants and save them as new mesh","BoldLabel");
		GUI.color = Color.white;

		//script.adjustScale = EditorGUILayout.Toggle("Adjust Scale", script.adjustScale);
		//GUI.color = myBlue;
		//hScale = EditorGUILayout.Foldout(hScale," Help");
		//GUI.color = Color.white;
		//if(hScale){
		//	EditorGUILayout.HelpBox("Smaller plants should react differently to touch bending than larger ones. " +
		//	"But the shader does not know about the size of each model." +
		//	"So baking the model's size into the mesh will give all needed information to the shader to handle each model differently." +
		//	"\n1 = small plant leading to less bending amplitudes whereas 3 = big plant on which the full bending will be applied.", MessageType.None, true);
		//}
		//if (script.adjustScale) {
		//	script.Scale = EditorGUILayout.IntSlider("Size", (int) script.Scale, 1, 3);
		//}
		
		//GUILayout.Space(5);
		//GUILayout.Label (script.oldFileName,"BoldLabel");
		
		
		if (!script.hasBeenSaved) {
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("You have to safe a new mesh before you can start reworking it. ", MessageType.Warning, true);
		}
		
		
		
		else {
/////////
		
		script.checkSubmeshes();
		if (script.hasSubmeshes) {
				GUILayout.Space(10);
				EditorGUILayout.HelpBox("The assigned Mesh has more than one Submesh.\n"+
				"You should start by merging the submeshes first.\nSo check 'Merge Submeshes', then hit 'Save Mesh'.", MessageType.Warning, true);


				EditorGUILayout.BeginHorizontal();
				script.mergeSubMeshes = EditorGUILayout.Toggle("", script.mergeSubMeshes, GUILayout.Width(14) );
				GUI.color = myCol;
				GUILayout.Label ("Merge Submeshes");
				GUI.color = Color.white;

				if (!script.has2ndUV) {
					GUI.enabled = false;
				}
				script.delete2ndUV = EditorGUILayout.Toggle("", script.delete2ndUV, GUILayout.Width(14) );
				GUILayout.Label ("Delete 2nd UV Set");
				if (!script.has2ndUV) {
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				script.showTextureCombine = true;

		}

		if (!script.mergeSubMeshes) {

			GUILayout.Space(10);
			
			EditorGUILayout.BeginHorizontal();
			script.adjustBending = EditorGUILayout.Toggle("", script.adjustBending, GUILayout.Width(14) );
			GUI.color = myCol;
			GUILayout.Label ("Set Bending");
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();

			GUI.color = myBlue;
			hSetBending = EditorGUILayout.Foldout(hSetBending," Help");
			GUI.color = Color.white;
			if(hSetBending){
				EditorGUILayout.HelpBox("Set up vertex colors for primary and secondary bending from scratch. "+ 
				"Using this function will overwrite all vertex color blue values originally baked into the mesh.", MessageType.None, true);
			}
			
			if (script.adjustBending) {
				GUILayout.Space(5);
				script.adjustVertexColors = false;
				
				EditorGUILayout.BeginHorizontal();
				script.maxBendingValueY = EditorGUILayout.Slider("Along Y-axis", script.maxBendingValueY, 0.0f, 1.0f);
				if (script.curvY.length < 2) {
						script.curvY.AddKey(0.0f,0.0f);
						script.curvY.AddKey(1.0f,1.0f);
					}
				script.curvY = EditorGUILayout.CurveField("", script.curvY, GUILayout.Width(40));
				EditorGUILayout.EndHorizontal();
				
				
					
				EditorGUILayout.BeginHorizontal();
				script.maxBendingValueX = EditorGUILayout.Slider("Along XZ-axis", script.maxBendingValueX, 0.0f, 1.0f);
				if (script.curvX.length < 2) {
					script.curvX.AddKey(0.0f,0.0f);
					script.curvX.AddKey(1.0f,1.0f);
				}
				script.curvX = EditorGUILayout.CurveField("", script.curvX, GUILayout.Width(40));
				EditorGUILayout.EndHorizontal();
				//curveZ = EditorGUILayout.CurveField("Animation on Z", curveZ);
				
				
				
				/*
				EditorGUILayout.BeginHorizontal();
					GUILayout.Label ("Along X and Z axis", GUILayout.MinWidth(144));
					EditorGUILayout.BeginVertical();
						script.maxBendingValueX = EditorGUILayout.Slider("", script.maxBendingValueX, 0.0f, 1.0f);
						script.curvX = EditorGUILayout.CurveField("", script.curvX);
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				*/
				
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();

					if(! EditorApplication.isPlaying ) {
						if (GUILayout.Button("Apply" )) {
							script.AdjustBending();
						}
						if (GUILayout.Button("Test", GUILayout.Width(94) )) {
							EditorApplication.isPlaying = true;
						}
					}
					else {
						GUI.enabled = false;
						GUILayout.Button("Apply");
						GUI.enabled = true;
						if (GUILayout.Button("Stop", GUILayout.Width(94) )) {
							EditorApplication.isPlaying = false;
						}
					}
					
				 EditorGUILayout.EndHorizontal();
				
			}
			GUILayout.Space(10);
			
	////////////		
			
			EditorGUILayout.BeginHorizontal();
				script.adjustVertexColors = EditorGUILayout.Toggle("", script.adjustVertexColors, GUILayout.Width(14) );
			GUI.color = myCol;
			GUILayout.Label ("Adjust Vertex Colors");
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();

			GUI.color = myBlue;
			hVertexColors = EditorGUILayout.Foldout(hVertexColors," Help");
			GUI.color = Color.white;
			if(hVertexColors){
				EditorGUILayout.HelpBox("This option let's you change the vertex colors applied to the original mesh in order to adjust the overall bending. " +
				"All changes are done only relatively to the original values.", MessageType.None, true);
			}
			if (script.adjustVertexColors) {
				GUILayout.Space(5);
				
				script.adjustBending = false;
				
				script.adjustBlueValue = EditorGUILayout.Slider("Main Bending (Blue/%)", script.adjustBlueValue, -95.0f, 95.0f);
				script.adjustGreenValue = EditorGUILayout.Slider("Edge Flutter (Green/%)", script.adjustGreenValue, -95.0f, 95.0f);
				script.adjustRedValue = EditorGUILayout.Slider("Phase (Red/%)", script.adjustRedValue, -95.0f, 95.0f);
				
				//GUILayout.Space(5);
				//script.RedcurvY = EditorGUI.CurveField(new Rect(0,0,10,10),"sasasas", script.RedcurvY );
				//GUILayout.Space(5);
				
				script.adjustAlphaValue = EditorGUILayout.Slider("AO (Alpha/%)", script.adjustAlphaValue, -95.0f, 95.0f);
					
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();

					if(! EditorApplication.isPlaying ) {
						if (GUILayout.Button("Apply" )) {
							script.AdjustVertexColors();
						}
						if (GUILayout.Button("Test", GUILayout.Width(94) )) {
							EditorApplication.isPlaying = true;
						}
					}
					else {
						GUI.enabled = false;
						GUILayout.Button("Apply");
						GUI.enabled = true;
						if (GUILayout.Button("Stop", GUILayout.Width(94) )) {
							EditorApplication.isPlaying = false;
						}
					}
					
				 EditorGUILayout.EndHorizontal();
			}
		}
			
			
		
			
			
			
/////
			
		}
		
		GUILayout.Space(10);
		if(! EditorApplication.isPlaying ) {
			if (GUILayout.Button("Save Mesh", GUILayout.Height(34) )) {
				script.SaveNewPlantMesh();
			}
		}
		else {
			GUI.enabled = false;
			GUILayout.Button("Save Mesh", GUILayout.Height(34) );
			GUI.enabled = true;
		}

/////
		GUI.color = myCol;
		GUILayout.Space(10);
		script.showTextureCombine = EditorGUILayout.Foldout(script.showTextureCombine," Merge Tree Creator Textures");
		GUI.color = Color.white;
		if(script.showTextureCombine || script.hasSubmeshes ){
			
			//
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.BeginVertical();
			GUILayout.Label ("normal_specular");
			script.sourceTex0 = (Texture2D)EditorGUILayout.ObjectField(script.sourceTex0, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical();
			GUILayout.Label ("translucency_gloss");
			script.sourceTex1 = (Texture2D)EditorGUILayout.ObjectField(script.sourceTex1, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(4);
			//
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
			toolTip = "If your Texture are split into halfs (left half: bark / right half: leafs) check this and the script will automatically fill the left half of the translucency map with black. Otherwise you will have to edit the generated texture manually.";  
			EditorGUILayout.LabelField(new GUIContent("Texture Split", toolTip), GUILayout.Width(80));
			script.TextureSplitSelected = (TextureSplit)EditorGUILayout.EnumPopup(script.TextureSplitSelected);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			GUILayout.Space(4);
			if (GUILayout.Button("Save Combined Texture", GUILayout.Height(34) )) {
				mergeTexture();
			}
			
		}


		
		EditorGUILayout.EndVertical();
		
		EditorUtility.SetDirty( script );
		
	}
	
	
	
	// if the editor looses focus
	void OnDisable() {
	}

	public void mergeTexture () {
		AfsFoliageTool script = (AfsFoliageTool)target;

		if(script.sourceTex0 != null && script.sourceTex1 != null) {
			// check textures
			bool wasReadable0 = false;
			TextureImporterFormat format0;
			TextureImporterType type0;
			bool wasReadable1 = false;
			TextureImporterFormat format1;
			TextureImporterType type1;
			// check texture0
			string path0 = AssetDatabase.GetAssetPath(script.sourceTex0);
			TextureImporter ti0 = (TextureImporter) TextureImporter.GetAtPath(path0);
			format0 = ti0.textureFormat;
			type0 = ti0.textureType;
			if (ti0.isReadable == true) {
				wasReadable0 = true;
			}
			else {
				ti0.isReadable = true;
			}
			if (ti0.textureFormat != TextureImporterFormat.AutomaticTruecolor || ti0.textureType != TextureImporterType.Image ) {
				ti0.textureType = TextureImporterType.Image; 
				ti0.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				// refresh texture
				AssetDatabase.ImportAsset( path0, ImportAssetOptions.ForceUpdate );
			}
			
			// check texture1
			string path1 = AssetDatabase.GetAssetPath(script.sourceTex1);
			TextureImporter ti1 = (TextureImporter) TextureImporter.GetAtPath(path1);
			format1 = ti1.textureFormat;
			type1 = ti1.textureType;
			if (ti1.isReadable == true) {
				wasReadable1 = true;
			}
			else {
				ti1.isReadable = true;
			}
			if (ti1.textureFormat != TextureImporterFormat.AutomaticTruecolor || ti1.textureType != TextureImporterType.Image ) {
				ti1.textureType = TextureImporterType.Image; 
				ti1.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				// refresh texture
				AssetDatabase.ImportAsset( path1, ImportAssetOptions.ForceUpdate ); 
			}
			///
			// check dimensions
			if (script.sourceTex0.width == script.sourceTex1.width)
			{
				// start combining
				Texture2D combinedTex = new Texture2D( script.sourceTex0.width, script.sourceTex0.height, TextureFormat.ARGB32, true);
				Color combinedColor;
				for (int y = 0; y < script.sourceTex0.height; y++) {
					for (int x = 0; x < script.sourceTex0.width; x++) {

						//if (x < script.sourceTex0.width * 0.5 && script.textureSplitHalf) {
						if (x < script.sourceTex0.width * 0.5 && script.TextureSplitSelected == TextureSplit.OneByOne) {
							combinedColor.r = 0;
						}
						else if (x < script.sourceTex0.width * 0.666 && script.TextureSplitSelected == TextureSplit.TwoByOne) {
							combinedColor.r = 0;
						}
						else {
							combinedColor.r = script.sourceTex1.GetPixel(x,y).b; // r = trans
						}
						combinedColor.g = script.sourceTex0.GetPixel(x,y).g; // g = g from normal
						combinedColor.b = script.sourceTex1.GetPixel(x,y).a; // b = gloss
						combinedColor.a = script.sourceTex0.GetPixel(x,y).r; // a = r from normal
						combinedTex.SetPixel(x,y,combinedColor);
					}
				}
				// save texture
				string filePath = EditorUtility.SaveFilePanelInProject
					(
						"Save Combined Normal/Trans/Spec Texture",
						"combinedNormaTransSpec.png", 
						"png",
						"Choose a file location and name"
						);
				if (filePath!=""){
					var bytes = combinedTex.EncodeToPNG(); 
					File.WriteAllBytes(filePath, bytes); 
					
					AssetDatabase.Refresh();
					TextureImporter ti2 = AssetImporter.GetAtPath(filePath) as TextureImporter;
					ti2.anisoLevel = 7;
					ti2.textureType = TextureImporterType.Advanced;
					ti2.textureFormat = TextureImporterFormat.ARGB32;
					AssetDatabase.ImportAsset(filePath);
					AssetDatabase.Refresh();
					
					
				}
				DestroyImmediate(combinedTex, true);
			}
			else {
				Debug.Log ("Both Textures have to fit in size.");
			}
			// reset texture settings
			ti0.textureFormat = format0;
			ti0.textureType = type0;
			ti1.textureFormat = format1;
			ti1.textureType = type1;
			if (wasReadable0 == false) {
				ti0.isReadable = false;
			}
			if (wasReadable1 == false) {
				ti1.isReadable = false;
			}
			AssetDatabase.ImportAsset( path0, ImportAssetOptions.ForceUpdate );
			AssetDatabase.ImportAsset( path1, ImportAssetOptions.ForceUpdate );
			Resources.UnloadUnusedAssets();
		}
		
		
	}
		
}
#endif