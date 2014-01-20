using UnityEngine;
using System.Collections;

public class VoxelModifyTerrain : MonoBehaviour
{
	

		GameObject cameraGO;
		private GameObject player;
		public VoxelWorld world;
		public Region myRegion = null;
		public int distToLoad = 4;
		public int distToUnload = 8;
		public bool saveLevel = false;
		
		void Awake ()
		{
				cameraGO = GameObject.FindGameObjectWithTag ("MainCamera");
				player = GameObject.FindGameObjectWithTag ("Player");
		}

		public void setStartRegion (Region region)
		{
				myRegion = region;
		}

		private void determinePlayerRegion (Vector3 playerPos)
		{

				if (playerPos.x >= myRegion.getBlockOffsetX () + myRegion.regionXZ && 
						playerPos.z < myRegion.getBlockOffsetZ () + myRegion.regionXZ) {
						myRegion = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetZ);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.z >= myRegion.getBlockOffsetZ () + myRegion.regionXZ && 
						playerPos.x < myRegion.getBlockOffsetX () + myRegion.regionXZ) {
						myRegion = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetZ + 1);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.x < myRegion.getBlockOffsetX () && 
						playerPos.z >= myRegion.getBlockOffsetZ ()) {	
						myRegion = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetZ);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.z < myRegion.getBlockOffsetZ () && 
						playerPos.x >= myRegion.getBlockOffsetX ()) {		
						myRegion = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetZ - 1);
						world.changeFocusRegion (myRegion);
				}
		}

		private void genRegionColumn (Region region, int x, int z)
		{
				if (region.chunks [x, 0, z] == null) {
						region.GenColumn (x, z);
				}
		}

		private void destroyRegionColumn (Region region, int x, int z)
		{
				if (region.chunks [x, 0, z] != null) {
						region.UnloadColumn (x, z);
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

				for (int x = x_start; x < x_finish; x++) {
						for (int z = z_start; z < z_finish; z++) {
								float dist = Vector2.Distance (new Vector2 (x, z), new Vector2 (playerChunkx, playerChunkz));

								if (dist <= distToLoad) {
										if (x >= 0 && z >= 0 && x < myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												genRegionColumn (myRegion, x, z);
										} else if (x < 0 && z < 0) {
												//southwest
												Region region = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetZ - 1);
												int newX = region.regionXZ / Chunk.chunkSize + x;
												int newZ = region.regionXZ / Chunk.chunkSize + z;
												genRegionColumn (region, newX, newZ);
										} else if (x >= myRegion.chunks.GetLength (0) && z >= myRegion.chunks.GetLength (2)) {
												//northeast
												Region region = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetZ + 1);
												int newX = x - region.regionXZ / Chunk.chunkSize;
												int newZ = z - region.regionXZ / Chunk.chunkSize;
												genRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= myRegion.chunks.GetLength (0)) {
												//southeast
												Region region = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetZ - 1);
												int newX = x - region.regionXZ / Chunk.chunkSize;
												int newZ = region.regionXZ / Chunk.chunkSize + z;
												genRegionColumn (region, newX, newZ);
										} else if (z >= myRegion.chunks.GetLength (2) && x < 0) {
												//northwest
												Region region = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetZ + 1);
												int newX = region.regionXZ / Chunk.chunkSize + x;
												int newZ = z - region.regionXZ / Chunk.chunkSize;
												genRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= 0) {
												//south
												Region region = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetZ - 1);
												int newZ = region.regionXZ / Chunk.chunkSize + z;
												genRegionColumn (region, x, newZ);

										} else if (x < 0 && z >= 0) {
												//west
												Region region = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetZ);
												int newX = region.regionXZ / Chunk.chunkSize + x;
												genRegionColumn (region, newX, z);
										} else if (x >= myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												//east
												Region region = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetZ);
												int newX = x - region.regionXZ / Chunk.chunkSize;
												genRegionColumn (region, newX, z);
										} else if (z >= myRegion.chunks.GetLength (2) && x >= 0) {
												//north
												Region region = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetZ + 1);
												int newZ = z - region.regionXZ / Chunk.chunkSize;
												genRegionColumn (region, x, newZ);
										}
								} else if (dist > distToUnload) {
										if (x >= 0 && z >= 0 && x < myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												destroyRegionColumn (myRegion, x, z);
										} else if (x < 0 && z < 0) {
												//southwest
												Region region = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetZ - 1);
												int newX = region.regionXZ / Chunk.chunkSize + x;
												int newZ = region.regionXZ / Chunk.chunkSize + z;
												destroyRegionColumn (region, newX, newZ);
										} else if (x >= myRegion.chunks.GetLength (0) && z >= myRegion.chunks.GetLength (2)) {
												//northeast
												Region region = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetZ + 1);
												int newX = x - region.regionXZ / Chunk.chunkSize;
												int newZ = z - region.regionXZ / Chunk.chunkSize;
												destroyRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= myRegion.chunks.GetLength (0)) {
												//southeast
												Region region = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetZ - 1);
												int newX = x - region.regionXZ / Chunk.chunkSize;
												int newZ = region.regionXZ / Chunk.chunkSize + z;
												destroyRegionColumn (region, newX, newZ);
										} else if (z >= myRegion.chunks.GetLength (2) && x < 0) {
												//northwest
												Region region = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetZ + 1);
												int newX = region.regionXZ / Chunk.chunkSize + x;
												int newZ = z - region.regionXZ / Chunk.chunkSize;
												destroyRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= 0) {
												//south
												Region region = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetZ - 1);
												int newZ = region.regionXZ / Chunk.chunkSize + z;
												destroyRegionColumn (region, x, newZ);
										} else if (x < 0 && z >= 0) {
												//west
												Region region = world.getRegionAtIndex (myRegion.offsetX - 1, myRegion.offsetZ);
												int newX = region.regionXZ / Chunk.chunkSize + x;
												destroyRegionColumn (region, newX, z);
										} else if (x >= myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												//east
												Region region = world.getRegionAtIndex (myRegion.offsetX + 1, myRegion.offsetZ);
												int newX = x - region.regionXZ / Chunk.chunkSize;
												destroyRegionColumn (region, newX, z);
										} else if (z >= myRegion.chunks.GetLength (2) && x >= 0) {
												//north
												Region region = world.getRegionAtIndex (myRegion.offsetX, myRegion.offsetZ + 1);
												int newZ = z - region.regionXZ / Chunk.chunkSize;
												destroyRegionColumn (region, x, newZ);
										}
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

				Region modRegion = world.getRegionAtCoords (x, z);
				int[] localCoords = modRegion.convertWorldToLocal (x, y, z);
				//print ("Set block at world(" + x + "," + y + "," + z + ") local(" + localCoords [0] + "," + localCoords [1] + "," + localCoords [2] + ")");
				modRegion.data [localCoords [0], localCoords [1], localCoords [2]] = block;
				UpdateChunkAt (modRegion, localCoords [0], localCoords [1], localCoords [2]);
		}
	
		//To do: add a way to just flag the chunk for update then it update it in lateupdate
		private void UpdateChunkAt (Region region, int x, int y, int z)
		{
				//Updates the chunk containing this block
				int updateX = Mathf.FloorToInt (x / Chunk.chunkSize);
				int updateY = Mathf.FloorToInt (y / Chunk.chunkSize);
				int updateZ = Mathf.FloorToInt (z / Chunk.chunkSize);
		
				//print ("Updating: " + updateX + "," + updateY + ", " + updateZ);
		
				//Update the chunk's mesh
				region.chunks [updateX, updateY, updateZ].update = true;
				region.isDirty = true;
		
				//Update neighbor chunks as neccessary.
				if (x - (Chunk.chunkSize * updateX) == 0) {
						region.flagChunkForUpdate (updateX - 1, updateY, updateZ);
				}
		
				if (x - (Chunk.chunkSize * updateX) == 15) {
						region.flagChunkForUpdate (updateX + 1, updateY, updateZ);
				}
		
				if (y - (Chunk.chunkSize * updateY) == 0) {
						region.flagChunkForUpdate (updateX, updateY - 1, updateZ);
				}
		
				if (y - (Chunk.chunkSize * updateY) == 15) {
						region.flagChunkForUpdate (updateX, updateY + 1, updateZ);
				}
		
				if (z - (Chunk.chunkSize * updateZ) == 0) {
						region.flagChunkForUpdate (updateX, updateY, updateZ - 1);
				}
		
				if (z - (Chunk.chunkSize * updateZ) == 15) {
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
						determinePlayerRegion (player.transform.position);
						LoadChunks (player.transform.position);
				}
		}
}
