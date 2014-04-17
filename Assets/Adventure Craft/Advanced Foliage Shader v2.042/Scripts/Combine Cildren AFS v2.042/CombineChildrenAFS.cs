// optimized version of the original "combine children" script which lets you destroy all child objects
// after combining as we ussually do not need them (in case they do not have any colliders attached)
// in case you have added any collider attached to your prefabs simply uncheck "destroyChildObjectsInPlaymode"

using UnityEngine;
using System.Collections;
/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.

Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
*/

[AddComponentMenu("Mesh/AFS Combine Children")]
public class CombineChildrenAFS : MonoBehaviour {
	
	public Terrain UnderlayingTerrain;
	public float GroundMaxDistance = 0.25f;
	
	public bool bakeGroundLightingGrass = false;
	public Color HealthyColor = new Color(1,1,1,1);
	public Color DryColor = new Color(1,1,1,1);
	public float NoiseSpread = 0.1f;
	
	public bool bakeGroundLightingFoliage = false;
	public float randomBrightness = 0.25f;
	public float randomPulse = 0.3f;
	public float randomBending = 0.3f;
	public float randomFluttering = 0.3f;
	public float NoiseSpreadFoliage = 0.1f;
	public bool bakeScale = true;
	
	public bool debugNormals = false;
	
	public bool destroyChildObjectsInPlaymode = true;
	public bool CastShadows = true;
	
	public float RealignGroundMaxDistance = 4.0f;
	public bool ForceRealignment = false;
	
	public bool isStaticallyCombined = false;
	
	public bool simplyCombine = false;
	
	void Start()
	{
		Combine();
	}
	
	[ContextMenu ("Realign Objects")]
	
	public void Realign () {
		Component[] filters = GetComponentsInChildren(typeof(MeshFilter));
		for (int i=0;i<filters.Length;i++) {
			Renderer curRenderer  = filters[i].renderer;
			
			///// sample ground normal
			Vector3 objectPos = curRenderer.transform.position;
			RaycastHit hit;
			if (!Physics.Raycast(objectPos + (Vector3.up * GroundMaxDistance * 0.5f), Vector3.down, out hit, GroundMaxDistance ) || ForceRealignment ) {
				// if ground is too far away...
				
				if (Physics.Raycast(objectPos + (Vector3.up * RealignGroundMaxDistance * 0.5f), Vector3.down, out hit, RealignGroundMaxDistance)) {
					curRenderer.transform.position = hit.point;
					Quaternion originalRotation = curRenderer.transform.rotation;
					curRenderer.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					originalRotation = new Quaternion(curRenderer.transform.rotation.x, originalRotation.y, curRenderer.transform.rotation.z, curRenderer.transform.rotation.w);
					curRenderer.transform.rotation = originalRotation; 
				}
				
			}
			
		}
		
	}
	
	public bool GetDebugNormalsScript () {
		GameObject CombineObj = gameObject;
		Component DebugNormalsScript = CombineObj.GetComponent<DebugNormalsInEditmode>();
		if ( DebugNormalsScript == null ){
			return false;
		}
		return true;
	}
	
	public void EnableDebugging() {
		GameObject CombineObj = gameObject;
		if (GetDebugNormalsScript() == false) {
			CombineObj.AddComponent<DebugNormalsInEditmode>();
		}
		CombineObj.GetComponent<DebugNormalsInEditmode>().enabled = true;
	}
	
