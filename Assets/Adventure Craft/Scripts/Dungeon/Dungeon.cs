using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Dungeon class. Singleton.
public class Dungeon : MonoBehaviour, VoxelStream
{
	
		// Dungeon Parameters
		public int MAP_COLS = 128;
		public int MAP_HEIGHT = 5;
		public int MAP_ROWS = 128;
	
		// Room Parameters
		public int ROOM_MAX_SIZE = 24;
		public int ROOM_MIN_SIZE = 4;
		public int ROOM_WALL_BORDER = 1;
		public bool ROOM_UGLY_ENABLED = true;
		public float ROOM_MAX_RATIO = 5.0f;
		public int ROOM_TO_EXIT_LENGTH = 6;
	
		// Generation Parameters
		public static int MAX_DEPTH = 10;
		public static int CHANCE_STOP = 5;
		public static int SLICE_TRIES = 10;
		public int CORRIDOR_WIDTH = 2;
	
		public int MAX_NUM_BARRIERS = 5;
	
		// Tilemap
		private byte[,,] tiles;
		private Vector3 tileBounds;
	
		// The Random Seed
		public int seed = -1;
	
		// QuadTreeMapLayer for dungeon distribution
		private QuadTreeMapLayer quadTree;
	
		// List of rooms
		private List<Room_2D> rooms;

		public byte GetBlockAtRelativeCoords (int x, int y, int z)
		{
				if (x >= MAP_COLS || x < 0 ||
						y >= MAP_HEIGHT || y < 0 ||
						z >= MAP_ROWS || z < 0) {
						return (byte)0;
				}
				return tiles [z, y, x];
		}
		public Vector3 getBounds ()
		{
				return new Vector3 (tiles.GetLength (0), tiles.GetLength (1), tiles.GetLength (2));
		}
		public byte[,,] GetAllBlocks ()
		{
				Debug.Log ("dungeon returning all blocks");
				return tiles;
		}
	
		private void resetBlocks ()
		{
				tiles = new byte[MAP_ROWS, MAP_HEIGHT, MAP_COLS];
				for (int x = 0; x < MAP_COLS; x++)
						for (int y = 0; y < MAP_HEIGHT; y++)
								for (int z = 0; z < MAP_ROWS; z++)
										
										//Draw the floor
										if (y == 0) { 
												tiles [z, y, x] = 1;
										} else {
												tiles [z, y, x] = 2;
										}
		}
	
		// On Awake
		public void create (object o)
		{
				// Initialize the tilemap
				resetBlocks ();
						
				// Init QuadTree
				quadTree = new QuadTreeMapLayer (new AABB (new XY (MAP_COLS / 2.0f, MAP_ROWS / 2.0f), new XY (MAP_COLS / 2.0f, MAP_ROWS / 2.0f)), this);
		
				// List of rooms
				rooms = new List<Room_2D> ();
		
				//barriers = new List<GameObject> ();
				
				// Set the randome seed
				//Random.seed = seed;
				
				// Generate Dungeon
				Debug.Log ("Dungeon Generation Started");
				
				GenerateDungeon (seed);
		}
	
		
	
		// Generate a new dungeon with the given seed
		public void GenerateDungeon (int seed)
		{
				Debug.Log ("Generating QuadTreeMapLayer");

				// Generate QuadTreeMapLayer
				GenerateQuadTree (ref quadTree);
		
				Debug.Log ("Generating Rooms");
		
				// Generate Rooms
				GenerateRooms (ref rooms, quadTree);
		
				Debug.Log ("Generating Corridors");
		
				// Generate Corridors
				GenerateCorridors ();
		
		
				Debug.Log ("Generating Exits");
				DigExits ();
				//GenerateWalls ();
		
				Debug.Log ("Finished generating Dungeon!");
		
			
		
		}
	

		// Generate the quadtree system
		void GenerateQuadTree (ref QuadTreeMapLayer _quadTree)
		{
				_quadTree.GenerateQuadTree (seed);
		}
	
		// Generate the list of rooms and dig them
		public void GenerateRooms (ref List<Room_2D> _rooms, QuadTreeMapLayer _quadTree)
		{
				// Childless node
				if (_quadTree.northWest == null && _quadTree.northEast == null && _quadTree.southWest == null && _quadTree.southEast == null) {
						_rooms.Add (GenerateRoom (_quadTree));
						return;
				}
		
				// Recursive call
				if (_quadTree.northWest != null)
						GenerateRooms (ref _rooms, _quadTree.northWest);
				if (_quadTree.northEast != null)
						GenerateRooms (ref _rooms, _quadTree.northEast);
				if (_quadTree.southWest != null)
						GenerateRooms (ref _rooms, _quadTree.southWest);
				if (_quadTree.southEast != null)
						GenerateRooms (ref _rooms, _quadTree.southEast);
		}
	
