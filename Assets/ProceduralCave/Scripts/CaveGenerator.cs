using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CaveGenerator : MonoSingleton <CaveGenerator> {
	
	// Map Parameters
	public const int MAP_SIZE_Z = 64;
	public const int MAP_SIZE_Y = 4;
	public const int MAP_SIZE_X = 64;
	
	
	// Tilemap
	public CaveTile[, ,] tiles;
	
	//Prefabs
	public GameObject prefabFloor01;
	public GameObject prefabWall01; 
	
	//Miners and Workers
	List<MinerBot> miners;
	
	// On Awake
	public override void Init()
	{
		// Initialize the tilemap
		tiles = new CaveTile[MAP_SIZE_Z,MAP_SIZE_Y,MAP_SIZE_X];
		for (int i = 0; i < MAP_SIZE_Z; i++) {
			for (int j = 0; j < MAP_SIZE_Y; j++) {
				for(int k = 0; k < MAP_SIZE_X; k++){
					tiles[i,j,k] = new CaveTile(CaveTile.TILE_WALL);
				}
			}
		
		}
		
		//Create Miners
		generateMiners ();
		
		generateRooms();
	}
	
	void generateMiners(){
		miners = new List<MinerBot>();
		int minerCount = (int)((Random.value * 10) + 5);
		for(int i = 0; i < minerCount; i++){
			int startX = (int)(Random.value * (MAP_SIZE_X - 1));
			int startZ = (int)(Random.value * (MAP_SIZE_Z - 1));
			for(int j = 1; j < MAP_SIZE_Y; j++){
				tiles[startZ,j,startX] = new CaveTile(CaveTile.TILE_EMPTY);
			}
			MinerBot miner = new MinerBot(startX,startZ);
			miners.Add(miner);
		}
		Debug.Log ("Created " + miners.Count + " miners");
		//Dig until no miners left
		int stopCount = 500;
		while(stopCount > 0){
		//while(miners.Count != 0){
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
			stopCount = stopCount - 1;
		}
	}
	void generateRooms(){
		for (int i = 0; i < MAP_SIZE_Z; i++) {
			for (int j = 0; j < MAP_SIZE_Y; j++) {
				for(int k = 0; k < MAP_SIZE_X; k++){
					if (tiles[i,j,k].id == CaveTile.TILE_WALL){
						GameObject floor = GameObject.Instantiate(prefabWall01,new Vector3(i,j,k),Quaternion.identity) as GameObject;
					}
				}
			}
		}
	}
}
