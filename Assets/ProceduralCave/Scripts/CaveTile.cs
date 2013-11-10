using UnityEngine;

[System.Serializable]
public class CaveTile {
	// Tile Types
	public const int TILE_EMPTY = 0;
	public const int TILE_WALL = 1;
	
	// Tile ID
	public int id;
	
	public CaveTile ( int _id )
	{
		this.id = _id;
	}
}