		// Generate a single room
		public Room_2D GenerateRoom (QuadTreeMapLayer _quadTree)
		{
				// Center of the room
				XY roomCenter = new XY ();
				roomCenter.x = Random.Range (ROOM_WALL_BORDER + _quadTree.boundary.Left () + ROOM_MIN_SIZE / 2.0f, _quadTree.boundary.Right () - ROOM_MIN_SIZE / 2.0f - ROOM_WALL_BORDER);
				roomCenter.y = Random.Range (ROOM_WALL_BORDER + _quadTree.boundary.Bottom () + ROOM_MIN_SIZE / 2.0f, _quadTree.boundary.Top () - ROOM_MIN_SIZE / 2.0f - ROOM_WALL_BORDER);		
		
				// Half size of the room
				XY roomHalf = new XY ();
		
				float halfX = (_quadTree.boundary.Right () - roomCenter.x - ROOM_WALL_BORDER);
				float halfX2 = (roomCenter.x - _quadTree.boundary.Left () - ROOM_WALL_BORDER);
				if (halfX2 < halfX)
						halfX = halfX2;
				if (halfX > ROOM_MAX_SIZE / 2.0f)
						halfX = ROOM_MAX_SIZE / 2.0f;
		
				float halfY = (_quadTree.boundary.Top () - roomCenter.y - ROOM_WALL_BORDER);
				float halfY2 = (roomCenter.y - _quadTree.boundary.Bottom () - ROOM_WALL_BORDER);
				if (halfY2 < halfY)
						halfY = halfY2;
				if (halfY > ROOM_MAX_SIZE / 2.0f)
						halfY = ROOM_MAX_SIZE / 2.0f;
		
				roomHalf.x = Random.Range ((float)ROOM_MIN_SIZE / 2.0f, halfX);
				roomHalf.y = Random.Range ((float)ROOM_MIN_SIZE / 2.0f, halfY);
		
				// Eliminate ugly zones
				if (ROOM_UGLY_ENABLED == false) {
						float aspect_ratio = roomHalf.x / roomHalf.y;
						if (aspect_ratio > ROOM_MAX_RATIO || aspect_ratio < 1.0f / ROOM_MAX_RATIO)
								return GenerateRoom (_quadTree); 
				}
		
				// Create AABB
				AABB randomAABB = new AABB (roomCenter, roomHalf);
		
				// Dig the room in our tilemap
				DigRoom (randomAABB.BottomTile (), randomAABB.LeftTile (), randomAABB.TopTile () - 1, randomAABB.RightTile () - 1);
		
				// Return the room
				return new Room_2D (randomAABB, _quadTree);
		}
	
		void GenerateCorridors ()
		{
				quadTree.GenerateCorridors ();
		}
	
		// Generate walls when there's something near
		public void GenerateWalls ()
		{
				// Place walls
				for (int i = 0; i < MAP_ROWS; i++) {
						for (int j = 0; j < MAP_COLS; j++) {
								bool room_near = false;
								if (IsPassable (i, j))
										continue;
								if (i > 0)
								if (IsPassable (i - 1, j))
										room_near = true;
								if (i < MAP_ROWS - 1)
								if (IsPassable (i + 1, j))
										room_near = true;
								if (j > 0)
								if (IsPassable (i, j - 1))
										room_near = true;
								if (j < MAP_COLS - 1)
								if (IsPassable (i, j + 1))
										room_near = true;
								if (room_near)
										SetWall (i, j);
						}
				}
		}
	
		// Helper Methods
		public bool IsEmpty (int row, int col)
		{
				return tiles [row, MAP_HEIGHT / 2, col] == 0;
		}
	
		public bool IsPassable (int row, int col)
		{
				return tiles [row, MAP_HEIGHT / 2, col] == 0;
		}
	
		public bool IsPassable (XY xy)
		{
				return IsPassable ((int)xy.y, (int)xy.x);
		}
	
		public void SetWall (int row, int col)
		{
				for (int y = 0; y < MAP_HEIGHT; y++) {
						tiles [row, y, col] = 2;
				}
		}
	
