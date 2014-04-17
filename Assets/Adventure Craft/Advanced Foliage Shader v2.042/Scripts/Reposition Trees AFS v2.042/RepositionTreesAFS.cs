using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Terrain))]
public class RepositionTreesAFS : MonoBehaviour {
	
	public bool[] ExcludetreePrototype;
	public string[] TreePrototypeName;
	
	public List<GameObject> additionalTerrainObjects; // GameObjects you want to place trees on top
	public Vector3[] treesPos;
	public Terrain myterrainComp;
	
	// Use this for initialization
	public void UpdatePrototypes () {
		myterrainComp = gameObject.GetComponent(typeof(Terrain)) as Terrain;
		
		// already initialized?
		if (ExcludetreePrototype == null) {
			ExcludetreePrototype = new bool[myterrainComp.terrainData.treePrototypes.Length];
			TreePrototypeName = new string[myterrainComp.terrainData.treePrototypes.Length];
			for(int k = 0; k < myterrainComp.terrainData.treePrototypes.Length; k++) {
				// set prototype names
				TreePrototypeName[k] = myterrainComp.terrainData.treePrototypes[k].prefab.name;
			}	
		}
		// any changes?
		else if (ExcludetreePrototype.Length != myterrainComp.terrainData.treePrototypes.Length) {
			ExcludetreePrototype = new bool[myterrainComp.terrainData.treePrototypes.Length];
			TreePrototypeName = new string[myterrainComp.terrainData.treePrototypes.Length];
			for(int k = 0; k < myterrainComp.terrainData.treePrototypes.Length; k++) {
				// set prototype names
				TreePrototypeName[k] = myterrainComp.terrainData.treePrototypes[k].prefab.name;
			}
		}
		
	}
	
	public void RepositionTrees () {
		Terrain terrainComp = gameObject.GetComponent(typeof(Terrain)) as Terrain;
		TreeInstance[] trees = terrainComp.terrainData.treeInstances;
		Vector3 terSize = terrainComp.terrainData.size;
		Vector3 terPos = terrainComp.GetPosition();
		treesPos = new Vector3[trees.Length];
		
		for(int j = 0; j < trees.Length; j++) {
			// Transform TreePosition into Worldspace
			treesPos[j] = new Vector3(trees[j].position.x * terSize.x, trees[j].position.y * terSize.y, trees[j].position.z * terSize.z) + terPos + Vector3.up;
		}
		
		RaycastHit raycastHit;
		for(int i = 0; i < trees.Length; i ++) {
			// check if prefab is excluded
			int ThisPrototypeIndex = trees[i].prototypeIndex;
			//Debug.Log (ThisPrototypeIndex);
			if (!ExcludetreePrototype[ThisPrototypeIndex]) {
				Vector3 treePos= new Vector3(treesPos[i].x, 2000.0f, treesPos[i].z) ;
				
				// check against terrain
				float yPos = trees[i].position.y;
				float xPos = trees[i].position.x;
				float zPos = trees[i].position.z;
				RaycastHit raycastTerrain;
				if (terrainComp.collider.Raycast(new Ray(treePos, new Vector3 (0.0f, -1.0f, 0.0f)), out raycastTerrain, Mathf.Infinity)) {
					yPos = raycastTerrain.point.y / terSize.y;
				}
				
				for(int j = 0; j < additionalTerrainObjects.Count; j++) {
					if (additionalTerrainObjects[j].collider) {
							if (additionalTerrainObjects[j].collider.Raycast(new Ray(treePos, new Vector3 (0.0f, -1.0f, 0.0f)), out raycastHit, Mathf.Infinity)) {
								//Debug.DrawLine (raycastHit.point, raycastHit.point + new Vector3(0,2,0), Color.blue, 1.0f);
								float yPosCollider = raycastHit.point.y / terSize.y ;
								// check against terrain
								if (yPos < yPosCollider) {
									yPos = yPosCollider;
								}
							}
						// do it
						Vector3 newPosition = new Vector3 (xPos, yPos, zPos);
						trees[i].position = newPosition;
						
					}
				}
			}
		}
		terrainComp.terrainData.treeInstances = trees;
	}
}