	public void DisableDebugging() {
		GameObject CombineObj = gameObject;
		if (GetDebugNormalsScript() == true) {
			CombineObj.GetComponent<DebugNormalsInEditmode>().enabled = false;
		}
	}
	
	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
	/// 
	[ContextMenu ("Combine Now")]
	/// 
	public void Combine () {

		/*if (bakeGroundLightingGrass && bakeGroundLightingFoliage) {
			Debug.Log ("You can not combine meshes using 2 different materials.");
			return;	
		}*/
		
		if (isStaticallyCombined) {
			return;	
		}
				
		Component[] filters = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Hashtable materialToMesh= new Hashtable();
		
		for (int i=0;i<filters.Length;i++) {
			MeshFilter filter = (MeshFilter)filters[i];
			Renderer curRenderer  = filters[i].renderer;
			
			///// sample ground normal
			Vector3 objectPos = curRenderer.transform.position;
			RaycastHit hit1;
			
			if (Physics.Raycast(objectPos + (Vector3.up * GroundMaxDistance * 0.5f), Vector3.down, out hit1, GroundMaxDistance)) {
				if(debugNormals) {
					Debug.DrawLine(objectPos + (Vector3.up * GroundMaxDistance * 0.5f), objectPos - (GroundMaxDistance * Vector3.up * 0.5f), Color.green, 5.0f, false);
					if(debugNormals) {
						Debug.DrawLine(objectPos, objectPos + (hit1.normal), Color.red, 5.0f, false);
					}
					
					// is it terrain? that makes a big difference!
					if (UnderlayingTerrain) {
						Vector3 terrainPos = (hit1.point - UnderlayingTerrain.transform.position) / UnderlayingTerrain.terrainData.size.x;
						//Debug.Log (hit1.point.y);	
						//Debug.Log ( UnderlayingTerrain.SampleHeight(terrainPos) + UnderlayingTerrain.transform.position.y );
						//Debug.Log (hit1.transform.gameObject.name);
						//if ( hit1.point.y - UnderlayingTerrain.SampleHeight(terrainPos) < 1.5f && hit1.point.y - UnderlayingTerrain.SampleHeight(terrainPos) > -1.5f  ) {
						if (hit1.transform.gameObject.name == UnderlayingTerrain.name ){
							if(debugNormals) {
								Debug.DrawLine(objectPos, objectPos + (UnderlayingTerrain.terrainData.GetInterpolatedNormal(terrainPos.x, terrainPos.z)), Color.blue, 5.0f, false);
							}
							hit1.normal = UnderlayingTerrain.terrainData.GetInterpolatedNormal(terrainPos.x, terrainPos.z);
						}
					}
				}
			}
			else {
				hit1.normal = new Vector3 (0,1,0);
				if(debugNormals) {
					Debug.DrawLine(objectPos, objectPos + (1.0f * hit1.normal), Color.yellow, 5.0f, false);
				}
			}
			
			
			MeshCombineUtilityAFS.MeshInstance instance = new MeshCombineUtilityAFS.MeshInstance ();
			instance.mesh = filter.sharedMesh;
			instance.groundNormal = hit1.normal;
			instance.scale = filter.transform.localScale.x;
			instance.pivot = filter.transform.position;
			
			
			// store max height in worldspace
			// Bounds bounds = curRenderer.bounds;
			// store max height in local space
			// Bounds bounds = filter.sharedMesh.bounds;
			// instance.maxHeight = bounds.size.y;
			
			if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				Material[] materials = curRenderer.sharedMaterials;
				for (int m=0;m<materials.Length;m++) {
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
					ArrayList objects = (ArrayList)materialToMesh[materials[m]];
					if (objects != null) {
						objects.Add(instance);
					}
					else
					{
						objects = new ArrayList ();
						objects.Add(instance);
						materialToMesh.Add(materials[m], objects);
					}
				}
				
				// handle dynamic combine
				if (Application.isPlaying) {
					if (destroyChildObjectsInPlaymode) {
						Destroy(curRenderer.gameObject);
					}
				}
				else {
					if (destroyChildObjectsInPlaymode) {
						DestroyImmediate(curRenderer.gameObject);
						isStaticallyCombined = true;
					}
					else {
						#if UNITY_3_5
						curRenderer.gameObject.active = false;
						#else
						curRenderer.gameObject.SetActive(false);
						#endif
						isStaticallyCombined = true;
					}
				}
					
			}
		}
	
