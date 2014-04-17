#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

public enum TextureSplit {
	OneByOne,
	TwoByOne,
	Other
}

[AddComponentMenu("Mesh/AFS Foliage Tool")]
public class AfsFoliageTool : MonoBehaviour {
	
	public float Scale = 2.0f;
	
	public bool hasBeenSaved = false;

	public bool hasSubmeshes = false;
	public bool mergeSubMeshes = false;
	public bool has2ndUV = false;
	public bool delete2ndUV = false;

	public Texture2D sourceTex0;
	public Texture2D sourceTex1;
	public bool textureSplitHalf = false;
	public bool showTextureCombine = false;


	public TextureSplit TextureSplitSelected = TextureSplit.OneByOne;


	
	public bool adjustScale = false;
	public bool adjustVertexColors = false;
	public bool adjustBending = false;
	public bool editVertices = false;
	public float maxBendingValueY = 0.1f;
	public float maxBendingValueX = 0.0f;
	public AnimationCurve curvY;
	public AnimationCurve curvX;
	
	private Vector3 tempPos;
	
	public float adjustBlueValue = 0.0f;
	public float adjustGreenValue = 0.0f;
	public float adjustRedValue = 0.0f;
	public AnimationCurve RedcurvY = AnimationCurve.Linear(0,0,1,1);
	public AnimationCurve RedcurvX = AnimationCurve.Linear(0,0,1,1);
	public float adjustAlphaValue = 0.0f;
	
	static Transform currentSelection;
	static Mesh currentMesh;
	static Collider currentCollider;
	private Mesh currentColliderMesh;

	//static Mesh newMesh;
	
	public string oldFileName;
	
	//public Color[] originalColors;
		
	void Update() {
		// hide wireframe in play mode
		if(Selection.activeGameObject) {
			EditorUtility.SetSelectedWireframeHidden(Selection.activeGameObject.renderer, true);
		}
	}

	public void checkSubmeshes () {
		currentMesh = GetComponent<MeshFilter>().sharedMesh;
		if (currentMesh.subMeshCount > 1) {
			hasSubmeshes = true;
		}
		else {
			hasSubmeshes = false;
			mergeSubMeshes = false;
		}
		if (currentMesh.uv2 != null) {
			has2ndUV = true;
		}
		else {
			delete2ndUV = false;
		}
	}


	
	public void AdjustBending () {
		currentMesh = GetComponent<MeshFilter>().sharedMesh;
		currentSelection = GetComponent<Transform>();
		Vector3[] vertices = currentMesh.vertices;
		Color[] colors = currentMesh.colors;
		
		// create vertex color in case there are no
		if (colors.Length == 0) {
			colors = new Color[vertices.Length];
			for (int i = 0; i < vertices.Length; i++) {
				colors[i] = new Color(0.0f,0.0f,0.0f,1.0f);
			}
		}
		
		for (int i = 0; i < vertices.Length; i++) {
			Bounds bounds = currentMesh.bounds;
				
				if (vertices[i].y <= 0.0f) {
					colors[i].b = 0.0f;
				}
				else {
					colors[i].b = Mathf.Lerp (0.0f, maxBendingValueY, curvY.Evaluate(vertices[i].y/bounds.size.y) );
					tempPos = new Vector3 (vertices[i].x, 0.0f, vertices[i].z);
					float Length = Vector3.Distance(tempPos, new Vector3(0.0f,0.0f,0.0f) ) / ((bounds.size.x + bounds.size.z) * 0.5f);
					Length = curvX.Evaluate(Length);
					colors[i].b += Mathf.Lerp (0.0f, maxBendingValueX, Length );
				}
			
				//colors[i].r = 0.0f;
				//colors[i].g = 0.0f;
		}
		
		//// update mesh
		currentMesh.colors = colors;
		
	}
	
