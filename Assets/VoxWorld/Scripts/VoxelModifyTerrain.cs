using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelModifyTerrain : MonoBehaviour
{
	

		GameObject cameraGO;
		
		private GameObject player;
		private ChunkManager chunkManager;
		private Vector3 lastPlayerPosition;
		
		public VoxelWorld world;
		public Region myRegion = null;
		public int distToLoad = 4;
		public int heightToLoad = 4;
		public int depthToLoad = 4;
		public int distToUnload = 8;
		public bool saveLevel = false;
		
		private bool chunksLoaded = false;
		void Awake ()
		{
				cameraGO = GameObject.FindGameObjectWithTag ("MainCamera");
				player = GameObject.FindGameObjectWithTag ("Player");
				lastPlayerPosition = player.transform.position;
				GameObject chunkManagerGO = GameObject.Find ("Chunk Manager");
				chunkManager = chunkManagerGO.GetComponent<ChunkManager> ();
				
		}

		public void setStartRegion (Region region)
		{
				myRegion = region;
		}

		private void determinePlayerRegion (Vector3 playerPos)
		{

				if (playerPos.x >= myRegion.getBlockOffsetX () + myRegion.regionXZ && 
						playerPos.z < myRegion.getBlockOffsetZ () + myRegion.regionXZ) {
						myRegion = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetY, myRegion.offsetZ);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.z >= myRegion.getBlockOffsetZ () + myRegion.regionXZ && 
						playerPos.x < myRegion.getBlockOffsetX () + myRegion.regionXZ) {
						myRegion = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetY, myRegion.offsetZ + 1);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.x < myRegion.getBlockOffsetX () && 
						playerPos.z >= myRegion.getBlockOffsetZ ()) {	
						myRegion = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetY, myRegion.offsetZ);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.z < myRegion.getBlockOffsetZ () && 
						playerPos.x >= myRegion.getBlockOffsetX ()) {		
						myRegion = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetY, myRegion.offsetZ - 1);
						world.changeFocusRegion (myRegion);
				}
		}
	
		private void RenderColumn (Vector3 playerPos, Region region, int x, int z)
		{
				if (region.chunks [x, 0, z] == null) {
						//Generate 0 to height
						int yi = 0;
						int regionY = region.regionY / Chunk.chunkSize;
						for (int y=0; y<heightToLoad; y++) {

								if (y >= regionY) {
										//Go to the next region north
										region = world.getRegionAtIndex (region.offsetX, region.offsetY + 1, region.offsetZ);
										regionY = region.getBlockOffsetY () / Chunk.chunkSize + region.regionY / Chunk.chunkSize;
										yi = 0;
								}
						
								//Debug.Log ("creating chunk at " + x + "," + yi + "," + z);
								region.loadChunk (x, yi, z);
								yi += 1;
						}
				}
		}
	
		private void UnloadColumn (Vector3 playerPos, Region region, int x, int z)
		{
				
				if (region.chunks [x, 0, z] != null) {
						//Generate 0 to height
						int yi = 0;
						int regionY = region.regionY / Chunk.chunkSize;
						for (int y=0; y<heightToLoad; y++) {
								if (y >= regionY) {
										//Go to the next region north
										region = world.getRegionAtIndex (region.offsetX, region.offsetY + 1, region.offsetZ);
										regionY = region.getBlockOffsetY () / Chunk.chunkSize + region.regionY / Chunk.chunkSize;
										yi = 0;
								}
								region.unloadChunk (x, yi, z);
								yi += 1;
						}
				}
		}
		
//		private void genRegionColumn (Region region, int x, int z)
//		{
//				if (region.chunks [x, 0, z] == null) {
//						region.GenColumn (x, z);
//				}
//		}
//
//		private void destroyRegionColumn (Region region, int x, int z)
//		{
//				if (region.chunks [x, 0, z] != null) {
//						region.UnloadColumn (x, z);
//				}
//		}
	