		foreach (DictionaryEntry de in materialToMesh) {
			ArrayList elements = (ArrayList)de.Value;
			MeshCombineUtilityAFS.MeshInstance[] instances = (MeshCombineUtilityAFS.MeshInstance[])elements.ToArray(typeof(MeshCombineUtilityAFS.MeshInstance));

			// We have a maximum of one material, so just attach the mesh to our own game object
			if (materialToMesh.Count == 1)
			{
			
				// Make sure we have a mesh filter & renderer
				if (GetComponent(typeof(MeshFilter)) == null)
					gameObject.AddComponent(typeof(MeshFilter));
				if (!GetComponent("MeshRenderer"))
					gameObject.AddComponent("MeshRenderer");
				MeshFilter filter = (MeshFilter)GetComponent(typeof(MeshFilter));
				
//				///////
				renderer.material = (Material)de.Key;
				//renderer.enabled = true;
				
				// check material and set up the combine script automatically
				bakeGroundLightingGrass = false;
				bakeGroundLightingFoliage = false;
				simplyCombine = false;
				
				string result;
				if (Application.isPlaying) {
					result = renderer.material.GetTag("RenderType", true, "");
					// RenderTags are: AfsGrassModelSingleSided / AfsGrassModel / AtsFoliage
					if (result == "AfsGrassModel" || result == "AfsGrassModelSingleSided" ) {
						bakeGroundLightingGrass = true;
					}
					else if (result == "AtsFoliage") {
						// just foliage of groundlighting?
						if (renderer.material.HasProperty("_GroundLightingAttunation")) {
							bakeGroundLightingFoliage = true;
						}
						else {
							simplyCombine = true;
						}
					}
					filter.mesh = MeshCombineUtilityAFS.Combine(instances, bakeGroundLightingGrass, bakeGroundLightingFoliage, randomBrightness, randomPulse, randomBending, randomFluttering, HealthyColor, DryColor, NoiseSpread, bakeScale, simplyCombine, NoiseSpreadFoliage);
				}
				else {
					result = renderer.sharedMaterial.GetTag("RenderType", true, "");
					// RenderTags are: AfsGrassModelSingleSided / AfsGrassModel / AtsFoliage
					if (result == "AfsGrassModel" || result == "AfsGrassModelSingleSided" ) {
						bakeGroundLightingGrass = true;
					}
					else if (result == "AtsFoliage") {
						// just foliage of groundlighting?
						if (renderer.sharedMaterial.HasProperty("_GroundLightingAttunation")) {
							bakeGroundLightingFoliage = true;
						}
						else {
							simplyCombine = true;
						}
					}
					filter.sharedMesh = MeshCombineUtilityAFS.Combine(instances, bakeGroundLightingGrass, bakeGroundLightingFoliage, randomBrightness, randomPulse, randomBending, randomFluttering, HealthyColor, DryColor, NoiseSpread, bakeScale, simplyCombine, NoiseSpreadFoliage);	

				}
				
				//renderer.material = (Material)de.Key;
				renderer.enabled = true;
				if (CastShadows) {
					renderer.castShadows = true;
				}
				else {
					renderer.castShadows = false;	
				}
			}
			// We have multiple materials to take care of, build one mesh / gameobject for each material
			// and parent it to this object
			else
			{
				GameObject go = new GameObject("Combined mesh");
				go.transform.parent = transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent("MeshRenderer");
				go.renderer.material = (Material)de.Key;
				go.layer = go.transform.parent.gameObject.layer;
				
//				///////
				
				// check material and set up the combine script automatically
				bakeGroundLightingGrass = false;
				bakeGroundLightingFoliage = false;
				simplyCombine = false;
				string result;
				if (Application.isPlaying) {
					result = go.renderer.material.GetTag("RenderType", true, "");
				}
				else {
					result = go.renderer.sharedMaterial.GetTag("RenderType", true, "");	
				}
				
				// RenderTags are: AfsGrassModelSingleSided / AfsGrassModel / AtsFoliage
				if (result == "AfsGrassModel" || result == "AfsGrassModelSingleSided" ) {
					bakeGroundLightingGrass = true;
				}
				else if (result == "AtsFoliage") {
					
					// just foliage of groundlighting?
					if (Application.isPlaying) {
						if (go.renderer.material.HasProperty("_GroundLightingAttunation")) {
							bakeGroundLightingFoliage = true;
						}
						else {
							simplyCombine = true;
						}
					}
					else {
						if (go.renderer.sharedMaterial.HasProperty("_GroundLightingAttunation")) {
							bakeGroundLightingFoliage = true;
						}
						else {
							simplyCombine = true;
						}	
					}
				}
//				///////
				
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				if (Application.isPlaying) {
					filter.mesh = MeshCombineUtilityAFS.Combine(instances, bakeGroundLightingGrass, bakeGroundLightingFoliage, randomBrightness, randomPulse, randomBending, randomFluttering, HealthyColor, DryColor, NoiseSpread, bakeScale, simplyCombine, NoiseSpreadFoliage);
				}
				else {
					filter.sharedMesh = MeshCombineUtilityAFS.Combine(instances, bakeGroundLightingGrass, bakeGroundLightingFoliage, randomBrightness, randomPulse, randomBending, randomFluttering, HealthyColor, DryColor, NoiseSpread, bakeScale, simplyCombine, NoiseSpreadFoliage);
				}
			}
		}
	}	
}