using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureAtlasTest : MonoBehaviour
{
		public List<Texture2D> loadedTextures;
		public Texture2D atlas;
		public Rect[] rects;
		
		private List<Vector3> newVertices = new List<Vector3> ();
		private List<int> newTriangles = new List<int> ();
		private List<Vector2> newUV = new List<Vector2> ();
		public Vector2 textureStart = new Vector2 (0, 0);
		private Mesh mesh;
		private MeshCollider col;
		private int faceCount;

		// Use this for initialization
		void Start ()
		{
				mesh = GetComponent<MeshFilter> ().mesh;
				col = GetComponent<MeshCollider> ();
				Texture2D[] textures = Resources.LoadAll<Texture2D> ("Terrain Textures");
				for (int i = 0; i < textures.Length; i++) {
						loadedTextures.Add (textures [i]);
				}
				
				Texture2D atlas = new Texture2D (8192, 8192);
				rects = atlas.PackTextures (loadedTextures.ToArray (), 0, 8192);
				renderer.material.mainTexture = atlas;
		}
	
		private const float tUnit = 0.0625f;
	
	
		public   int render ()
		{
				CreateFaceTop ();
				faceCount += 1;

				CreateFaceBot ();
				faceCount += 1;

				CreateFaceEast ();
				faceCount += 1;

				CreateFaceWest ();
				faceCount += 1;

				CreateFaceNorth ();
				faceCount += 1;

				CreateFaceSouth ();
				faceCount += 1;
			
				return faceCount;
		}
	
		private  void CreateFaceTop ()
		{
				newVertices.Add (new Vector3 (0, 0, 1));
				newVertices.Add (new Vector3 (1, 0, 1));
				newVertices.Add (new Vector3 (1, 0, 0));
				newVertices.Add (new Vector3 (0, 0, 0));
				CreateFace ();
		}
	
		private  void CreateFaceNorth ()
		{
		
				newVertices.Add (new Vector3 (0 + 1, 0 - 1, 0 + 1));
				newVertices.Add (new Vector3 (0 + 1, 0, 0 + 1));
				newVertices.Add (new Vector3 (0, 0, 0 + 1));
				newVertices.Add (new Vector3 (0, 0 - 1, 0 + 1));
				CreateFace ();
		
		}
	
		private  void CreateFaceEast ()
		{
		
				newVertices.Add (new Vector3 (0 + 1, 0 - 1, 0));
				newVertices.Add (new Vector3 (0 + 1, 0, 0));
				newVertices.Add (new Vector3 (0 + 1, 0, 0 + 1));
				newVertices.Add (new Vector3 (0 + 1, 0 - 1, 0 + 1));
				CreateFace ();
		}
	
		private  void CreateFaceSouth ()
		{
		
				newVertices.Add (new Vector3 (0, 0 - 1, 0));
				newVertices.Add (new Vector3 (0, 0, 0));
				newVertices.Add (new Vector3 (0 + 1, 0, 0));
				newVertices.Add (new Vector3 (0 + 1, 0 - 1, 0));
				CreateFace ();
		}
	
		private  void CreateFaceWest ()
		{
				newVertices.Add (new Vector3 (0, 0 - 1, 0 + 1));
				newVertices.Add (new Vector3 (0, 0, 0 + 1));
				newVertices.Add (new Vector3 (0, 0, 0));
				newVertices.Add (new Vector3 (0, 0 - 1, 0));
				CreateFace ();
		}
	
		private  void CreateFaceBot ()
		{
		
				newVertices.Add (new Vector3 (0, 0 - 1, 0));
				newVertices.Add (new Vector3 (0 + 1, 0 - 1, 0));
				newVertices.Add (new Vector3 (0 + 1, 0 - 1, 0 + 1));
				newVertices.Add (new Vector3 (0, 0 - 1, 0 + 1));
				CreateFace ();
		}
	
		private  void CreateFace ()
		{
		
		
				newTriangles.Add (faceCount * 4); //1
				newTriangles.Add (faceCount * 4 + 1); //2
				newTriangles.Add (faceCount * 4 + 2); //3
				newTriangles.Add (faceCount * 4); //1
				newTriangles.Add (faceCount * 4 + 2); //3
				newTriangles.Add (faceCount * 4 + 3); //4
		

				//Vector2 textureStart = new Vector2 (0, 0);
				Vector2 texturePos = textureStart;// + new Vector2 (Random.Range (0, 7), Random.Range (0, 7));
				List<Vector2> newTextures = new List<Vector2> (4);
				newTextures.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
				newTextures.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
				newTextures.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
				newTextures.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y));
			
				newUV.Add (newTextures [0]);
				newUV.Add (newTextures [1]);
				newUV.Add (newTextures [2]); 
				newUV.Add (newTextures [3]);

		
		
		}
		// Update is called once per frame
		void Update ()
		{
				render ();
				UpdateMesh ();
		}
		
		private void UpdateMesh ()
		{
		
				mesh.Clear ();
				mesh.vertices = newVertices.ToArray ();
				mesh.uv = newUV.ToArray ();
				mesh.triangles = newTriangles.ToArray ();
				mesh.Optimize ();
				mesh.RecalculateNormals ();
				//TangentSolver (mesh);
		
				col.sharedMesh = null;
				col.sharedMesh = mesh;
				newVertices.Clear ();
				newUV.Clear ();
				newTriangles.Clear ();
				faceCount = 0;
		
		
		}		
}
