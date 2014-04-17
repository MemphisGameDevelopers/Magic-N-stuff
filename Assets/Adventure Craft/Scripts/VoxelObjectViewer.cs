using UnityEngine;
using System.Collections;

public class VoxelObjectViewer : MonoBehaviour
{

		private Chunk[,,] chunks = null;
		public Vector3 objectBounds;
		public GameObject objectToView;
		private GameObject objectGO;
		private VoxelStream voxels;
		public GameObject chunk;
	
		// Use this for initialization
		void Start ()
		{

				
				
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (chunks == null) {
						objectGO = GameObject.Instantiate (objectToView) as GameObject;
						voxels = objectGO.GetComponent (typeof(VoxelStream)) as VoxelStream;
						voxels.create (null);
						objectBounds = voxels.getBounds ();
						chunks = new Chunk[Mathf.FloorToInt (objectBounds.x / Chunk.chunkSize),
			                   Mathf.FloorToInt (objectBounds.y / Chunk.chunkSize),
			                   Mathf.FloorToInt (objectBounds.z / Chunk.chunkSize)];


						GenerateChunks ();
				}
		}
	
		private void GenerateChunks ()
		{
				for (int x=0; x<chunks.GetLength(0); x++) {
						for (int y=0; y<chunks.GetLength(1); y++) {
								for (int z=0; z<chunks.GetLength(2); z++) {
										//Create a temporary Gameobject for the new chunk instead of using chunks[x,y,z]
										GameObject newChunk = Instantiate (chunk, 
						new Vector3 (x * Chunk.chunkSize - 0.5f, y * Chunk.chunkSize + 0.5f, z * Chunk.chunkSize - 0.5f),
			        new Quaternion (0, 0, 0, 0)) as GameObject;
										newChunk.transform.parent = objectGO.transform;
										chunks [x, y, z] = newChunk.GetComponent ("Chunk") as Chunk;
										chunks [x, y, z].voxels = voxels;
										chunks [x, y, z].chunkX = x * Chunk.chunkSize;
										chunks [x, y, z].chunkY = y * Chunk.chunkSize;
										chunks [x, y, z].chunkZ = z * Chunk.chunkSize;			
								}
						}
				}
		}
}
