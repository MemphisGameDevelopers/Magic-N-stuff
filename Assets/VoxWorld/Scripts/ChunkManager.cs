//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

		public float renderSpeed;
		public float initialPoolSize = 1000;
		public int unloadBlockDistance = 10;
		private GameObject player;
		private LinkedList<GameObject> inactiveChunkPool;
		private LinkedList<Chunk> chunksToUpdate;
		private LinkedList<GameObject> chunkGOsToUpdate;
		private BinaryHeap<ChunkWatcher> unloaderQueue;
		private bool coroutineStarted = false;
		private bool firstRun = true;
		
		private class ChunkWatcher : IComparable<ChunkWatcher>
		{
				public float timeSinceActive;
				public Region region;
				public int x, y, z;
				public Chunk chunk;
				
				public ChunkWatcher (Region inRegion, Chunk inChunk, int xi, int yi, int zi)
				{
						region = inRegion;
						chunk = inChunk;
						x = xi;
						y = yi;
						z = zi;
						timeSinceActive = Time.realtimeSinceStartup;
				}
				
				public int CompareTo (ChunkWatcher other)
				{
						return (int)(this.timeSinceActive - other.timeSinceActive);
				}
		}
		
		void Start ()
		{
				inactiveChunkPool = new LinkedList<GameObject> ();
				chunksToUpdate = new LinkedList<Chunk> ();
				unloaderQueue = new BinaryHeap<ChunkWatcher> ();
				player = GameObject.FindGameObjectWithTag ("Player");

		}
		
		public void createChunkPool ()
		{
				while (inactiveChunkPool.Count < initialPoolSize) {
						GameObject newChunk = createChunk ();
						inactiveChunkPool.AddLast (newChunk);
				}
				Debug.Log ("Chunk pool creation completed.");
		}
		public GameObject getChunk ()
		{
				if (inactiveChunkPool.Count > 0) {
						LinkedListNode<GameObject> chunkGO = inactiveChunkPool.First;
						inactiveChunkPool.RemoveFirst ();
						return chunkGO.Value;
				} else {
						//Need to create a new chunk
						return createChunk ();
				}
		}

		public void addToUnloadQueue (Region region, Chunk chunk, int x, int y, int z)
		{
				ChunkWatcher watcher = new ChunkWatcher (region, chunk, x, y, z);
				unloaderQueue.Add (watcher);
		}
		
		//Need both references here.
		public void flagChunkForUpdate (Chunk chunk)
		{
				chunksToUpdate.AddLast (chunk);
		}
		
		// Update is called once per frame
		void Update ()
		{

				if (!coroutineStarted) {
						coroutineStarted = true;
						StartCoroutine ("UpdateChunks");
				}
		}
		
		private void popAndGenerate ()
		{
				//pop the chunk
				Chunk chunk = chunksToUpdate.First.Value;
				chunksToUpdate.RemoveFirst ();
				chunk.makeActive ();
		}

		IEnumerator UpdateChunks ()
		{
				for (;;) {
						
						//Wait for the initial chunks to get loaded and load them all at once.
						if (chunksToUpdate.Count > 0 && firstRun) {
								Debug.Log ("only once");
								while (chunksToUpdate.Count > 0) {
										firstRun = false;
										popAndGenerate ();
								}
						}
						
						if (chunksToUpdate.Count > 0) {
								popAndGenerate ();
						}
						
						
						//Might need to unload chunks that have been around for awhile
						ChunkWatcher chunkWatcher = unloaderQueue.Peek ();
						if (chunkWatcher != null) {

								if (Time.realtimeSinceStartup - chunkWatcher.timeSinceActive > 30) {
										unloaderQueue.Remove ();
										float distance = Vector3.Distance (player.transform.position, chunkWatcher.chunk.gameObject.transform.position);
										if (Math.Abs (distance) > unloadBlockDistance) {
												chunkWatcher.chunk.gameObject.SetActive (false);
												chunkWatcher.region.chunks [chunkWatcher.x, chunkWatcher.y, chunkWatcher.z] = null;
												inactiveChunkPool.AddLast (chunkWatcher.chunk.gameObject);
										} else {
												//update time stamp and put back in queue.
												chunkWatcher.timeSinceActive += 30;
												unloaderQueue.Add (chunkWatcher);
										}
								}
						}
						
						yield return new WaitForSeconds (renderSpeed);
				}
		}


		private GameObject createChunk ()
		{
				GameObject newChunk = GameObject.Instantiate (Resources.Load ("Voxel Generators/Chunk")) as GameObject;
				newChunk.SetActive (false);
				newChunk.transform.parent = this.transform;
				newChunk.transform.rotation = new Quaternion (0, 0, 0, 0);
				return newChunk;
		}
}


