using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class BlockFactory
{

		private const float tUnit = 0.0625f;
		private static Texture2D texture = null;
		
		public  static int render (Chunk chunk,
		int x, int y, int z, 
		List<Vector3> newVertices, 
		List<int> newTriangles,
		List<Vector2> newUV,
		int faceCount)
		{
	
				if (texture == null) {
						GameObject chunkM = GameObject.Find ("Chunk Manager");
						ChunkManager textureManager = chunkM.GetComponent<ChunkManager> ();
						texture = textureManager.atlas;
				}
				
				if (chunk.Block (x, y, z) != AirBlock.blockId) {
						if (chunk.Block (x, y + 1, z) == 0) {
								//Block above is air
								CreateFaceTop (chunk, x, y, z, newVertices, newTriangles, newUV, faceCount);
								faceCount += 1;
						}
				
						if (chunk.Block (x, y - 1, z) == 0) {
								//Block below is air
								CreateFaceBot (chunk, x, y, z, newVertices, newTriangles, newUV, faceCount);
								faceCount += 1;
						}
				
						if (chunk.Block (x + 1, y, z) == 0) {
								//Block east is air
								CreateFaceEast (chunk, x, y, z, newVertices, newTriangles, newUV, faceCount);
								faceCount += 1;
						}
				
						if (chunk.Block (x - 1, y, z) == 0) {
								//Block west is air
								CreateFaceWest (chunk, x, y, z, newVertices, newTriangles, newUV, faceCount);
								faceCount += 1;
						}
				
						if (chunk.Block (x, y, z + 1) == 0) {
								//Block north is air
								CreateFaceNorth (chunk, x, y, z, newVertices, newTriangles, newUV, faceCount);
								faceCount += 1;
						}
				
						if (chunk.Block (x, y, z - 1) == 0) {
								//Block south is air
								CreateFaceSouth (chunk, x, y, z, newVertices, newTriangles, newUV, faceCount);
								faceCount += 1;
						}
						
				}
				return faceCount;
		}
	
		private static void CreateFaceTop (Chunk chunk,
	                                   int x, int y, int z, 
	                                   List<Vector3> newVertices, 
	                      List<int> newTriangles,
	                      List<Vector2> newUV,
	                      int faceCount)
		{
				newVertices.Add (new Vector3 (x, y, z + 1));
				newVertices.Add (new Vector3 (x + 1, y, z + 1));
				newVertices.Add (new Vector3 (x + 1, y, z));
				newVertices.Add (new Vector3 (x, y, z));
				CreateFace (chunk, x, y, z, newTriangles, newUV, faceCount);
		}
		
		private static void CreateFaceNorth (Chunk chunk,
	                        int x, int y, int z, 
	                        List<Vector3> newVertices, 
	                        List<int> newTriangles,
	                        List<Vector2> newUV,
	                        int faceCount)
		{
			
				newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
				newVertices.Add (new Vector3 (x + 1, y, z + 1));
				newVertices.Add (new Vector3 (x, y, z + 1));
				newVertices.Add (new Vector3 (x, y - 1, z + 1));
				CreateFace (chunk, x, y, z, newTriangles, newUV, faceCount);

		}
		
		private static void CreateFaceEast (Chunk chunk,
	                       int x, int y, int z, 
	                       List<Vector3> newVertices, 
	                       List<int> newTriangles,
	                       List<Vector2> newUV,
	                       int faceCount)
		{
			
				newVertices.Add (new Vector3 (x + 1, y - 1, z));
				newVertices.Add (new Vector3 (x + 1, y, z));
				newVertices.Add (new Vector3 (x + 1, y, z + 1));
				newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
				CreateFace (chunk, x, y, z, newTriangles, newUV, faceCount);
		}
		
		private static void CreateFaceSouth (Chunk chunk,
	                       int x, int y, int z, 
	                       List<Vector3> newVertices, 
	                       List<int> newTriangles,
	                       List<Vector2> newUV,
	                       int faceCount)
		{
			
				newVertices.Add (new Vector3 (x, y - 1, z));
				newVertices.Add (new Vector3 (x, y, z));
				newVertices.Add (new Vector3 (x + 1, y, z));
				newVertices.Add (new Vector3 (x + 1, y - 1, z));
				CreateFace (chunk, x, y, z, newTriangles, newUV, faceCount);
		}
		
		private static void CreateFaceWest (Chunk chunk,
	                       int x, int y, int z, 
	                       List<Vector3> newVertices, 
	                       List<int> newTriangles,
	                       List<Vector2> newUV,
	                       int faceCount)
		{
				newVertices.Add (new Vector3 (x, y - 1, z + 1));
				newVertices.Add (new Vector3 (x, y, z + 1));
				newVertices.Add (new Vector3 (x, y, z));
				newVertices.Add (new Vector3 (x, y - 1, z));
				CreateFace (chunk, x, y, z, newTriangles, newUV, faceCount);
		}
		
		private static void CreateFaceBot (Chunk chunk,
	                      int x, int y, int z, 
	                      List<Vector3> newVertices, 
	                      List<int> newTriangles,
	                      List<Vector2> newUV,
	                      int faceCount)
		{
			
				newVertices.Add (new Vector3 (x, y - 1, z));
				newVertices.Add (new Vector3 (x + 1, y - 1, z));
				newVertices.Add (new Vector3 (x + 1, y - 1, z + 1));
				newVertices.Add (new Vector3 (x, y - 1, z + 1));
				CreateFace (chunk, x, y, z, newTriangles, newUV, faceCount);
		}
		
		private static void CreateFace (Chunk chunk, int x, int y, int z, List<int> newTriangles, List<Vector2> newUV, int faceCount)
		{
			
			
				newTriangles.Add (faceCount * 4); //1
				newTriangles.Add (faceCount * 4 + 1); //2
				newTriangles.Add (faceCount * 4 + 2); //3
				newTriangles.Add (faceCount * 4); //1
				newTriangles.Add (faceCount * 4 + 2); //3
				newTriangles.Add (faceCount * 4 + 3); //4
			
				string key = getHash (x, y, z);
				if (chunk.textureMap.ContainsKey (key)) {
						List<Vector2> myTexture = chunk.textureMap [key];
						newUV.Add (myTexture [0]);
						newUV.Add (myTexture [1]);
						newUV.Add (myTexture [2]);
						newUV.Add (myTexture [3]);
		
				} else {
						Vector2 textureStart = GetTexture (chunk.Block (x, y, z));
						Vector2 texturePos = textureStart + new Vector2 (Random.Range (0f, 7f), Random.Range (0f, 7f));
						List<Vector2> newTextures = new List<Vector2> (4);
						newTextures.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
						newTextures.Add (new Vector2 (tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
						newTextures.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
						newTextures.Add (new Vector2 (tUnit * texturePos.x, tUnit * texturePos.y));
						
						newUV.Add (newTextures [0]);
						newUV.Add (newTextures [1]);
						newUV.Add (newTextures [2]); 
						newUV.Add (newTextures [3]);
						
						chunk.textureMap.Add (getHash (x, y, z), newTextures);
				}
				
				
		}

		private static Vector2 GetTexture (int block)
		{
				if (block == DirtBlock.blockId) {
						return DirtBlock.textureId;
				} else if (block == GrassBlock.blockId) {
						return GrassBlock.textureId;
				} else {
						return Vector2.zero;  //default is dirt texture.  TODO replace with an invalid texture.
				}
		}
		private  static string getHash (int x, int y, int z)
		{
				return x + "," + y + "," + z;
		}
}