		// Dig a room, placing floor tiles
		public void DigRoom (int row_bottom, int col_left, int row_top, int col_right)
		{
				// Out of range
				if (row_top < row_bottom) {
						int tmp = row_top;
						row_top = row_bottom;
						row_bottom = tmp;
				}
		
				// Out of range
				if (col_right < col_left) {
						int tmp = col_right;
						col_right = col_left;
						col_left = tmp;
				}
		
				if (row_top > MAP_ROWS - 1)
						return;
				if (row_bottom < 0)
						return;
				if (col_right > MAP_COLS - 1)
						return;
				if (col_left < 0)
						return;
		
				// Dig floor
				for (int row = row_bottom; row <= row_top; row++) 
						for (int col = col_left; col <= col_right; col++) 
								DigRoom (row, col);
		}
	
		public void DigRoom (int row, int col)
		{
				for (int y = 1; y < MAP_HEIGHT; y++) {
						tiles [row, y, col] = 0;
				}
		}
	
		public void DigCorridor (int row, int col)
		{
				for (int y = 1; y < MAP_HEIGHT; y++) {
						tiles [row, y, col] = 0;
				}
		}
	
		public void DigCorridor (XY p1, XY p2)
		{
				int row1 = Mathf.RoundToInt (p1.y);
				int row2 = Mathf.RoundToInt (p2.y);
				int col1 = Mathf.RoundToInt (p1.x);
				int col2 = Mathf.RoundToInt (p2.x);
		
				DigCorridor (row1, col1, row2, col2);
		}
	
		private void DigCorridor (int row1, int col1, int row2, int col2)
		{		
				
				if (row1 <= row2) {
						// source is below the dest
						for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
								for (int row = row1; row <= row2; row++)
										DigCorridor (row, col);
				} else {
						// source is above the dest
						for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
								for (int row = row2; row <= row1; row++)
										DigCorridor (row, col);
				}
		
				if (col1 <= col2) {
						// source is to the left of the dest.
						for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
								for (int col = col1; col <= col2; col++) 
										DigCorridor (row, col);

				} else {
						// source is to the right of the dest.
						for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
								for (int col = col2; col <= col1; col++)
										DigCorridor (row, col);
				}
		}
		
		public void DigExits ()
		{
				//Find all the eligable exits
				LinkedList<Room_2D> eligableRooms = new LinkedList<Room_2D> ();
				foreach (Room_2D room in rooms) {
						int col = Mathf.RoundToInt (room.boundary.center.x);
						int row = Mathf.RoundToInt (room.boundary.center.y);
						if (Mathf.Abs (MAP_ROWS - row) <= ROOM_TO_EXIT_LENGTH || 
								row <= ROOM_TO_EXIT_LENGTH || 
								Mathf.Abs (MAP_COLS - col) <= ROOM_TO_EXIT_LENGTH ||
								col <= ROOM_TO_EXIT_LENGTH) {
								eligableRooms.AddLast (room);
						} 
				}
				
				//Choose a random number of exits/entrances
				int roomCount = eligableRooms.Count;
				if (roomCount > 0) {
						int index = Random.Range (0, roomCount);
						Room_2D room = eligableRooms.ElementAt (index);
						int col = Mathf.RoundToInt (room.boundary.center.x);
						int row = Mathf.RoundToInt (room.boundary.center.y);
						if (Mathf.Abs (MAP_ROWS - row) <= ROOM_TO_EXIT_LENGTH || 
								row <= ROOM_TO_EXIT_LENGTH) {
								if (row > MAP_ROWS / 2) {
										DigExit (row, col, MAP_ROWS - 1, col); 
								} else {
										DigExit (row, col, 0, col); 
								}
						} else if (Mathf.Abs (MAP_COLS - col) <= ROOM_TO_EXIT_LENGTH || col <= ROOM_TO_EXIT_LENGTH) {
								if (col > MAP_COLS / 2) {
										DigExit (row, col, row, MAP_COLS - 1); 
								} else {
										DigExit (row, col, row, 0); 
								}
						}
				}
		}
	
		private void DigExit (int row1, int col1, int row2, int col2)
		{		
				if (col1 == col2) {
						if (row1 <= row2) {
								// source is below the dest
								for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
										for (int row = row1; row <= row2; row++)
												DigCorridor (row, col);
						} else {
								// source is above the dest
								for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
										for (int row = row2; row <= row1; row++)
												DigCorridor (row, col);
						}
				} else {
		
						if (col1 <= col2) {
								// source is to the left of the dest.
								for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
										for (int col = col1; col <= col2; col++) 
												DigCorridor (row, col);
			
						} else {
								// source is to the right of the dest.
								for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
										for (int col = col2; col <= col1; col++)
												DigCorridor (row, col);
						}
				}
		}
}
