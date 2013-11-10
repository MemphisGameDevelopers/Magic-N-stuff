using UnityEngine;

public class MinerBot {
	
	public int posX;
	public int posZ;
	
	private int boardX = CaveGenerator.MAP_SIZE_X;
	private int boardZ = CaveGenerator.MAP_SIZE_Z;
	private int height = CaveGenerator.MAP_SIZE_Y;
	
	private int curDirection = (int)(Random.value * 4);
	
	public MinerBot(int startX, int startZ){
		posX = startX;
		posZ = startZ;
	}
	
	public bool dig(CaveTile[,,] tileMap){
		int TileUp = posZ + 1 < boardZ?tileMap[posZ + 1,1,posX].id:CaveTile.TILE_WALL;
		int TileDown = posZ - 1 > 0?tileMap[posZ - 1,1,posX].id:CaveTile.TILE_WALL;
		int TileLeft = posX + 1 < boardX?tileMap[posZ,1,posX + 1].id:CaveTile.TILE_WALL;
		int TileRight = posX - 1 > 0?tileMap[posZ,1,posX - 1].id:CaveTile.TILE_WALL;
		
		//Rule 1: Only dig if the miner is next at least one wall
		if(TileUp == CaveTile.TILE_EMPTY &&
			TileDown == CaveTile.TILE_EMPTY &&
			TileLeft == CaveTile.TILE_EMPTY &&
			TileRight == CaveTile.TILE_EMPTY){
			Debug.Log ("Miner Died");
			return false;
		}
		
		//Dig in a random direction
		float rand = Random.value;
		if(rand < .8){
			curDirection = (int)(Random.value * 4);
		}
		if(curDirection == 0){
				posZ = (posZ+1) >= boardZ?posZ:posZ+1;
		}else if(curDirection == 1){
				posX = (posX+1) >= boardX?posX:posX+1;
		}else if(curDirection == 2){
				posZ = (posZ-1) <= 0?posZ:posZ-1;
		}else if(curDirection == 3){
				posX = (posX-1) <= 0?posX:posX-1;
		}
		
		//Dig the tiles.  Don't dig the floor.
		for(int i = 1; i < height; i++){
			tileMap[posZ,i,posX].id = CaveTile.TILE_EMPTY;
		}
		
		return true;	
	}
	
	public MinerBot clone(){
		Debug.Log("Created Miner");
		MinerBot minerBot = new MinerBot(posX,posZ);
		return minerBot;
	}
}
