﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
	
		
		public static int chunkSize = 16;
		public int chunkX;
		public int chunkY;
		public int chunkZ;
		public bool update;

		public VoxelStream voxels;
		private List<Vector3> newVertices = new List<Vector3> ();
		private List<int> newTriangles = new List<int> ();
		private List<Vector2> newUV = new List<Vector2> ();
		private float tUnit = 0.25f;
		private Vector2 tStone = new Vector2 (0, 1);
		private Vector2 tGrass = new Vector2 (1, 1);
		private Vector2 tGrassTop = new Vector2 (1, 0);
		private Mesh mesh;
		private MeshCollider col;
		private int faceCount;
	
		// Use this for initialization
		void Start ()
		{
				mesh = GetComponent<MeshFilter> ().mesh;
				col = GetComponent<MeshCollider> ();
				GenerateMesh ();
		}
  
		public void setVoxelsToRender (VoxelStream inVoxels)
		{
				voxels = inVoxels;
				
		}
  		
		// Update is called once per frame
		void LateUpdate ()
		{
				if (update) {
						GenerateMesh ();
						update = false;
				}
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
				return voxels.GetBlockAtCoords (x + chunkX, y + chunkY, z + chunkZ);
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
  
		void UpdateMesh ()
		{
   
				mesh.Clear ();
				mesh.vertices = newVertices.ToArray ();
				mesh.uv = newUV.ToArray ();
				mesh.triangles = newTriangles.ToArray ();
				mesh.Optimize ();
				mesh.RecalculateNormals ();
   
				col.sharedMesh = null;
				col.sharedMesh = mesh;
   
				newVertices.Clear ();
				newUV.Clear ();
				newTriangles.Clear ();
				faceCount = 0;
   
		}
}