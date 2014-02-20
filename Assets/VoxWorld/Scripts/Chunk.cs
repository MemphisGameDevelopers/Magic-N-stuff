using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
	
		
		public static int chunkSize = 32;
		public int chunkX;
		public int chunkY;
		public int chunkZ;
		public bool update;

		public VoxelStream voxels;
		private List<Vector3> newVertices = new List<Vector3> ();
		private List<int> newTriangles = new List<int> ();
		private List<Vector2> newUV = new List<Vector2> ();
		private float tUnit = 0.0625f;
		private Vector2 tStone = new Vector2 (0, 1);
		private Vector2 tGrass = new Vector2 (1, 1);
		private Vector2 tGrassTop = new Vector2 (1, 0);
		private Mesh mesh;
		private MeshCollider col;
		private int faceCount;
	
		// Use this for initialization
		void Awake ()
		{
				mesh = GetComponent<MeshFilter> ().mesh;
				col = GetComponent<MeshCollider> ();
				//GenerateMesh ();
		}
  
		public void setVoxelsToRender (VoxelStream inVoxels)
		{
				voxels = inVoxels;
				
		}
  
		public void GenerateMesh ()
		{
   
				for (int x=0; x<chunkSize; x++) {
						for (int y=0; y<chunkSize; y++) {
								for (int z=0; z<chunkSize; z++) {
										//This code will run for every block in the chunk
      
										if (Block (x, y, z) != 0) {
												if (Block (x, y + 1, z) == 0) {
														//Block above is air
														CubeTop (x, y, z, Block (x, y, z));
												}
       
												if (Block (x, y - 1, z) == 0) {
														//Block below is air
														CubeBot (x, y, z, Block (x, y, z));
        
												}
       
												if (Block (x + 1, y, z) == 0) {
														//Block east is air
														CubeEast (x, y, z, Block (x, y, z));
        
												}
       
												if (Block (x - 1, y, z) == 0) {
														//Block west is air
														CubeWest (x, y, z, Block (x, y, z));
        
												}
       
												if (Block (x, y, z + 1) == 0) {
														//Block north is air
														CubeNorth (x, y, z, Block (x, y, z));
        
												}
       
												if (Block (x, y, z - 1) == 0) {
														//Block south is air
														CubeSouth (x, y, z, Block (x, y, z));
        
												}
       
										}
      
								}
						}
				}
   
				//Generate items
				UpdateMesh ();
		}
  
		private byte Block (int x, int y, int z)
		{
				return voxels.GetBlockAtRelativeCoords (x + chunkX, y + chunkY, z + chunkZ);
		}
  
		private float[,,] Blocks ()
		{
				float[,,] blocks = new float[chunkSize, chunkSize, chunkSize];
				for (int x=0; x<chunkSize; x++) {
						for (int y=0; y<chunkSize; y++) {
								for (int z=0; z<chunkSize; z++) {
										blocks [x, y, z] = (float)Block (x, y, z);
								}
						}
				}
				return blocks;
		}
		void CubeTop (int x, int y, int z, byte block)
		{
   
				newVertices.Add (new Vector3 (x, y, z + 1));
				newVertices.Add (new Vector3 (x + 1, y, z + 1));
				newVertices.Add (new Vector3 (x + 1, y, z));
				newVertices.Add (new Vector3 (x, y, z));
   
				Vector2 texturePos = new Vector2 (0, 0);
   
				if (Block (x, y, z) == 1) {
						texturePos = tStone;
				} else if (Block (x, y, z) == 2) {
						texturePos = tGrassTop;
				}
   
				Cube (texturePos);
   
		}
  
		void CubeNorth (int x, int y, int z, byte block)
		{
   
				newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
				newVertices.Add (new Vector3 (x + 1, y, z + 1));
				newVertices.Add (new Vector3 (x, y, z + 1));
				newVertices.Add (new Vector3 (x, y - 1, z + 1));
   
				Vector2 texturePos = new Vector2 (0, 0);
   
				if (Block (x, y, z) == 1) {
						texturePos = tStone;
				} else if (Block (x, y, z) == 2) {
						texturePos = tGrass;
				}
   
				Cube (texturePos);
   
		}
  
		void CubeEast (int x, int y, int z, byte block)
		{
   
				newVertices.Add (new Vector3 (x + 1, y - 1, z));
				newVertices.Add (new Vector3 (x + 1, y, z));
				newVertices.Add (new Vector3 (x + 1, y, z + 1));
				newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
   
				Vector2 texturePos = new Vector2 (0, 0);
   
				if (Block (x, y, z) == 1) {
						texturePos = tStone;
				} else if (Block (x, y, z) == 2) {
						texturePos = tGrass;
				}
   
				Cube (texturePos);
   
		}
  
		void CubeSouth (int x, int y, int z, byte block)
		{
   
				newVertices.Add (new Vector3 (x, y - 1, z));
				newVertices.Add (new Vector3 (x, y, z));
				newVertices.Add (new Vector3 (x + 1, y, z));
				newVertices.Add (new Vector3 (x + 1, y - 1, z));
   
				Vector2 texturePos = new Vector2 (0, 0);
   
				if (Block (x, y, z) == 1) {
						texturePos = tStone;
				} else if (Block (x, y, z) == 2) {
						texturePos = tGrass;
				}
   
				Cube (texturePos);
   
		}
  
		void CubeWest (int x, int y, int z, byte block)
		{
   
				newVertices.Add (new Vector3 (x, y - 1, z + 1));
				newVertices.Add (new Vector3 (x, y, z + 1));
				newVertices.Add (new Vector3 (x, y, z));
				newVertices.Add (new Vector3 (x, y - 1, z));
   
				Vector2 texturePos = new Vector2 (0, 0);
   
				if (Block (x, y, z) == 1) {
						texturePos = tStone;
				} else if (Block (x, y, z) == 2) {
						texturePos = tGrass;
				}
   
				Cube (texturePos);
   
		}
  
		void CubeBot (int x, int y, int z, byte block)
		{
   
				newVertices.Add (new Vector3 (x, y - 1, z));
				newVertices.Add (new Vector3 (x + 1, y - 1, z));
				newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
				newVertices.Add (new Vector3 (x, y - 1, z + 1));
   
				Vector2 texturePos = new Vector2 (0, 0);
   
				if (Block (x, y, z) == 1) {
						texturePos = tStone;
				} else if (Block (x, y, z) == 2) {
						texturePos = tGrass;
				}
   
				Cube (texturePos);
   
		}
  
		void Cube (Vector2 texturePos)
		{
				//override texture pos for now TODO
				texturePos = new Vector2 (Random.Range (0, 16), Random.Range (0, 16));
				
				newTriangles.Add (faceCount * 4); //1
				newTriangles.Add (faceCount * 4 + 1); //2
				newTriangles.Add (faceCount * 4 + 2); //3
				newTriangles.Add (faceCount * 4); //1
				newTriangles.Add (faceCount * 4 + 2); //3
				newTriangles.Add (faceCount * 4 + 3); //4
   
				newUV.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
				newUV.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
				newUV.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
				newUV.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y));
   
				faceCount++; // Add this line
		}
  
		public void clearMesh ()
		{
				if (mesh != null) {
						mesh.Clear ();
				}
		}
		private void UpdateMesh ()
		{
				mesh.Clear ();
				mesh.vertices = newVertices.ToArray ();
				mesh.uv = newUV.ToArray ();
				mesh.triangles = newTriangles.ToArray ();
				mesh.Optimize ();
				mesh.RecalculateNormals ();
				TangentSolver (mesh);
   
				col.sharedMesh = null;
				col.sharedMesh = mesh;
				newVertices.Clear ();
				newUV.Clear ();
				newTriangles.Clear ();
				faceCount = 0;
				
   
		}
		
		private static void TangentSolver (Mesh theMesh)
		{
				int vertexCount = theMesh.vertexCount;
				Vector3[] vertices = theMesh.vertices;
				Vector3[] normals = theMesh.normals;
				Vector2[] texcoords = theMesh.uv;
				int[] triangles = theMesh.triangles;
				int triangleCount = triangles.Length / 3;
				Vector4[] tangents = new Vector4[vertexCount];
				Vector3[] tan1 = new Vector3[vertexCount];
				Vector3[] tan2 = new Vector3[vertexCount];
				int tri = 0;
				for (int i = 0; i < (triangleCount); i++) {
						int i1 = triangles [tri];
						int i2 = triangles [tri + 1];
						int i3 = triangles [tri + 2];
			
						Vector3 v1 = vertices [i1];
						Vector3 v2 = vertices [i2];
						Vector3 v3 = vertices [i3];
			
						Vector2 w1 = texcoords [i1];
						Vector2 w2 = texcoords [i2];
						Vector2 w3 = texcoords [i3];
			
						float x1 = v2.x - v1.x;
						float x2 = v3.x - v1.x;
						float y1 = v2.y - v1.y;
						float y2 = v3.y - v1.y;
						float z1 = v2.z - v1.z;
						float z2 = v3.z - v1.z;
			
						float s1 = w2.x - w1.x;
						float s2 = w3.x - w1.x;
						float t1 = w2.y - w1.y;
						float t2 = w3.y - w1.y;
			
						float r = 1.0f / (s1 * t2 - s2 * t1);
						Vector3 sdir = new Vector3 ((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
						Vector3 tdir = new Vector3 ((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
						tan1 [i1] += sdir;
						tan1 [i2] += sdir;
						tan1 [i3] += sdir;
			
						tan2 [i1] += tdir;
						tan2 [i2] += tdir;
						tan2 [i3] += tdir;
			
						tri += 3;
				}
		
				for (int i = 0; i < (vertexCount); i++) {
						Vector3 n = normals [i];
						Vector3 t = tan1 [i];
			
						// Gram-Schmidt orthogonalize
						Vector3.OrthoNormalize (ref n, ref t);
			
						tangents [i].x = t.x;
						tangents [i].y = t.y;
						tangents [i].z = t.z;
			
						// Calculate handedness
						tangents [i].w = (Vector3.Dot (Vector3.Cross (n, t), tan2 [i]) < 0.0) ? -1.0f : 1.0f;
				}
				theMesh.tangents = tangents;
		}
		
		public bool hasMesh ()
		{
				if (mesh == null) {
						return false;
				} else {
						return true;
				}
				
		}
}