#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor (typeof(SetupAdvancedFoliageShader))]
public class SetupAdvancedFoliageShaderEditor : Editor {

	private bool fadeLenghtReseted = false;
	private Terrain[] allTerrains;

	private string toolTip;
	
	public override void OnInspectorGUI () {
		SetupAdvancedFoliageShader script = (SetupAdvancedFoliageShader)target;
		//Color myBlue = new Color(0.5f,0.7f,1.0f,1.0f);
		Color myCol = Color.green;
		GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
		myFoldoutStyle.fontStyle = FontStyle.Bold;

		GUILayout.Space(6);

	//	Wind settings
		GUILayout.Space(4);
		EditorGUILayout.BeginVertical("Box");
		GUI.color = myCol;
		script.FoldWind = EditorGUILayout.Foldout(script.FoldWind," Wind Settings", myFoldoutStyle );
		GUI.color = Color.white;
		if (script.FoldWind) {
			GUILayout.Space(2);
			GUI.color = myCol;
			EditorGUILayout.LabelField("Overall Wind Settings");
			GUI.color = Color.white;
			GUILayout.Label ("Wind Direction (xyz) Strength (w)");
			GUILayout.Space(-14);
			script.Wind = EditorGUILayout.Vector4Field("", script.Wind);
			GUILayout.Space(2);
			toolTip = "Frequency the wind changes over time. Effects grass and foliage.";
			script.WindFrequency = EditorGUILayout.Slider(
				new GUIContent("Wind Frequency", toolTip), script.WindFrequency, 0.0f, 10.0f);

			GUILayout.Space(4);
			GUI.color = myCol;
			GUILayout.Label ("Wind Settings for the Foliage Shaders");
			GUI.color = Color.white;
			toolTip = "The shader adds some variation to the bending taking the vertex position in world space and the 'Wave Size Foliage' "+
				"parameter into account. So smaller wave sizes will add more variety to a given area but also lead to slightly different amounts of bending on each vertex even of a single mesh. This might cause some strange distortion of your models – especially large models. " +
				"For this reason you should set the 'Wave Size' to at least 2 or even 3 times the bounding box size of the largest model.";
			script.WaveSizeFoliageShader = EditorGUILayout.Slider(new GUIContent("Wave Size Foliage", toolTip), script.WaveSizeFoliageShader, 0.0f, 50.0f);
			GUILayout.Space(4);
			GUI.color = myCol;
			GUILayout.Label ("Wind Settings for the Grass Shaders");
			GUI.color = Color.white;
			toolTip = "Factor to make the bending of the grass fit the bending of the foliage. " +
				"Effects both: grass placed manually and grass placed within the terrain engine (if you use the 'atsVxx.WavingGrass vertexAlpha' shader).";
			script.WindMultiplierForGrassshader = EditorGUILayout.Slider(new GUIContent("Wind Multiplier Grass", toolTip), script.WindMultiplierForGrassshader, 0.0f, 5.0f);
			toolTip = "Similar to the 'Wave Size Foliage' parameter, but as grass models usually are pretty small even low values might look good. "+
				"It effects both: grass placed manually and grass placed within the terrain engine (if you use the 'atsVxx.WavingGrass vertexAlpha' shader).";
			script.WaveSizeForGrassshader = EditorGUILayout.Slider(new GUIContent("Wave Size Grass", toolTip), script.WaveSizeForGrassshader, 0.0f, 50.0f);
		}
		EditorGUILayout.EndVertical();

	//	Rain Settings
		GUILayout.Space(4);
		EditorGUILayout.BeginVertical("Box");
		GUI.color = myCol;
		script.FoldRain = EditorGUILayout.Foldout(script.FoldRain," Rain Settings", myFoldoutStyle );
		GUI.color = Color.white;
		if (script.FoldRain) {
			script.RainAmount = EditorGUILayout.Slider("Rain Amount", script.RainAmount, 0.0f, 1.0f);
			script.SpecPower = EditorGUILayout.Slider("Specular Power", script.SpecPower, 0.0f, 8.0f);
		}
		EditorGUILayout.EndVertical();
		
	//	Terrain Detail Vegetation Settings
		GUILayout.Space(4);
		EditorGUILayout.BeginVertical("Box");
		GUI.color = myCol;
		script.FoldVegTerrain = EditorGUILayout.Foldout(script.FoldVegTerrain," Terrain Detail Vegetation Settings", myFoldoutStyle );
		GUI.color = Color.white;
		if (script.FoldVegTerrain) {
			GUILayout.Space(2);
			script.VertexLitAlphaCutOff = EditorGUILayout.Slider("Alpha Cut Off", script.VertexLitAlphaCutOff, 0.1f, 1.0f);
			script.VertexLitTranslucencyColor = EditorGUILayout.ColorField ("Translucency Color", script.VertexLitTranslucencyColor);
			script.VertexLitTranslucencyViewDependency = EditorGUILayout.Slider("Translucency View Dependency", script.VertexLitTranslucencyViewDependency, 0.1f, 1.0f);
			script.VertexLitShadowStrength = EditorGUILayout.Slider("Shadow Strength", script.VertexLitShadowStrength, 0.1f, 1.0f);
			script.VertexLitShininess = EditorGUILayout.Slider("Shininess", script.VertexLitShininess, 0.03f, 1.0f);
		}
		EditorGUILayout.EndVertical();

	//	Grass, Tree and Billboard settings
		GUILayout.Space(4);
		EditorGUILayout.BeginVertical("Box");
		GUI.color = myCol;
		script.FoldBillboard = EditorGUILayout.Foldout(script.FoldBillboard," Grass, Tree and Billboard Settings", myFoldoutStyle );
		GUI.color = Color.white;
		if (script.FoldBillboard) {
			GUILayout.Space(2);
			// General Billboard settings
			GUI.color = myCol;
			EditorGUILayout.LabelField("Sync Settings to Terrain");
			GUI.color = Color.white;
			
			EditorGUILayout.BeginHorizontal();
			script.AutoSyncToTerrain = EditorGUILayout.Toggle("", script.AutoSyncToTerrain, GUILayout.Width(14) );
			EditorGUILayout.LabelField("Automatically sync with Terrain");
			EditorGUILayout.EndHorizontal();
			
			if(script.AutoSyncToTerrain) {
				script.SyncedTerrain = (Terrain)EditorGUILayout.ObjectField("Specify Terrain", script.SyncedTerrain, typeof(Terrain), true);
				if(script.SyncedTerrain != null){
					GUI.enabled = false;
				}
				else {
					EditorGUILayout.HelpBox("Please assign a terrain.", MessageType.Warning, true);
				}
			}
			GUILayout.Space(4);
			script.BillboardStart = EditorGUILayout.Slider("Billboard Start", script.BillboardStart, 0.0f, 1000.0f);
			if(script.TreeBillboardShadows){
				GUI.enabled = true;
			}
			script.BillboardFadeLenght = EditorGUILayout.Slider("Fade Length", script.BillboardFadeLenght, 0.0f, 30.0f);
			
			if(script.AutoSyncToTerrain) {
				GUI.enabled = false;
			}
			script.DetailDistanceForGrassShader = EditorGUILayout.Slider("Grass Fadeout Distance", script.DetailDistanceForGrassShader, 0.0f, 400.0f);
			script.GrassWavingTint = EditorGUILayout.ColorField ("Grass Waving Tint", script.GrassWavingTint);
			if(script.AutoSyncToTerrain) {
				GUI.enabled = true;
			}
			// Grass Render settings
			GUILayout.Space(4);
			GUI.color = myCol;
			EditorGUILayout.LabelField("Grass Render Settings");
			GUI.color = Color.white;
			EditorGUILayout.BeginHorizontal();
			script.GrassAnimateNormal = EditorGUILayout.Toggle("", script.GrassAnimateNormal, GUILayout.Width(14) );
			EditorGUILayout.LabelField("Enable Normal Animation on Grass");
			EditorGUILayout.EndHorizontal();
			
			
			// Tree and Billboard Render settings
			GUILayout.Space(4);
			GUI.color = myCol;
			EditorGUILayout.LabelField("Tree and Billboard Render Settings");
			GUI.color = Color.white;
			EditorGUILayout.BeginHorizontal();
			script.TreeShadowDissolve = EditorGUILayout.Toggle("", script.TreeShadowDissolve, GUILayout.Width(14) );
			EditorGUILayout.LabelField("Enable Dissolve Shadows for Trees");
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(4);
			if (script.TreeShadowDissolve) {
				script.TreeBillboardShadows = false;
			}
			
			EditorGUILayout.BeginHorizontal();
			script.TreeBillboardShadows = EditorGUILayout.Toggle("", script.TreeBillboardShadows, GUILayout.Width(14) );
			EditorGUILayout.LabelField("Enable Shadows casted by Billboards");
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(4);
			
			#if UNITY_3_5
			EditorGUILayout.HelpBox("Unfortunately 'Enable/disable shadows casted by Billboards' does not work automatically in Unity 3.x.\nSee docs for further instructions.", MessageType.Warning, true);
			#endif

			/*
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.Width(14));
			script.AfsTreeBillboardShadowShader = (Shader)EditorGUILayout.ObjectField("Billboard Shader", script.AfsTreeBillboardShadowShader, typeof(Shader), true);
			EditorGUILayout.EndHorizontal();
			*/
			
			if (script.TreeBillboardShadows) {
				script.TreeShadowDissolve = false;

				EditorGUILayout.BeginVertical();
				//EditorGUI.indentLevel++;
				//




				//AfsTreeBillboardShadowShader


				//
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(14));
				toolTip = "Check this to fade out shadows casted by billboards towards the edges of the screen.";
				script.BillboardShadowEdgeFade = EditorGUILayout.Toggle("", script.BillboardShadowEdgeFade, GUILayout.Width(14) );
				EditorGUILayout.LabelField(new GUIContent("Enable Shadow edgefade on Billboards", toolTip));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(32));
				script.BillboardShadowEdgeFadeThreshold = EditorGUILayout.Slider("Threshold", script.BillboardShadowEdgeFadeThreshold, 0.01f, 0.5f);
				EditorGUILayout.EndHorizontal();
				//
				//EditorGUILayout.BeginHorizontal();
				//EditorGUILayout.LabelField("", GUILayout.Width(14));
				//script.TreeShadowEdgeFade = EditorGUILayout.Toggle("", script.TreeShadowEdgeFade, GUILayout.Width(14) );
				//EditorGUILayout.LabelField("Enable Shadow edgefade on Mesh Trees");
				//EditorGUILayout.EndHorizontal();
				//
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(14));
				EditorGUILayout.HelpBox("Please make sure that all trees use the right shaders and set up 'Fade Length' in the option group 'Automatically sync with Terrain' above.", MessageType.Warning, true);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);
				if(script.BillboardLightReference == null) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(14));
					script.BillboardLightReference = (GameObject)EditorGUILayout.ObjectField("Light Reference", script.BillboardLightReference, typeof(GameObject), true);
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("", GUILayout.Width(14));
					EditorGUILayout.HelpBox("You have to specify a light reference.", MessageType.Error, true);
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(4);
				}
				
				//
				if (!fadeLenghtReseted) {
					ResetBillboardFadeLength();
					fadeLenghtReseted = true;
				}
				//EditorGUI.indentLevel--;

				EditorGUILayout.EndVertical();

			}
			
			else {
				if (fadeLenghtReseted) {
					RestoreBillboardFadeLength(script.BillboardFadeLenght);
					fadeLenghtReseted = false;
				}
			}
			
			script.BillboardFadeOutLength = EditorGUILayout.Slider("Billboard Fade Out Length", script.BillboardFadeOutLength, 10.0f, 100.0f);
			EditorGUILayout.BeginHorizontal();
			script.BillboardAdjustToCamera = EditorGUILayout.Toggle("", script.BillboardAdjustToCamera, GUILayout.Width(14) );
			EditorGUILayout.LabelField("Align Billboards to Camera");
			EditorGUILayout.EndHorizontal();
			if (script.BillboardAdjustToCamera) {
				script.BillboardAngleLimit = EditorGUILayout.Slider("Angle Limit", script.BillboardAngleLimit, 10.0f, 90.0f);
			}
			
			// Billboard RenderTex settings
			GUILayout.Space(4);
			GUI.color = myCol;
			EditorGUILayout.LabelField("Billboard Texture Settings");
			GUI.color = Color.white;
			script.BillboardTranslucencyFactor = EditorGUILayout.Slider("Translucency Factor", script.BillboardTranslucencyFactor, 1.0f, 3.0f);
			script.BillboardLeafsLightingFactor = EditorGUILayout.Slider("Leaf Lighting Factor", script.BillboardLeafsLightingFactor, 1.0f, 3.0f);
			script.BillboardLeafsContrast = EditorGUILayout.Slider("Leaf Contrast", script.BillboardLeafsContrast, .5f, 2.0f);
			//
			GUILayout.Space(4);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("");
			if ( GUILayout.Button( "Reset", GUILayout.MaxWidth(50) ) ) {
				script.BillboardTranslucencyFactor = 2.0f;
				script.BillboardLeafsLightingFactor = 2.0f;
				script.BillboardLeafsContrast = 1.0f;
			}
			EditorGUILayout.EndHorizontal();
			
			// "Shaded" Billboard settings
			GUILayout.Space(4);
			GUI.color = myCol;
			EditorGUILayout.LabelField("Shaded Billboard Settings");
			GUI.color = Color.white;
			script.BillboardLightReference = (GameObject)EditorGUILayout.ObjectField("Light Reference", script.BillboardLightReference, typeof(GameObject), true);
			GUI.enabled = false;
			script.BillboardShadowColor = EditorGUILayout.ColorField ("Shadow Color", script.BillboardShadowColor);
			GUI.enabled = true;
			script.BillboardAmbientLightFactor = EditorGUILayout.Slider("Ambient Light Factor", script.BillboardAmbientLightFactor, 0.0f, 4.0f);
			script.BillboardAmbientLightDesaturationFactor = EditorGUILayout.Slider("Ambient Light Desaturation Factor", script.BillboardAmbientLightDesaturationFactor, 0.0f, 2.0f);
		}
		EditorGUILayout.EndVertical();

	//	Camera Layer Culling Settings
		GUILayout.Space(4);
		EditorGUILayout.BeginVertical("Box");
		GUI.color = myCol;
		script.FoldCulling = EditorGUILayout.Foldout(script.FoldCulling," Camera Culling Settings", myFoldoutStyle );
		GUI.color = Color.white;
		if (script.FoldCulling) {
			EditorGUILayout.BeginHorizontal();
			script.EnableCameraLayerCulling = EditorGUILayout.Toggle("", script.EnableCameraLayerCulling, GUILayout.Width(14) );
			EditorGUILayout.LabelField("Enable Layer Culling");
			EditorGUILayout.EndHorizontal();
			if (script.EnableCameraLayerCulling) {
				script.SmallDetailsDistance = EditorGUILayout.IntSlider("Small Detail Distance", script.SmallDetailsDistance, 10, 300);
				script.MediumDetailsDistance = EditorGUILayout.IntSlider("Medium Detail Distance", script.MediumDetailsDistance, 10, 300);
			}
		}
		EditorGUILayout.EndVertical();

	//	Special Render Settings
		GUILayout.Space(4);
		EditorGUILayout.BeginVertical("Box");
		GUI.color = myCol;
		script.FoldRender = EditorGUILayout.Foldout(script.FoldRender," Special Render Settings", myFoldoutStyle );
		GUI.color = Color.white;
		if (script.FoldRender) {
			EditorGUILayout.BeginHorizontal();
			script.AllGrassObjectsCombined = EditorGUILayout.Toggle("", script.AllGrassObjectsCombined, GUILayout.Width(14) );
			EditorGUILayout.LabelField("All Grass Objects Combined");
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		
		GUILayout.Space(4);
		
		// update settings // we have to call it constantly to make the grass shaders work
		if (GUI.changed) {
			script.Update();
			SceneView.RepaintAll();
		}
	}

	void ResetBillboardFadeLength () {
		allTerrains = FindObjectsOfType(typeof(Terrain)) as Terrain[];
		for (int i = 0; i < allTerrains.Length; i ++) {
			// treeCrossFadeLength must be > 0 otherwise the number of draw calls will explode at steep viewing angles
			allTerrains[i].treeCrossFadeLength = 0.0001f;
		}
		
	}
	
	void RestoreBillboardFadeLength (float resetValue) {
		allTerrains = FindObjectsOfType(typeof(Terrain)) as Terrain[];
		for (int i = 0; i < allTerrains.Length; i ++) {
			allTerrains[i].treeCrossFadeLength = resetValue;
		}
	}

}
#endif
