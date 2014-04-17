using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
	
		
		public static int chunkSize = 16;
		public int chunkX;
		public int chunkY;
		public int chunkZ;

		public VoxelStream voxels;
		public Dictionary<string, List<Vector2>> textureMap = new Dictionary<string, List<Vector2> > ();
		private List<Vector3> newVertices = new List<Vector3> ();
		private List<int> newTriangles = new List<int> ();
		private List<Vector2> newUV = new List<Vector2> ();
		private Mesh mesh;
		private MeshCollider col;
		private int faceCount;
	
		// Use this for initialization
		void Awake ()
		{
				mesh = GetComponent<MeshFilter> ().mesh;
				col = GetComponent<MeshCollider> ();
		}
  
		public void assignTexture (Texture2D texture)
		{
				renderer.material.mainTexture = texture;
		}
		public void makeActive ()
		{
				gameObject.SetActive (true);
				textureMap.Clear ();
				GenerateMesh ();
				
		}
		
		public void updateChunk ()
		{
				print ("updating chunk");
				GenerateMesh ();
		}
		public void setVoxelsToRender (VoxelStream inVoxels)
		{
				voxels = inVoxels;
				
		}
  
		public void GenerateMesh ()
		{
				int tFace = 0;
				for (int x=0; x<chunkSize; x++) {
						for (int y=0; y<chunkSize; y++) {
								for (int z=0; z<chunkSize; z++) {
										tFace = BlockFactory.render (this, x, y, z, newVertices, newTriangles, newUV, tFace);
								}
						}
				}
   
				//Generate items
				UpdateMesh ();
		}
  
		public byte Block (int x, int y, int z)
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
				//TangentSolver (mesh);
   
				col.sharedMesh = null;
				col.sharedMesh = mesh;
				newVertices.Clear ();
				newUV.Clear ();
				newTriangles.Clear ();
				faceCount = 0;
				
				
				
   
		}		
		
}