	public void AdjustVertexColors () {
		currentMesh = GetComponent<MeshFilter>().sharedMesh;
		currentSelection = GetComponent<Transform>();
		Vector3[] vertices = currentMesh.vertices;
		Color[] colors = currentMesh.colors; //originalColors; 
		
		// create vertex color in case there are no
		if (colors.Length == 0) {
			colors = new Color[vertices.Length];
			for (int i = 0; i < vertices.Length; i++) {
				colors[i] = new Color(0.0f,0.0f,0.0f,1.0f);
			}
		}
		
		for (int i = 0; i < vertices.Length; i++) {
			if (adjustScale) {
				// compress ambient occlusion value and add scale
				// colors[i].a = (4.0f * Mathf.Clamp(colors[i].a * 255.0f / 4.0f, 0.0f, 63.0f) + Mathf.Clamp(Scale, 0.0f, 3.0f)) / 255.0f;
			}
			if (adjustVertexColors) {
				colors[i].b = Mathf.Clamp( colors[i].b + colors[i].b/100 * adjustBlueValue, 0.0f, 1.0f);
				colors[i].g = Mathf.Clamp( colors[i].g + colors[i].g/100 * adjustGreenValue, 0.0f, 1.0f);
				colors[i].r = Mathf.Clamp( colors[i].r + colors[i].r/100 * adjustRedValue, 0.0f, 1.0f);
				colors[i].a = Mathf.Clamp( colors[i].a + colors[i].a/100 * adjustAlphaValue, 0.0f, 1.0f);
			}
			
		}
		
		//// update mesh
		currentMesh.colors = colors;
		
	}
	
	
/// 
	
	public void SaveNewPlantMesh () {
		currentMesh = GetComponent<MeshFilter>().sharedMesh;
		currentSelection = GetComponent<Transform>();
		Vector3[] vertices = currentMesh.vertices;
		Color[] colors = currentMesh.colors;
		
		// create vertex color in case there are no
		if (colors.Length == 0) {
			colors = new Color[vertices.Length];
			for (int i = 0; i < vertices.Length; i++) {
				colors[i] = new Color(0.0f,0.0f,0.0f,1.0f);
			}
			//// update mesh
			currentMesh.colors = colors;
		}
		
		
		/// reset vertex color adjustment values
		//adjustBlueValue = 0.0f;
		//adjustGreenValue = 0.0f;
		
		
		///// create a new mesh    
		Mesh newMesh = new Mesh();
		newMesh.vertices = currentMesh.vertices;
		newMesh.colors = currentMesh.colors;
		newMesh.uv = currentMesh.uv;
		if (!delete2ndUV && currentMesh.uv2 != null) {
			newMesh.uv2 = currentMesh.uv2;
		}
		newMesh.normals = currentMesh.normals;
		newMesh.tangents = currentMesh.tangents;
		//newMesh.triangles = currentMesh.triangles;

		if (currentMesh.subMeshCount == 1) {
			newMesh.triangles = currentMesh.triangles;	
		}
		
		else if (currentMesh.subMeshCount == 2 && mergeSubMeshes == false) {
			newMesh.subMeshCount = 2;
			int[] tri1 = currentMesh.GetTriangles(0);
			int[] tri2 = currentMesh.GetTriangles(1);
			newMesh.SetTriangles(tri1,0);
			newMesh.SetTriangles(tri2,1);
		}
		else if (currentMesh.subMeshCount == 2 && mergeSubMeshes == true) {
			newMesh.subMeshCount = 1;
			int[] tri1 = currentMesh.GetTriangles(0);
			int[] tri2 = currentMesh.GetTriangles(1);
			int[] triCombined = new int[tri1.Length + tri2.Length];
			int counter = 0;
			for (int i = 0; i < tri1.Length; i++) {
				triCombined[i] = tri1[i];
				counter = i;
			}
			counter += 1;
			for (int j = 0; j < tri2.Length; j++) {
				//Debug.Log (counter);
				triCombined[counter + j] = tri2[j];
			}
			newMesh.SetTriangles(triCombined,0);
		}

		///// save newMesh
		string filePath;
		if (oldFileName == "") {
			oldFileName = "AfsPlantMesh.asset";
		}
		//else {
			filePath = EditorUtility.SaveFilePanelInProject
			(
				"Save new Mesh",
				oldFileName, 
				"asset",
				"Choose a file location and name"
			);
		//}
		if (filePath!=""){
			UnityEditor.AssetDatabase.DeleteAsset(filePath);
			UnityEditor.AssetDatabase.CreateAsset(newMesh,filePath);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			oldFileName = Path.GetFileName(filePath);
		}
		
		///// assign newMesh
		currentSelection.GetComponent<MeshFilter>().sharedMesh = newMesh;
		if (currentSelection.GetComponent<MeshCollider>()) {
			currentSelection.GetComponent<MeshCollider>().sharedMesh = newMesh;
		}
		
		//// reset values
		adjustBlueValue = 0.0f;
		adjustGreenValue = 0.0f;
		adjustRedValue = 0.0f;
		adjustAlphaValue = 0.0f;
		
		hasBeenSaved = true;
		delete2ndUV = false;
		
		//originalColors = currentMesh.colors;
	}
}
#endif
