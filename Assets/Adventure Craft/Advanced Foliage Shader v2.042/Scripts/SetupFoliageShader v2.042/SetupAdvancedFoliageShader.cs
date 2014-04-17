using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetupAdvancedFoliageShader : MonoBehaviour {

	//	Editor Variables
	#if UNITY_EDITOR
	public bool newEditor = true;
	public bool FoldWind = false;
	public bool FoldRain = false;
	public bool FoldGrass = false;
	public bool FoldVegTerrain = false;
	public bool FoldBillboard = false;
	public bool FoldCulling = false;
	public bool FoldRender = false;
	#endif
	
	//	Wind parameters which are needed by all shaders
	public Vector4 Wind = new Vector4(0.85f,0.075f,0.4f,0.5f);
	public float WindFrequency = 0.75f;
	public float WaveSizeFoliageShader = 10.0f;
	//	Wind parameters only needed by the advanced grass shaders
	public float WindMultiplierForGrassshader = 1.0f;
	public float WaveSizeForGrassshader = 10.0f;

	//	Rain Settings
	public float RainAmount = 0.0f;
	public float SpecPower = 1.0f;
	
	//	Terrain Detail Vegetation Settings
	public float VertexLitAlphaCutOff = 0.3f;
	public Color VertexLitTranslucencyColor = new Color(0.73f,0.85f,0.4f,1f);
	public float VertexLitTranslucencyViewDependency = 0.7f;
	public float VertexLitShadowStrength = 0.8f;
	public float VertexLitShininess = 0.2f;

	//	Grass, Tree and Billboard settings
	public bool AutoSyncToTerrain = false;
	public Terrain SyncedTerrain;
	public bool AutoSyncInPlaymode = false;
	public float DetailDistanceForGrassShader = 80.0f;
	public float BillboardStart = 50.0f;
	public float BillboardFadeLenght = 5.0f;
	
	public bool GrassAnimateNormal = false;
	public Color GrassWavingTint;
	
	//	Tree Render settings
	public bool TreeShadowDissolve = false;
	public bool TreeBillboardShadows = false;
	public int TreeBillboardLOD = 200;
	public bool TreeShadowEdgeFade = false;
	public bool BillboardShadowEdgeFade = false;
	public float BillboardShadowEdgeFadeThreshold = 0.1f;
	public float BillboardFadeOutLength = 60.0f;
	public bool BillboardAdjustToCamera = true;
	public float BillboardAngleLimit = 30.0f;

	public Shader AfsTreeBillboardShadowShader;
	
	//	Billboard Render Tex settings
	public float BillboardTranslucencyFactor = 1.8f;
	public float BillboardLeafsLightingFactor = 2.13f;
	public float BillboardLeafsContrast = 1.05f;
	
	//	"Shaded" Billboard settings
	public GameObject BillboardLightReference;
	public Color BillboardShadowColor;
	public float BillboardAmbientLightFactor = 1.0f;
	public float BillboardAmbientLightDesaturationFactor = 0.7f;

	//	Camera Layer Culling Settings
	public bool EnableCameraLayerCulling = true;
	public int SmallDetailsDistance = 70;
	public int MediumDetailsDistance = 90;

	//	Special Render Settings
	public bool AllGrassObjectsCombined = false;

	// Init vars used by the scripts
	private Vector4 TempWind;
	private float TempWindForce;
	private float GrassWind;
	private Vector3 CameraForward = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3 ShadowCameraForward = new Vector3(0.0f, 0.0f, 0.0f);
	private float rollingX;
	private float rollingXShadow;
	private Vector3 lightDir;
	private Vector3 templightDir;
	private float CameraAngle;
	private Terrain[] allTerrains;
	private float grey;

//	////////////////////////////////

	void Awake () {
		AfsTreeBillboardShadowShader = Shader.Find("Hidden/TerrainEngine/BillboardTree");

		afsSetupTerrainEngine();
		afsSetupGrassShader();
		afsUpdateWind();
		afsUpdateRain();
		afsAutoSyncToTerrain();
		afsUpdateTreeAndBillboardShaders();
		afsUpdateGrassTreesBillboards();
		afsSetupCameraLayerCulling();
	}

	// Use this for initialization
	void Start () {
		AfsTreeBillboardShadowShader = Shader.Find("Hidden/TerrainEngine/BillboardTree");
	}

	// Update is called once per frame
	public void Update () {
		#if UNITY_EDITOR
		afsSetupTerrainEngine();
		afsSetupGrassShader();
		#endif
		afsUpdateWind();
		afsUpdateRain();
		afsAutoSyncToTerrain();
		afsUpdateTreeAndBillboardShaders();
		afsUpdateGrassTreesBillboards();
	}


//	////////////////////////////////

//	Special Render Settings
	void afsSetupGrassShader() {
		//	Tell the "advancedGrassShader Groundlighting" how to lit the objects
		if (Application.isPlaying || AllGrassObjectsCombined) {
			// Lighting based on baked normals
			Shader.DisableKeyword("IN_EDITMODE");
			Shader.EnableKeyword("IN_PLAYMODE");
		}
		else {
			// Lighting according to rotation
			Shader.DisableKeyword("IN_PLAYMODE");
			Shader.EnableKeyword("IN_EDITMODE");
		}
	}

//	Terrain engine settings
	void afsSetupTerrainEngine() {
		Shader.SetGlobalFloat("_AfsAlphaCutOff", VertexLitAlphaCutOff);
		Shader.SetGlobalColor("_AfsTranslucencyColor", VertexLitTranslucencyColor);
		Shader.SetGlobalFloat("_AfsTranslucencyViewDependency", VertexLitTranslucencyViewDependency);
		Shader.SetGlobalFloat("_AfsShadowStrength", VertexLitShadowStrength);
		Shader.SetGlobalFloat("_AfsShininess", VertexLitShininess);
	}

//	Update Wind Settings / Simple wind animation for the foliage shaders
	void afsUpdateWind() {
		TempWind  = Wind;
		TempWindForce = Wind.w;
		TempWind.x *= (1.25f + Mathf.Sin(UnityEngine.Time.time * WindFrequency) * Mathf.Sin(UnityEngine.Time.time * 0.375f)) * 0.5f;
		TempWind.z *= (1.25f + Mathf.Sin(UnityEngine.Time.time * WindFrequency) * Mathf.Sin(UnityEngine.Time.time * 0.193f)) * 0.5f;
		TempWind.w = TempWindForce;
		Shader.SetGlobalVector("_Wind", TempWind);
		GrassWind = (TempWind.x + TempWind.z) / 2.0f;
		Shader.SetGlobalFloat("_AfsWaveSize", (0.5f / WaveSizeFoliageShader) );
		// Wind animation for the grass shader on manually placed game objects: wind speed, wave size, wind amount, max pow2 distance
		Shader.SetGlobalVector("_AfsWaveAndDistance", new Vector4( UnityEngine.Time.time * (WindFrequency * 0.05f) , (0.5f / WaveSizeForGrassshader ), GrassWind * WindMultiplierForGrassshader, DetailDistanceForGrassShader * DetailDistanceForGrassShader ) );
	}

//	Update Rain Settings
	void afsUpdateRain() {
		Shader.SetGlobalFloat("_AfsRainamount", RainAmount);
		Shader.SetGlobalFloat("_AfsSpecPower", SpecPower);
	}

//	AutoSyncToTerrain
	void afsAutoSyncToTerrain() {
		if(AutoSyncToTerrain && SyncedTerrain != null) {
			DetailDistanceForGrassShader = SyncedTerrain.detailObjectDistance;
			BillboardStart = SyncedTerrain.treeBillboardDistance;
			if(!TreeBillboardShadows) {
				BillboardFadeLenght = SyncedTerrain.treeCrossFadeLength;
			}
			GrassWavingTint = SyncedTerrain.terrainData.wavingGrassTint;
		}
	}
	
//	Setup Tree and Billboard Shaders
	void afsUpdateTreeAndBillboardShaders() {
		if(TreeShadowDissolve) {
			Shader.EnableKeyword("TREE_SHADOW_DISSOLVE");
			Shader.DisableKeyword("TREE_SHADOW_NO_DISSOLVE");
		}
		else {
			Shader.EnableKeyword("TREE_SHADOW_NO_DISSOLVE");
			Shader.DisableKeyword("TREE_SHADOW_DISSOLVE");
		}
		if(TreeBillboardShadows) {
			Shader.EnableKeyword("BILLBOARD_SHADOWS");
			Shader.DisableKeyword("BILLBOARD_NO_SHADOWS");
		}
		else {
			Shader.EnableKeyword("BILLBOARD_NO_SHADOWS");
			Shader.DisableKeyword("BILLBOARD_SHADOWS");
		}
		if(TreeShadowEdgeFade) {
			Shader.EnableKeyword("TREESHADOW_EDGEFADE");
			Shader.DisableKeyword("TREESHADOW_NO_EDGEFADE");
		}
		else {
			Shader.EnableKeyword("TREESHADOW_NO_EDGEFADE");
			Shader.DisableKeyword("TREESHADOW_EDGEFADE");
		}
		if (BillboardShadowEdgeFade) {
			Shader.EnableKeyword("BILLBOARDSHADOW_EDGEFADE");
			Shader.DisableKeyword("BILLBOARDSHADOW_NO_EDGEFADE");
		}
		else {
			Shader.EnableKeyword("BILLBOARDSHADOW_NO_EDGEFADE");
			Shader.DisableKeyword("BILLBOARDSHADOW_EDGEFADE");
		}
		if(GrassAnimateNormal) {
			Shader.EnableKeyword("GRASS_ANIMATE_NORMAL");
			Shader.DisableKeyword("GRASS_ANIMATE_COLOR");	
		}
		else {
			Shader.EnableKeyword("GRASS_ANIMATE_COLOR");
			Shader.DisableKeyword("GRASS_ANIMATE_NORMAL");	
		}

		if(TreeBillboardShadows && TreeBillboardLOD != 300 && AfsTreeBillboardShadowShader != null) {
		//if(TreeBillboardShadows && AfsTreeBillboardShadowShader != null) {
			//AfsTreeBillboardShadowShader = Shader.Find("Hidden/TerrainEngine/BillboardTree");
			AfsTreeBillboardShadowShader.maximumLOD = 300;
			TreeBillboardLOD = 300;
		}
		if (!TreeBillboardShadows && TreeBillboardLOD != 200 && AfsTreeBillboardShadowShader != null) { 
		//if (!TreeBillboardShadows && TreeBillboardLOD != 200 & AfsTreeBillboardShadowShader != null) { 
			//AfsTreeBillboardShadowShader = Shader.Find("Hidden/TerrainEngine/BillboardTree");
			AfsTreeBillboardShadowShader.maximumLOD = 200;
			TreeBillboardLOD = 200;
		}

	}
	
//	Grass, Tree and Billboard Settings
	void afsUpdateGrassTreesBillboards() {
		// DetailDistanceForGrassShader has already been passed with: _AfsWaveAndDistance
		Shader.SetGlobalColor("_AfsWavingTint",GrassWavingTint); 
		// Tree Variables
		Shader.SetGlobalVector("_AfsTerrainTrees", new Vector4(BillboardStart, BillboardFadeLenght, BillboardFadeOutLength, 0 ));
		//	Billboard Texture Settings
		Shader.SetGlobalVector("_AfsBillboardAdjustments", new Vector4( 1, BillboardTranslucencyFactor, BillboardLeafsLightingFactor, BillboardLeafsContrast));

		//	Camera Settings for the Billboard Shader
		if (BillboardAdjustToCamera) {
			if (Camera.main) {
				CameraForward = Camera.main.transform.forward;
				ShadowCameraForward = CameraForward;
				rollingX = Camera.main.transform.eulerAngles.x;
			}
			else {
				Debug.Log("You have to tag your Camera as MainCamera");
			}
			if (rollingX >= 270.0f) {					// looking up
				rollingX = (rollingX - 270.0f);
				rollingX = (90.0f - rollingX);
				rollingXShadow = rollingX;
			}
			else {										// looking down
				rollingXShadow = -rollingX;
				if (rollingX > BillboardAngleLimit) {
					rollingX = Mathf.Lerp(rollingX, 0.0f,  (rollingX / BillboardAngleLimit) - 1.0f );
				}
				rollingX *= -1;
			}
		}
		else {
			rollingX = 0.0f;
			rollingXShadow = 0.0f;
		}
		CameraForward *= rollingX / 90.0f;
		ShadowCameraForward *= rollingXShadow / 90.0f;
		Shader.SetGlobalVector("_AfsBillboardCameraForward", new Vector4( CameraForward.x, CameraForward.y, CameraForward.z, 1.0f));
		Shader.SetGlobalVector("_AfsBillboardShadowCameraForward", new Vector4( ShadowCameraForward.x, ShadowCameraForward.y, ShadowCameraForward.z, 1.0f));

		// Set lightDir for Billboard Shadows
		if (TreeBillboardShadows) {
				if (BillboardLightReference != null) {
				lightDir = BillboardLightReference.transform.forward;
				templightDir = lightDir;
				lightDir = Vector3.Cross(lightDir, Vector3.up);
				// flip lightDir if camera is aligned with light
				if (Vector3.Dot(templightDir, Camera.main.transform.forward) > 0) {
					lightDir = Quaternion.AngleAxis(180, Vector3.up) * lightDir;
				}
				Shader.SetGlobalVector("_AfsSunDirection", new Vector4(lightDir.x, lightDir.y, lightDir.z, 1) );

				//	Unity 4.3 fix
				//	treeCrossFadeLength must be > 0 when light is more or less behind our camera
				//	otherwise unity will produce an incredible high number of draw calls
				allTerrains = FindObjectsOfType(typeof(Terrain)) as Terrain[];
				Vector3 CameraForwardVec = Camera.main.transform.forward; //.right;
				if ( Vector3.Dot(Camera.main.transform.forward, BillboardLightReference.transform.forward) > 0.5f ) {
					for (int i = 0; i < allTerrains.Length; i ++) {
						allTerrains[i].treeCrossFadeLength = 0.001f;
					}
				}
				else {
					for (int j = 0; j < allTerrains.Length; j ++) {
						allTerrains[j].treeCrossFadeLength = 0.0f;
					}
					//CameraForwardVec = -CameraForwardVec; //forward;
				}
				///
				CameraAngle = Camera.main.fieldOfView;
				CameraAngle = CameraAngle - CameraAngle * BillboardShadowEdgeFadeThreshold;
				CameraAngle = Mathf.Cos( CameraAngle * Mathf.Deg2Rad );
				Shader.SetGlobalVector("_CameraForwardVec", new Vector4(CameraForwardVec.x, CameraForwardVec.y, CameraForwardVec.z, CameraAngle) );
			}
			else {
				Debug.LogWarning("You have to specify a Light Reference!");
			}
		}
	
		//	Set desaturated ambient light for shaded billboards
		BillboardShadowColor = RenderSettings.ambientLight;
		BillboardShadowColor = Desaturate(BillboardShadowColor.r * BillboardAmbientLightFactor, BillboardShadowColor.g * BillboardAmbientLightFactor, BillboardShadowColor.b * BillboardAmbientLightFactor);
		if (BillboardLightReference) {
			BillboardShadowColor += 0.5f * (BillboardShadowColor * (1.0f - BillboardLightReference.light.shadowStrength));
		}
		Shader.SetGlobalColor("_AfsAmbientBillboardLight", BillboardShadowColor );
	}

//	Camera Layer Culling Settings
	void afsSetupCameraLayerCulling() {
		if(EnableCameraLayerCulling) { 
			for (int i = 0; i < Camera.allCameras.Length; i++) {
				float[] distances = new float[32];
				distances = Camera.allCameras[i].layerCullDistances;
				distances[8] = SmallDetailsDistance; // small things like DetailDistance of the terrain engine
				distances[9] = MediumDetailsDistance; // small things like DetailDistance of the terrain engine
				Camera.allCameras[i].layerCullDistances = distances;
				distances = null;
			}
		}
	}


//	////////////////////////////////

//	Helper functions

	private Color Desaturate(float r, float g, float b) {
		grey = 0.3f * r + 0.59f * g + 0.11f * b;
		r = grey * BillboardAmbientLightDesaturationFactor + r * (1.0f - BillboardAmbientLightDesaturationFactor);
		g = grey * BillboardAmbientLightDesaturationFactor + g * (1.0f - BillboardAmbientLightDesaturationFactor);
		b = grey * BillboardAmbientLightDesaturationFactor + b * (1.0f - BillboardAmbientLightDesaturationFactor);
		return (new Color(r, g, b, 1.0f));
	}
}
