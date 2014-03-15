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
		public int initialDistToLoad = 8;
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
		
		private void MoveChunks (Vector3 playerPos)
		{
				int newChunkx = Mathf.FloorToInt (playerPos.x) / Chunk.chunkSize;
				int newChunky = Mathf.FloorToInt (playerPos.y) / Chunk.chunkSize;
				int newChunkz = Mathf.FloorToInt (playerPos.z) / Chunk.chunkSize;
		
				int oldChunkx = Mathf.FloorToInt (lastPlayerPosition.x) / Chunk.chunkSize;
				int oldChunky = Mathf.FloorToInt (lastPlayerPosition.y) / Chunk.chunkSize;
				int oldChunkz = Mathf.FloorToInt (lastPlayerPosition.z) / Chunk.chunkSize;

				int chunkChangeX = newChunkx - oldChunkx;
				int chunkChangeY = newChunky - oldChunky;
				int chunkChangeZ = newChunkz - oldChunkz;
		
				if (chunkChangeX == 0 && chunkChangeY == 0 && chunkChangeZ == 0) {
						return;
				} 
				
				int xLoadStart = 0;
				int x_end = 0;
				if (chunkChangeX == 0) {
						xLoadStart = newChunkx - distToLoad;
						x_end = newChunkx + distToLoad;
				} else if (chunkChangeX > 0) {
						xLoadStart = oldChunkx + distToLoad;
						x_end = newChunkx + distToLoad;
				} else {
						xLoadStart = newChunkx - distToLoad;
						x_end = oldChunkx - distToLoad;
				}
				
				int yLoadStart = 0;
				int y_end = 0;
				if (chunkChangeY == 0) {
						yLoadStart = newChunky - distToLoad;
						y_end = newChunky + distToLoad;
				} else if (chunkChangeY > 0) {
						yLoadStart = oldChunky + distToLoad;
						y_end = newChunky + distToLoad;
				} else {
						yLoadStart = newChunky - distToLoad;
						y_end = oldChunky - distToLoad;
				}
				if (yLoadStart < 0) {
						yLoadStart = 0;
				}
				if (y_end < 0) {
						y_end = 0;
				}
				
				int zLoadStart = 0;
				int z_end = 0;
				if (chunkChangeZ == 0) {
						zLoadStart = newChunkz - distToLoad;
						z_end = newChunkz + distToLoad;
				} else if (chunkChangeZ > 0) {
						zLoadStart = oldChunkz + distToLoad;
						z_end = newChunkz + distToLoad;
				} else {
						zLoadStart = newChunkz - distToLoad;
						z_end = oldChunkz - distToLoad;
				}
				LinkedList<Region> loadedRegions = new LinkedList<Region> ();
				for (int x = xLoadStart; x < x_end; x++) {
						for (int z = zLoadStart; z < z_end; z++) {
								for (int y = yLoadStart; y < y_end; y++) {
										Region region = world.getRegionAtCoords (x * Chunk.chunkSize, y * Chunk.chunkSize, z * Chunk.chunkSize);
										int[] localCoords = region.convertWorldChunksToLocal (x, y, z);
										region.loadChunk (localCoords [0], localCoords [1], localCoords [2]);
										
										if (!loadedRegions.Contains (region)) {
												loadedRegions.AddLast (region);
										}
								}
						}
				}
				
				foreach (Region region in loadedRegions) {
						world.loadAllNeighbors (region, true);
				}
		}
		private void LoadChunks (Vector3 playerPos)
		{
		
				int playerChunkx = (Mathf.FloorToInt (playerPos.x)) / Chunk.chunkSize;
				int playerChunky = (Mathf.FloorToInt (playerPos.y)) / Chunk.chunkSize;
				int playerChunkz = (Mathf.FloorToInt (playerPos.z)) / Chunk.chunkSize;
				
				int xLoadStart = playerChunkx - initialDistToLoad;
				int xLoadFinish = playerChunkx + initialDistToLoad;
				int yLoadStart = playerChunky - initialDistToLoad;
				int yLoadFinish = playerChunky + initialDistToLoad;
				int zLoadStart = playerChunkz - initialDistToLoad;
				int zLoadFinish = playerChunkz + initialDistToLoad;


				if (yLoadStart < 0) {
						yLoadStart = 0;
				}
				
				for (int x = xLoadStart; x < xLoadFinish; x++) {
						for (int z = zLoadStart; z < zLoadFinish; z++) {
								for (int y = yLoadStart; y < yLoadFinish; y++) {
										Region region = world.getRegionAtCoords (x * Chunk.chunkSize, y * Chunk.chunkSize, z * Chunk.chunkSize);
										if (region == null) {
												XYZ coords = world.getIndexFromCoords (x * Chunk.chunkSize, y * Chunk.chunkSize, z * Chunk.chunkSize);
												region = world.createRegion (coords.x, coords.y, coords.z, false);
												world.loadAllNeighbors (region, true);
										}
										int[] localCoords = region.convertWorldChunksToLocal (x, y, z);
										region.loadChunk (localCoords [0], localCoords [1], localCoords [2]);

								}
						}
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
				int chunkDim = region.regionXZ / Chunk.chunkSize;
				//Neighbor chunks also may need to be re-rendered since their mesh could have
				//potentially changed.
				int xEdge = x - (Chunk.chunkSize * updateX);
				int yEdge = y - (Chunk.chunkSize * updateY);
				int zEdge = z - (Chunk.chunkSize * updateZ);


				if (xEdge == 0) {
						int neighborX = updateX - 1;
						if (neighborX < 0) {
								Region r = world.getRegionAtCoords (region.offsetX - 1, region.offsetY, region.offsetZ);
								chunkManager.flagChunkForUpdate (r.chunks [neighborX + chunkDim, updateY, updateZ]);
						} else {
								chunkManager.flagChunkForUpdate (region.chunks [neighborX, updateY, updateZ]);
						}
				}
		
				if (xEdge == Chunk.chunkSize - 1) {
						int neighborX = updateX + 1;
						if (neighborX >= chunkDim) {
								Region r = world.getRegionAtCoords (region.offsetX + 1, region.offsetY, region.offsetZ);
								chunkManager.flagChunkForUpdate (r.chunks [neighborX - chunkDim, updateY, updateZ]);
						} else {
								chunkManager.flagChunkForUpdate (region.chunks [neighborX, updateY, updateZ]);
						}
				}
		
				if (yEdge == 0) {
						int neighborY = updateY - 1;
						if (neighborY < 0) {
								Region r = world.getRegionAtCoords (region.offsetX, region.offsetY - 1, region.offsetZ);
								chunkManager.flagChunkForUpdate (r.chunks [updateX, neighborY + chunkDim, updateZ]);
						} else {
								chunkManager.flagChunkForUpdate (region.chunks [updateX, neighborY, updateZ]);
						}
				}
		
				if (yEdge == Chunk.chunkSize - 1) {
						int neighborY = updateY + 1;
						if (neighborY >= chunkDim) {
								Region r = world.getRegionAtCoords (region.offsetX, region.offsetY + 1, region.offsetZ);
								chunkManager.flagChunkForUpdate (r.chunks [updateX, neighborY - chunkDim, updateZ]);
						} else {
								chunkManager.flagChunkForUpdate (region.chunks [updateX, neighborY, updateZ]);
						}
				}
				
				if (zEdge == 0) {
						int neighborZ = updateZ - 1;
						if (neighborZ < 0) {
								Region r = world.getRegionAtCoords (region.offsetX, region.offsetY, region.offsetZ - 1);
								chunkManager.flagChunkForUpdate (r.chunks [updateX, updateY, neighborZ + chunkDim]);
						} else {
								chunkManager.flagChunkForUpdate (region.chunks [updateX, updateY, neighborZ]);
						}
				}
		
				if (zEdge == Chunk.chunkSize - 1) {
						int neighborZ = updateZ + 1;
						if (neighborZ >= chunkDim) {
								Region r = world.getRegionAtCoords (region.offsetX, region.offsetY, region.offsetZ + 1);
								chunkManager.flagChunkForUpdate (r.chunks [updateX, updateY, neighborZ - chunkDim]);
						} else {
								chunkManager.flagChunkForUpdate (region.chunks [updateX, updateY, neighborZ]);
						}
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
								determinePlayerRegion (player.transform.position);
								MoveChunks (player.transform.position);
								lastPlayerPosition = player.transform.position;
							
						}
				}
		}
}
