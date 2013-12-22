using UnityEngine;
using System.Collections;

public class VoxelModifyTerrain : MonoBehaviour
{
	

		GameObject cameraGO;
		public VoxelWorld world;
		public Region myRegion = null;
		public float dist = 0;
		public int distToLoad = 4;
		public int distToUnload = 8;
	
		// Use this for initialization
		void VoxelModify ()
		{
				cameraGO = GameObject.FindGameObjectWithTag ("MainCamera");
		}

		private void determinePlayerRegion (Vector3 playerPos)
		{
				if(world == null){
					world = gameObject.GetComponent ("VoxelWorld") as VoxelWorld;
				}
				if (playerPos.x >= myRegion.regionXoffset + myRegion.regionX && 
						playerPos.z < myRegion.regionZoffset + myRegion.regionZ) {
						myRegion = myRegion.getNeighbor (Region.Directions.East);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.z >= myRegion.regionZoffset + myRegion.regionZ && 
						playerPos.x < myRegion.regionXoffset + myRegion.regionX) {
						myRegion = myRegion.getNeighbor (Region.Directions.North);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.x < myRegion.regionXoffset && 
						playerPos.z >= myRegion.regionZoffset) {	
						myRegion = myRegion.getNeighbor (Region.Directions.West);
						world.changeFocusRegion (myRegion);
				} else if (playerPos.z < myRegion.regionZoffset && 
						playerPos.x >= myRegion.regionXoffset) {		
						myRegion = myRegion.getNeighbor (Region.Directions.South);
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
				
				int playerChunkx = (Mathf.FloorToInt (playerPos.x) - myRegion.regionXoffset) / myRegion.chunkSize;
				int playerChunkz = (Mathf.FloorToInt (playerPos.z) - myRegion.regionZoffset) / myRegion.chunkSize;
				int x_start = playerChunkx - distToUnload - 2;
				int x_finish = playerChunkx + distToUnload + 2;
				int z_start = playerChunkz - distToUnload - 2;
				int z_finish = playerChunkz + distToUnload + 2;

				for (int x = x_start; x < x_finish; x++) {
						for (int z = z_start; z < z_finish; z++) {
								dist = Vector2.Distance (new Vector2 (x, z), new Vector2 (playerChunkx, playerChunkz));

								if (dist <= distToLoad) {
										if (x >= 0 && z >= 0 && x < myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												genRegionColumn (myRegion, x, z);
										} else if (x < 0 && z < 0) {
												//southwest
												Region region = myRegion.getNeighbor (Region.Directions.SouthWest);
												if (region == null) {
														continue;
												}

												int newX = region.regionX / region.chunkSize + x;
												int newZ = region.regionZ / region.chunkSize + z;
												genRegionColumn (region, newX, newZ);
										} else if (x >= myRegion.chunks.GetLength (0) && z >= myRegion.chunks.GetLength (2)) {
												//northeast
												Region region = myRegion.getNeighbor (Region.Directions.NorthEast);
												if (region == null) {
														continue;
												}
												int newX = x - region.regionX / region.chunkSize;
												int newZ = z - region.regionZ / region.chunkSize;
												genRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= myRegion.chunks.GetLength (0)) {
												//southeast
												Region region = myRegion.getNeighbor (Region.Directions.SouthEast);
												if (region == null) {
														continue;
												}
												int newX = x - region.regionX / region.chunkSize;
												int newZ = region.regionZ / region.chunkSize + z;
												genRegionColumn (region, newX, newZ);
										} else if (z >= myRegion.chunks.GetLength (2) && x < 0) {
												//northwest
												Region region = myRegion.getNeighbor (Region.Directions.NorthWest);
												if (region == null) {
														continue;
												}
												int newX = region.regionX / region.chunkSize + x;
												int newZ = z - region.regionZ / region.chunkSize;
												genRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= 0) {
												//south
												Region region = myRegion.getNeighbor (Region.Directions.South);
												if (region == null) {
														continue;
												}
												int newZ = region.regionZ / region.chunkSize + z;
												genRegionColumn (region, x, newZ);

										} else if (x < 0 && z >= 0) {
												//west
												Region region = myRegion.getNeighbor (Region.Directions.West);
												if (region == null) {
														continue;
												}
												int newX = region.regionX / region.chunkSize + x;
												genRegionColumn (region, newX, z);
										} else if (x >= myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												//east
												Region region = myRegion.getNeighbor (Region.Directions.East);
												if (region == null) {
														continue;
												}
												int newX = x - region.regionX / region.chunkSize;
												genRegionColumn (region, newX, z);
										} else if (z >= myRegion.chunks.GetLength (2) && x >= 0) {
												//north
												Region region = myRegion.getNeighbor (Region.Directions.North);
												if (region == null) {
														continue;
												}
												int newZ = z - region.regionZ / region.chunkSize;
												genRegionColumn (region, x, newZ);


										}
								} else if (dist > distToUnload) {
										if (x >= 0 && z >= 0 && x < myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												destroyRegionColumn (myRegion, x, z);
										} else if (x < 0 && z < 0) {
												//southwest
												Region region = myRegion.getNeighbor (Region.Directions.SouthWest);
												if (region == null) {
														continue;
												}
						
												int newX = region.regionX / region.chunkSize + x;
												int newZ = region.regionZ / region.chunkSize + z;
												destroyRegionColumn (region, newX, newZ);
										} else if (x >= myRegion.chunks.GetLength (0) && z >= myRegion.chunks.GetLength (2)) {
												//northeast
												Region region = myRegion.getNeighbor (Region.Directions.NorthEast);
												if (region == null) {
														continue;
												}
												int newX = x - region.regionX / region.chunkSize;
												int newZ = z - region.regionZ / region.chunkSize;
												destroyRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= myRegion.chunks.GetLength (0)) {
												//southeast
												Region region = myRegion.getNeighbor (Region.Directions.SouthEast);
												if (region == null) {
														continue;
												}
												int newX = x - region.regionX / region.chunkSize;
												int newZ = region.regionZ / region.chunkSize + z;
												destroyRegionColumn (region, newX, newZ);
										} else if (z >= myRegion.chunks.GetLength (2) && x < 0) {
												//northwest
												Region region = myRegion.getNeighbor (Region.Directions.NorthWest);
												if (region == null) {
														continue;
												}
												int newX = region.regionX / region.chunkSize + x;
												int newZ = z - region.regionZ / region.chunkSize;
												destroyRegionColumn (region, newX, newZ);
										} else if (z < 0 && x >= 0) {
												//south
												Region region = myRegion.getNeighbor (Region.Directions.South);
												if (region == null) {
														continue;
												}
												int newZ = region.regionZ / region.chunkSize + z;
												destroyRegionColumn (region, x, newZ);
						
										} else if (x < 0 && z >= 0) {
												//west
												Region region = myRegion.getNeighbor (Region.Directions.West);
												if (region == null) {
														continue;
												}
												int newX = region.regionX / region.chunkSize + x;
												destroyRegionColumn (region, newX, z);
										} else if (x >= myRegion.chunks.GetLength (0) && z < myRegion.chunks.GetLength (2)) {
												//east
												Region region = myRegion.getNeighbor (Region.Directions.East);
												if (region == null) {
														continue;
												}
												int newX = x - region.regionX / region.chunkSize;
												destroyRegionColumn (region, newX, z);
										} else if (z >= myRegion.chunks.GetLength (2) && x >= 0) {
												//north
												Region region = myRegion.getNeighbor (Region.Directions.North);
												if (region == null) {
														continue;
												}
												int newZ = z - region.regionZ / region.chunkSize;
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
		
				SetBlockAt (position, block);
		}
	
		public void AddBlockAt (RaycastHit hit, byte block)
		{
				//adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
				Vector3 position = hit.point;
				position += (hit.normal * 0.5f);
		
				SetBlockAt (position, block);
		}
	
		public void SetBlockAt (Vector3 position, byte block)
		{
				//sets the specified block at these coordinates
		
				int x = Mathf.RoundToInt (position.x);
				int y = Mathf.RoundToInt (position.y);
				int z = Mathf.RoundToInt (position.z);
		
				SetBlockAt (x, y, z, block);
		}
	
		public void SetBlockAt (int x, int y, int z, byte block)
		{
				//adds the specified block at these coordinates
				myRegion.data [x, y, z] = block;
				UpdateChunkAt (x, y, z);
		}
	
		//To do: add a way to just flag the chunk for update then it update it in lateupdate
		private void UpdateChunkAt (int x, int y, int z)
		{
				//Updates the chunk containing this block
		
				int updateX = Mathf.FloorToInt (x / myRegion.chunkSize);
				int updateY = Mathf.FloorToInt (y / myRegion.chunkSize);
				int updateZ = Mathf.FloorToInt (z / myRegion.chunkSize);
		
				//print ("Updating: " + updateX + "," + updateY + ", " + updateZ);
		
				//Update the chunk's mesh
				myRegion.chunks [updateX, updateY, updateZ].update = true;
		
				//Update neighbor meshes as neccessary.
				if (x - (myRegion.chunkSize * updateX) == 0 && updateX != 0) {
						myRegion.chunks [updateX - 1, updateY, updateZ].update = true;
				}
		
				if (x - (myRegion.chunkSize * updateX) == 15 && updateX != myRegion.chunks.GetLength (0) - 1) {
						myRegion.chunks [updateX + 1, updateY, updateZ].update = true;
				}
		
				if (y - (myRegion.chunkSize * updateY) == 0 && updateY != 0) {
						myRegion.chunks [updateX, updateY - 1, updateZ].update = true;
				}
		
				if (y - (myRegion.chunkSize * updateY) == 15 && updateY != myRegion.chunks.GetLength (1) - 1) {
						myRegion.chunks [updateX, updateY + 1, updateZ].update = true;
				}
		
				if (z - (myRegion.chunkSize * updateZ) == 0 && updateZ != 0) {
						myRegion.chunks [updateX, updateY, updateZ - 1].update = true;
				}
		
				if (z - (myRegion.chunkSize * updateZ) == 15 && updateZ != myRegion.chunks.GetLength (2) - 1) {
						myRegion.chunks [updateX, updateY, updateZ + 1].update = true;
				}
		
		}
	
		// Update is called once per frame
		void Update ()
		{
			
				if (myRegion != null) {
						determinePlayerRegion (GameObject.FindGameObjectWithTag ("Player").transform.position);
						LoadChunks (GameObject.FindGameObjectWithTag ("Player").transform.position);
				}
		}
}