//		private void MoveChunks (Vector3 playerPos)
//		{
//				int newChunkx = (Mathf.FloorToInt (playerPos.x) - myRegion.getBlockOffsetX ()) / Chunk.chunkSize;
//				int newChunkz = (Mathf.FloorToInt (playerPos.z) - myRegion.getBlockOffsetZ ()) / Chunk.chunkSize;
//				
//				int oldChunkx = (Mathf.FloorToInt (lastPlayerPosition.x) - myRegion.getBlockOffsetX ()) / Chunk.chunkSize;
//				int oldChunkz = (Mathf.FloorToInt (lastPlayerPosition.z) - myRegion.getBlockOffsetZ ()) / Chunk.chunkSize;
//
//				int chunkChangeX = newChunkx - oldChunkx;
//				int chunkChangeZ = newChunkz - oldChunkz;
//				
//				int x_start = 0;
//				int x_end = 0;
//				if (chunkChangeX == 0) {
//						x_start = newChunkx - distToLoad;
//						x_end = newChunkx + distToLoad;
//				} else if (chunkChangeX > 0) {
//						x_start = oldChunkx + distToLoad;
//						x_end = newChunkx + distToLoad;
//				} else {
//						x_start = newChunkx - distToLoad;
//						x_end = oldChunkx - distToLoad;
//				}
//				
//				int z_start = 0;
//				int z_end = 0;
//				if (chunkChangeZ == 0) {
//						z_start = newChunkz - distToLoad;
//						z_end = newChunkz + distToLoad;
//				} else if (chunkChangeZ > 0) {
//						z_start = oldChunkz + distToLoad;
//						z_end = newChunkz + distToLoad;
//				} else {
//						z_start = newChunkz - distToLoad;
//						z_end = oldChunkz - distToLoad;
//				}
//				
//				//Debug.Log ("x_start:" + x_start + ", x_end:" + x_end + ", z_start:" + z_start + ", z_end:" + z_end);
//				for (int x = x_start; x < x_start + chunkChangeX; x++) {
//						for (int z = z_start; z <= z_end; z++) {
//								loadChunkAt (x, z);
//						}
//				}
//				for (int z = z_start; z < z_start + chunkChangeZ; z++) {
//						for (int x = x_start; x <= x_end; x++) {
//								loadChunkAt (x, z);
//						}
//				}
//				
//				
//				
//		}
		
		private void loadChunkAt (Region region, int x, int z)
		{
				if (region == null) {
						return;
				}
				if (x >= 0 && z >= 0 && x < region.chunks.GetLength (0) && z < region.chunks.GetLength (2)) {
						//genRegionColumn (region, x, z);
						RenderColumn (lastPlayerPosition, region, x, z);
				} else if (x < 0 && z < 0) {
						//southwest
						Region nextRegion = world.getRegionAtIndex (region.offsetX - 1, region.offsetY, region.offsetZ - 1);
						int newX = region.regionXZ / Chunk.chunkSize + x;
						int newZ = region.regionXZ / Chunk.chunkSize + z;
						loadChunkAt (nextRegion, newX, newZ);
				} else if (x >= region.chunks.GetLength (0) && z >= region.chunks.GetLength (2)) {
						//northeast
						Region nextRegion = world.getRegionAtIndex (region.offsetX + 1, region.offsetY, region.offsetZ + 1);
						int newX = x - region.regionXZ / Chunk.chunkSize;
						int newZ = z - region.regionXZ / Chunk.chunkSize;
						loadChunkAt (nextRegion, newX, newZ);
				} else if (z < 0 && x >= region.chunks.GetLength (0)) {
						//southeast
						Region nextRegion = world.getRegionAtIndex (region.offsetX + 1, region.offsetY, region.offsetZ - 1);
						int newX = x - region.regionXZ / Chunk.chunkSize;
						int newZ = region.regionXZ / Chunk.chunkSize + z;
						loadChunkAt (nextRegion, newX, newZ);
				} else if (z >= region.chunks.GetLength (2) && x < 0) {
						//northwest
						Region nextRegion = world.getRegionAtIndex (region.offsetX - 1, region.offsetY, region.offsetZ + 1);
						int newX = region.regionXZ / Chunk.chunkSize + x;
						int newZ = z - region.regionXZ / Chunk.chunkSize;
						loadChunkAt (nextRegion, newX, newZ);
				} else if (z < 0 && x >= 0) {
						//south
						Region nextRegion = world.getRegionAtIndex (region.offsetX, region.offsetY, region.offsetZ - 1);
						int newZ = region.regionXZ / Chunk.chunkSize + z;
						loadChunkAt (nextRegion, x, newZ);
				} else if (x < 0 && z >= 0) {
						//west
						Region nextRegion = world.getRegionAtIndex (region.offsetX - 1, region.offsetY, region.offsetZ);
						int newX = region.regionXZ / Chunk.chunkSize + x;
						loadChunkAt (nextRegion, newX, z);
				} else if (x >= region.chunks.GetLength (0) && z < region.chunks.GetLength (2)) {
						//east
						Region nextRegion = world.getRegionAtIndex (region.offsetX + 1, region.offsetY, region.offsetZ);
						int newX = x - region.regionXZ / Chunk.chunkSize;
						loadChunkAt (nextRegion, newX, z);
				} else if (z >= region.chunks.GetLength (2) && x >= 0) {
						//north
						Region nextRegion = world.getRegionAtIndex (region.offsetX, region.offsetY, region.offsetZ + 1);
						int newZ = z - region.regionXZ / Chunk.chunkSize;
						loadChunkAt (nextRegion, x, newZ);
				}
		}
		
		private void unLoadChunkAt (Region region, int x, int z)
		{
				if (region == null) {
						return;
				}
				if (x >= 0 && z >= 0 && x < region.chunks.GetLength (0) && z < region.chunks.GetLength (2)) {
						//destroyRegionColumn (region, x, z);
						UnloadColumn (lastPlayerPosition, region, x, z);
				} else if (x < 0 && z < 0) {
						//southwest
						Region newRegion = world.getRegionAtIndex (region.offsetX - 1, region.offsetY, region.offsetZ - 1);
						int newX = region.regionXZ / Chunk.chunkSize + x;
						int newZ = region.regionXZ / Chunk.chunkSize + z;
						unLoadChunkAt (newRegion, newX, newZ);
				} else if (x >= region.chunks.GetLength (0) && z >= region.chunks.GetLength (2)) {
						//northeast
						Region newRegion = world.getRegionAtIndex (region.offsetX + 1, region.offsetY, region.offsetZ + 1);
						int newX = x - region.regionXZ / Chunk.chunkSize;
						int newZ = z - region.regionXZ / Chunk.chunkSize;
						unLoadChunkAt (newRegion, newX, newZ);
				} else if (z < 0 && x >= region.chunks.GetLength (0)) {
						//southeast
						Region newRegion = world.getRegionAtIndex (region.offsetX + 1, region.offsetY, region.offsetZ - 1);
						int newX = x - region.regionXZ / Chunk.chunkSize;
						int newZ = region.regionXZ / Chunk.chunkSize + z;
						unLoadChunkAt (newRegion, newX, newZ);
				} else if (z >= region.chunks.GetLength (2) && x < 0) {
						//northwest
						Region newRegion = world.getRegionAtIndex (region.offsetX - 1, region.offsetY, region.offsetZ + 1);
						int newX = region.regionXZ / Chunk.chunkSize + x;
						int newZ = z - region.regionXZ / Chunk.chunkSize;
						unLoadChunkAt (newRegion, newX, newZ);
				} else if (z < 0 && x >= 0) {
						//south
						Region newRegion = world.getRegionAtIndex (region.offsetX, region.offsetY, region.offsetZ - 1);
						int newZ = region.regionXZ / Chunk.chunkSize + z;
						unLoadChunkAt (newRegion, x, newZ);
				} else if (x < 0 && z >= 0) {
						//west
						Region newRegion = world.getRegionAtIndex (region.offsetX - 1, region.offsetY, region.offsetZ);
						int newX = region.regionXZ / Chunk.chunkSize + x;
						unLoadChunkAt (newRegion, newX, z);
				} else if (x >= region.chunks.GetLength (0) && z < region.chunks.GetLength (2)) {
						//east
						Region newRegion = world.getRegionAtIndex (region.offsetX + 1, region.offsetY, region.offsetZ);
						int newX = x - region.regionXZ / Chunk.chunkSize;
						unLoadChunkAt (newRegion, newX, z);
				} else if (z >= region.chunks.GetLength (2) && x >= 0) {
						//north
						Region newRegion = world.getRegionAtIndex (region.offsetX, region.offsetY, region.offsetZ + 1);
						int newZ = z - region.regionXZ / Chunk.chunkSize;
						unLoadChunkAt (newRegion, x, newZ);
				}
		}
		
		private void LoadChunks (Vector3 playerPos)
		{
		
				int playerChunkx = (Mathf.FloorToInt (playerPos.x) - myRegion.getBlockOffsetX ()) / Chunk.chunkSize;
				int playerChunkz = (Mathf.FloorToInt (playerPos.z) - myRegion.getBlockOffsetZ ()) / Chunk.chunkSize;
				int x_start = playerChunkx - distToUnload - 2;
				int x_finish = playerChunkx + distToUnload + 2;
				int z_start = playerChunkz - distToUnload - 2;
				int z_finish = playerChunkz + distToUnload + 2;
		
				LinkedList<Vector2> chunksToLoad = new LinkedList<Vector2> ();
				LinkedList<Vector2> chunksToUnload = new LinkedList<Vector2> ();
				for (int x = x_start; x < x_finish; x++) {
						for (int z = z_start; z < z_finish; z++) {
								float dist = Vector2.Distance (new Vector2 (x, z), new Vector2 (playerChunkx, playerChunkz));
				
								if (dist <= distToLoad) {
										chunksToLoad.AddLast (new Vector2 (x, z));
								} else if (dist > distToUnload) {
										chunksToUnload.AddLast (new Vector2 (x, z));
								}
						}
				}
				
				foreach (Vector2 vector in chunksToLoad) {
						loadChunkAt (myRegion, (int)vector.x, (int)vector.y);
				}
				foreach (Vector2 vector in chunksToUnload) {
						unLoadChunkAt (myRegion, (int)vector.x, (int)vector.y);
				}

		}
	
		public void ReplaceBlockCenter (float range, byte block)
		{
				//Replaces the block directly in front of the player
		
				Ray ray = new Ray (cameraGO.transform.position, cameraGO.transform.forward);
				RaycastHit hit;
		
				if (Physics.Raycast (ray, out hit)) {
			
						if (hit.distance < range) {
								ReplaceBlockAt (hit, block);
						}
				}
		
		}
	
		public void AddBlockCenter (float range, byte block)
		{
				//Adds the block specified directly in front of the player
		
				Ray ray = new Ray (cameraGO.transform.position, cameraGO.transform.forward);
				RaycastHit hit;
		
				if (Physics.Raycast (ray, out hit)) {
			
						if (hit.distance < range) {
								AddBlockAt (hit, block);
						}
						Debug.DrawLine (ray.origin, ray.origin + (ray.direction * hit.distance), Color.green, 2);
				}
		
		}
	
		public void ReplaceBlockCursor (byte block)
		{
				//Replaces the block specified where the mouse cursor is pointing
		
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
		
				if (Physics.Raycast (ray, out hit)) {
			
						ReplaceBlockAt (hit, block);
						Debug.DrawLine (ray.origin, ray.origin + (ray.direction * hit.distance),
			                Color.green, 2);
			
				}
		}
	
		public void AddBlockCursor (byte block)
		{
				//Adds the block specified where the mouse cursor is pointing
		
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
		
				if (Physics.Raycast (ray, out hit)) {
			
						AddBlockAt (hit, block);
						Debug.DrawLine (ray.origin, ray.origin + (ray.direction * hit.distance),
			                Color.green, 2);
				}
		}
	
		public void ReplaceBlockAt (RaycastHit hit, byte block)
		{
				//removes a block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
				Vector3 position = hit.point;
				position += (hit.normal * -0.5f);
				
				//Need to reduce this position to a region local position.
				SetBlockAt (position, block);
		}
	
		public void AddBlockAt (RaycastHit hit, byte block)
		{
				//adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
				Vector3 position = hit.point;
				position += (hit.normal * 0.5f);
		
				SetBlockAt (position, block);
		}
	
		private void SetBlockAt (Vector3 position, byte block)
		{
				//sets the specified block at these coordinates
		
				int x = Mathf.RoundToInt (position.x);
				int y = Mathf.RoundToInt (position.y);
				int z = Mathf.RoundToInt (position.z);


		
				SetBlockAt (x, y, z, block);
		}
	
		private void SetBlockAt (int x, int y, int z, byte block)
		{
				//Block could be part of the neighbor region.
				//adds the specified block at these coordinates

				Region modRegion = world.getRegionAtCoords (x, y, z);
				int[] localCoords = modRegion.convertWorldToLocal (x, y, z);
				//print ("Set block at world(" + x + "," + y + "," + z + ") local(" + localCoords [0] + "," + localCoords [1] + "," + localCoords [2] + ")");
				modRegion.data [localCoords [0], localCoords [1], localCoords [2]] = block;
				UpdateChunkAt (modRegion, localCoords [0], localCoords [1], localCoords [2]);
		}

		private void UpdateChunkAt (Region region, int x, int y, int z)
		{
				//Updates the chunk containing this block
				int updateX = Mathf.FloorToInt (x / Chunk.chunkSize);
				int updateY = Mathf.FloorToInt (y / Chunk.chunkSize);
				int updateZ = Mathf.FloorToInt (z / Chunk.chunkSize);
		
				//Update the chunk's mesh
				chunkManager.flagChunkForUpdate (region.chunks [updateX, updateY, updateZ]);
				//region.chunks [updateX, updateY, updateZ].update = true;
				
				//Region has changed state.
				region.isDirty = true;
		
				//Neighbor chunks also may need to be re-rendered since their mesh could have
				//potentially changed.
				int xEdge = x - (Chunk.chunkSize * updateX);
				int yEdge = y - (Chunk.chunkSize * updateY);
				int zEdge = z - (Chunk.chunkSize * updateZ);
		
				if (xEdge == 0) {
						region.flagChunkForUpdate (updateX - 1, updateY, updateZ);
				}
		
				if (xEdge == Chunk.chunkSize - 1) {
						region.flagChunkForUpdate (updateX + 1, updateY, updateZ);
				}
		
				if (yEdge == 0) {
						region.flagChunkForUpdate (updateX, updateY - 1, updateZ);
				}
		
				if (yEdge == Chunk.chunkSize - 1) {
						region.flagChunkForUpdate (updateX, updateY + 1, updateZ);
				}
		
				if (zEdge == 0) {
						region.flagChunkForUpdate (updateX, updateY, updateZ - 1);
				}
		
				if (zEdge == Chunk.chunkSize - 1) {
						region.flagChunkForUpdate (updateX, updateY, updateZ + 1);
				}
		
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (saveLevel) {
						world.saveWorld ();
						saveLevel = false;
				}
				if (myRegion != null) {
						if (!chunksLoaded) {
								LoadChunks (player.transform.position);
								determinePlayerRegion (player.transform.position);
								lastPlayerPosition = player.transform.position;
								chunksLoaded = true;

								
						} else if (Vector3.Distance (lastPlayerPosition, player.transform.position) > 0.1f) {
								lastPlayerPosition = player.transform.position;
								determinePlayerRegion (player.transform.position);
								LoadChunks (player.transform.position);
							
						}
				}
		}
}
