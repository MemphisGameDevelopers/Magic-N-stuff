using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CaveGenerator : MonoSingleton <CaveGenerator> {
	
	// Map Parameters
	public const int MAP_WIDTH = 64;
	public const int MAP_HEIGHT = 64;
	
	// Tilemap
	public CaveTile[,] tiles;
	
	//Prefabs
	public GameObject prefabFloor01;
	public GameObject prefabWall01; 
	
	//Miners and Workers
	List<MinerBot> miners;
	
	// On Awake
	public override void Init()
	{
		// Initialize the tilemap
		tiles = new CaveTile[MAP_HEIGHT,MAP_WIDTH];
		for (int i = 0; i < MAP_HEIGHT; i++) {
			for (int j = 0; j < MAP_WIDTH; j++) {
				tiles[i,j] = new CaveTile(CaveTile.TILE_WALL);
			}
		
		}
		
		//Create Miners
		miners = new List<MinerBot>();
		int minerCount = (int)((Random.value * 10) + 5);
		for(int i = 0; i < minerCount; i++){
			int startX = (int)(Random.value * (MAP_WIDTH - 1));
			int startZ = (int)(Random.value * (MAP_HEIGHT - 1));
			tiles[startX,startZ] = new CaveTile(CaveTile.TILE_EMPTY);
			MinerBot miner = new MinerBot(startX,startZ);
			miners.Add(miner);
		}
		Debug.Log ("Created " + miners.Count + " miners");
		//Dig until no miners left
		while(miners.Count != 0){
			for(int i = 0; i < miners.Count; i++){
				MinerBot miner = miners[i];
				bool stillWorking = miner.dig(tiles);
				if(!stillWorking){
					Debug.Log("Removing miner");
					miners.Remove(miner);	
				}else{
					double roll = Random.value * 100;
					if(roll <= 10){
						MinerBot newMiner = miner.clone ();
						miners.Add (newMiner);
					}
				}
			}
		}
		
		generateRooms();
	}
	
	void generateRooms(){
		for (int i = 0; i < MAP_HEIGHT; i++) {
			for (int j = 0; j < MAP_WIDTH; j++) {
				if(tiles[i,j].id == CaveTile.TILE_EMPTY){
					GameObject floor = GameObject.Instantiate(prefabFloor01,new Vector3(i,0.0f,j),Quaternion.identity) as GameObject;
				}else if (tiles[i,j].id == CaveTile.TILE_WALL){
					GameObject floor = GameObject.Instantiate(prefabWall01,new Vector3(i,1.5f,j),Quaternion.identity) as GameObject;
				}
			}
		}
	}
}
