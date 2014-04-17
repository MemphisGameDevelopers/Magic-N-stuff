using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// based on a collaboration of tomaszek, becoming and me for the rtp3 demo
// script originally written by tomaszek / adopted and improved by me

[RequireComponent(typeof(Terrain))]
[ExecuteInEditMode]
public class ShadedBillboardsAFS : MonoBehaviour {
	
	public bool showRays;
	public bool shadeOnlyWithinShadowRange = false;
	
	public GameObject lightRef;
	public List<GameObject> shadowCasters;
	
	public int treesPerFrame = 10;
	public float changeStateSpeed = 4f;
	
	public float treeYOffset = 0f;
	
	public Vector3[] treesPos;
	public bool[] treesShadowed;
	public float[] treesStates;
	public int curIdx;

	// general vars to avoid garbage collection
	private Terrain terrainComp;
	private TreeInstance[] treeInstances;

	void Awake () {
		terrainComp = gameObject.GetComponent(typeof(Terrain)) as Terrain;
		treeInstances = terrainComp.terrainData.treeInstances;
	}
	
	void Update () {
		if (Application.isPlaying) {
			ShadeTrees();
		}
		#if UNITY_EDITOR
		else {
			if (treeInstances == null) {
				treeInstances = terrainComp.terrainData.treeInstances;
				UpdateTrees();
			}
			else if (treeInstances.Length != terrainComp.terrainData.treeInstances.Length) {
				treeInstances = terrainComp.terrainData.treeInstances;
				UpdateTrees();
			}
		}
		#endif
		//Terrain.activeTerrain.terrainData.ComputeDelayedLod();
	}
	
//	function to reset the array
	public void UpdateTrees () {
		if (treeInstances == null) {
			treeInstances = terrainComp.terrainData.treeInstances;
		}
		Vector3 terSize = terrainComp.terrainData.size;
		Vector3 terPos = terrainComp.GetPosition();
		treesPos = new Vector3[treeInstances.Length];
		treesShadowed = new bool[treeInstances.Length];
		treesStates = new float[treeInstances.Length];
		curIdx = 0;
		for(int j = 0; j < treeInstances.Length; j++) {
			treesPos[j] = new Vector3(treeInstances[j].position.x * terSize.x, treeInstances[j].position.y * terSize.y, treeInstances[j].position.z * terSize.z) + terPos + Vector3.up;
			treesShadowed[j] = false; // not shadowed
			treesStates[j] = 0;
		}
	}
	
//	function to remove shadows added by the lightmapper
	public void RemoveLightMap () {
		if (treeInstances == null) {
			treeInstances = terrainComp.terrainData.treeInstances;
		}
		for(int i = 0; i < treeInstances.Length; i ++) {
			treeInstances[i].lightmapColor = new Color(1 , 1 , 1 , 1.0f);
		}
		terrainComp.terrainData.treeInstances = treeInstances;
	}

//	function to remove shadows added by the script
	public void RemoveShadowing () {
		if (treeInstances == null) {
			treeInstances = terrainComp.terrainData.treeInstances;
		}
		for(int i = 0; i < treeInstances.Length; i ++) {
			treeInstances[i].color = new Color(treeInstances[i].color.r,treeInstances[i].color.g, treeInstances[i].color.b, 1.0f);
		}
		terrainComp.terrainData.treeInstances = treeInstances;
	}

//	function to calculate the shadows
	public void ShadeTrees () {
		if (lightRef == null) return;
		if (shadowCasters == null) return;

		if (treesPos == null || treesPos.Length == 0) {
			UpdateTrees();
		}
		float timF = Time.deltaTime;
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			timF = 0.1f;
		}
		#endif

		if (treeInstances == null) {
			treeInstances = terrainComp.terrainData.treeInstances;
		}

		int numTrees = treeInstances.Length;
		if (lightRef.light.shadows != LightShadows.None) {	
			RaycastHit raycastHit;
			for(int k = 0; k < treesPerFrame; k++) {
				int j = (curIdx + k) % numTrees;
				Vector3 treePos = treesPos[j];
				treesShadowed[j] = false;
				for(int i = 0; i < shadowCasters.Count; i++) {
					if (shadowCasters[i].collider) {
						// RayCast only due to rotation of sun
						if (shadowCasters[i].collider.Raycast(new Ray(treePos + new Vector3(0,treeYOffset,0), lightRef.transform.rotation * new Vector3 (0.0f, 0.0f, -1.0f)), out raycastHit, Mathf.Infinity)) {
							treesShadowed[j] = true;
							#if UNITY_EDITOR
							if(showRays) {
								Debug.DrawLine(treePos + new Vector3(0,treeYOffset,0), lightRef.transform.rotation * new Vector3 (0.0f, 0.0f, -1000.0f) + treePos + new Vector3(0,treeYOffset,0), Color.red, .1f);
							}
							#endif
							break;
						}
						#if UNITY_EDITOR
						else {
							if(showRays) {
								Debug.DrawLine(treePos + new Vector3(0,treeYOffset,0), lightRef.transform.rotation * new Vector3 (0.0f, 0.0f, -1000.0f) + treePos + new Vector3(0,treeYOffset,0), Color.green, .1f);
							}
						}
						#endif
					} else {
						for(int c = 0; c < shadowCasters[i].transform.childCount; c++) {
							Collider col = shadowCasters[i].transform.GetChild(c).collider;
							if (col) {
								// RayCast only due to rotation of sun
								if (shadowCasters[i].collider.Raycast(new Ray(treePos + new Vector3(0,treeYOffset,0), lightRef.transform.rotation * new Vector3 (0.0f, 0.0f, -1.0f)), out raycastHit, Mathf.Infinity)) {
									treesShadowed[j]=true;
									#if UNITY_EDITOR
									if(showRays) {
										Debug.DrawLine(treePos + new Vector3(0,treeYOffset,0), lightRef.transform.rotation * new Vector3 (0.0f, 0.0f, -1000.0f) + treePos + new Vector3(0,treeYOffset,0), Color.blue, .1f);
									}
									#endif
									break;
								}
							}
						}
						if (treesShadowed[j]) break;
					}
				}
			}
			curIdx+=treesPerFrame;
			if (curIdx >= numTrees) curIdx -= numTrees;
			for(int j = 0; j < numTrees; j++) {
				Vector3 treePos = treesPos[j];
				float dist = Vector3.Distance(Camera.main.transform.position, treePos);
				// shade only within real time shadow distance
				bool inDistance = dist > QualitySettings.shadowDistance;
				if (shadeOnlyWithinShadowRange && inDistance) {
					treeInstances[j].color = new Color(treeInstances[j].color.r,treeInstances[j].color.g, treeInstances[j].color.b, 1.0f);
				} else {
					if (treesShadowed[j] && treesStates[j] < 1.0f) {
						treesStates[j] += changeStateSpeed*timF;
					} else if (!treesShadowed[j] && treesStates[j] > 0f) {
						treesStates[j]-=changeStateSpeed * timF;
					}
					treesStates[j] = Mathf.Clamp01 (treesStates[j]);
					treeInstances[j].color = Color.Lerp(new Color(treeInstances[j].color.r,treeInstances[j].color.g, treeInstances[j].color.b, 1.0f), new Color(treeInstances[j].color.r,treeInstances[j].color.g, treeInstances[j].color.b, 0.0f),treesStates[j]);
				}
			}
		} else {
			// no shadows
			for(int j=0; j<numTrees; j++) {
				treeInstances[j].color = new Color(treeInstances[j].color.r,treeInstances[j].color.g, treeInstances[j].color.b, 1.0f);
			}
		}
		terrainComp.terrainData.treeInstances = treeInstances;
	}
}
