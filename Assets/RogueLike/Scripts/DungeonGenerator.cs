using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// DungeonGenerator class. Singleton.
public class DungeonGenerator : MonoSingleton <DungeonGenerator> {
	
	// Dungeon Parameters
	public int MAP_WIDTH = 128;
	public int MAP_HEIGHT = 128;
	
	// Room Parameters
	public int ROOM_MAX_SIZE = 24;
	public int ROOM_MIN_SIZE = 4;
	public int ROOM_WALL_BORDER = 1;
	public bool ROOM_UGLY_ENABLED = true;
	public float ROOM_MAX_RATIO = 5.0f;
	
	// Generation Parameters
	public int MAX_DEPTH = 10;
	public int CHANCE_STOP = 5;
	public int SLICE_TRIES = 10;
	public int CORRIDOR_WIDTH = 2;

    public int MAX_NUM_BARRIERS = 5;
	
	// Tilemap
	public Tile[,] tiles;
	
	// Prefabs and Instance Management
	public GameObject containerRooms;
	public GameObject prefabWall01; 
	public GameObject prefabFloor01;
	public GameObject meshCombiner;
    public GameObject barrier;
    public GameObject exit;
    public GameObject key;
	
	// Player	
	public GameObject player;
	
	// The Random Seed
	public int seed = -1;
	
	// QuadTree for dungeon distribution
	public QuadTree quadTree;
	
	// List of rooms
	public List<Room> rooms;

    public List<GameObject> barriers;

    public List<GameObject> keys; 
	
	// Auxiliar vars
//	private GameObject floor;
	private Texture2D dungeonTexture;
	
	// On Awake
	public override void Init()
	{
		// Initialize the tilemap
		tiles = new Tile[MAP_HEIGHT,MAP_WIDTH];
		for (int i = 0; i < MAP_HEIGHT; i++) 
			for (int j = 0; j < MAP_WIDTH; j++) 
				tiles[i,j] = new Tile(Tile.TILE_EMPTY);
		
		// Init QuadTree
		quadTree = new QuadTree(new AABB(new XY(MAP_WIDTH/2.0f,MAP_HEIGHT/2.0f),new XY(MAP_WIDTH/2.0f, MAP_HEIGHT/2.0f)));

		// List of rooms
		rooms = new List<Room>();

        barriers = new List<GameObject>();
	}
	
	// On Start
	void Start () 
	{
        // Generate a new Seed
        seed = System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100;

        // Camera on middle and looking down
        Camera.main.transform.position = new Vector3(MAP_WIDTH / 2, 100, MAP_HEIGHT / 2);

        // Set the randome seed
        Random.seed = seed;

        // Generate Dungeon
        Debug.Log("Dungeon Generation Started");

        GenerateDungeon(seed);
	
	}
	
	// Each frame
	void Update () {
		// Generate a new Test Dungeon
		if (Input.GetButtonDown("Jump"))
		{
			// Generate a new Seed
			seed = System.DateTime.Now.Millisecond*1000 + System.DateTime.Now.Minute*100;
			
			// Camera on middle and looking down
<<<<<<< HEAD
			//Camera.mainCamera.transform.position = new Vector3(MAP_WIDTH/2,100,MAP_HEIGHT/2);
=======
			Camera.main.transform.position = new Vector3(MAP_WIDTH/2,100,MAP_HEIGHT/2);
>>>>>>> origin/dev
			
			// Set the randome seed
			Random.seed = seed;
			
			// Generate Dungeon
			Debug.Log ("Dungeon Generation Started");
			
			GenerateDungeon(seed);
			
		}
	}
	
	// Clean everything
    public void ResetDungeon()
    {
        // Disable player
        player.SetActive(false);

        // Reset tilemap
        for (int i = 0; i < MAP_HEIGHT; i++)
            for (int j = 0; j < MAP_WIDTH; j++)
                tiles[i, j] = new Tile(Tile.TILE_EMPTY);

        // Reset QuadTree
        quadTree =
            new QuadTree(new AABB(new XY(MAP_WIDTH/2.0f, MAP_HEIGHT/2.0f), new XY(MAP_WIDTH/2.0f, MAP_HEIGHT/2.0f)));

        // Reset rooms
        rooms.Clear();

        // Destroy tile GameObjects
        foreach (Transform t in containerRooms.transform) Destroy(t.gameObject);

        foreach (var b in barriers) Destroy(b);
        barriers.Clear();
        foreach (var k in keys) Destroy(k);
        keys.Clear();

        exit.SetActive(false);
    }

    // Generate a new dungeon with the given seed
	public void GenerateDungeon(int seed)
	{
		Debug.Log ("Generating QuadTree");
			
		// Clean
		ResetDungeon ();
		
		// Place a temporary floor to see progress
//		floor = GameObject.Instantiate(prefabFloor01,new Vector3(MAP_WIDTH/2,-0.5f,MAP_HEIGHT/2), Quaternion.identity) as GameObject;
//		floor.transform.localScale = new Vector3(MAP_WIDTH,1,MAP_HEIGHT);
		
		// Generate QuadTree
		GenerateQuadTree (ref quadTree);
		
		// Export texture
		Texture2D quadTreeTexture = quadTree.QuadTreeToTexture();
//		floor.renderer.material.mainTexture = quadTree.QuadTreeToTexture();
		TextureToFile(quadTreeTexture,seed + "_quadTree");

		Debug.Log ("Generating Rooms");
		
		// Generate Rooms
		GenerateRooms (ref rooms, quadTree);
		
		// Export texture
		dungeonTexture = DungeonToTexture();
//		floor.renderer.material.mainTexture = dungeonTexture;
		TextureToFile(dungeonTexture,seed + "_rooms");
		
		Debug.Log ("Generating Corridors");
		
		// Generate Corridors
		GenerateCorridors ();
		
		// Export texture
		dungeonTexture = DungeonToTexture();
//		floor.renderer.material.mainTexture = dungeonTexture;
		TextureToFile(dungeonTexture,seed + "_corridors");
		
		
		Debug.Log ("Generating Walls");
		
		GenerateWalls();
		
		// Export texture
		dungeonTexture = DungeonToTexture();
//		floor.renderer.material.mainTexture = dungeonTexture;
		TextureToFile(dungeonTexture,seed + "_walls");
		
		Debug.Log ("Generating GameObjects, this may take a while..");
		
		// Instantiate prefabs
		GenerateGameObjects(quadTree);

        int r = Random.Range(0, rooms.Count - 1);
        Room room = rooms[r];
        exit.SetActive(true);
        exit.transform.position = new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y);
        GameObject _barrier = Instantiate(barrier, new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y), Quaternion.identity) as GameObject;
        _barrier.transform.localScale *= Random.Range(ROOM_MIN_SIZE, MAP_HEIGHT / 2);
	    _barrier.renderer.material.color = Color.red;
	    _barrier.name = "Barrier 0";
	    barriers.Add(_barrier);

	    for (int i = 1; i < MAX_NUM_BARRIERS; i++)
	    {
            Color barrierType = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            _barrier = Instantiate(barrier, new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y), Quaternion.identity) as GameObject;
            _barrier.transform.localScale *= Random.Range(ROOM_MIN_SIZE, MAP_HEIGHT / 2);
            _barrier.renderer.material.color = barrierType;
	        _barrier.name = "Barrier " + i;
            barriers.Add(_barrier);
	        
	    }

	    PlaceKeys();

	    //PlacePuzzles(barriers[0]);

	    PlacePlayer();


//		GameObject.DestroyImmediate(floor);

	}

    private void PlaceKeys()
    {
        for (int i = 0; i < MAX_NUM_BARRIERS; i++)
        {
            int r = Random.Range(0, rooms.Count - 1);
            Room room = rooms[r];
            if (!barriers[i].collider.bounds.Contains(new Vector3(rooms[r].boundary.center.x, room.boundary.center.y)))
            {
                GameObject _key = Instantiate(key, new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y), Quaternion.identity) as GameObject;
                keys.Add(_key);
                keys[i].name = "Key for " + barriers[i].name;
                keys[i].renderer.material.color = barriers[i].renderer.material.color;
            }
        }
    }

    private void PlacePlayer()
    {
        int r = Random.Range(0, rooms.Count - 1);
        Room room = rooms[r];
        
        foreach (var barrier in barriers)
        {
            if(barrier.collider.bounds.Contains(new Vector3(rooms[r].boundary.center.x, room.boundary.center.y)))
            {
                PlacePlayer();
            }
        }
        player.SetActive(true);
        player.transform.position = new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y);
    }

    //private void PlacePuzzles(GameObject prevBarrier)
    //{
    //    int r = Random.Range(0, rooms.Count - 1);
    //    Room room = rooms[r];
    //    Color barrierType = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    //    if (prevBarrier.collider.bounds.Contains(new Vector3(rooms[r].boundary.center.x, room.boundary.center.y)))
    //    {
    //        PlacePuzzles(prevBarrier);
    //    }
    //    GameObject _key = Instantiate(key, new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y), Quaternion.identity) as GameObject;
    //    _key.renderer.material.color = prevBarrier.renderer.material.color;
    //    keys.Add(_key);
    //    GameObject _barrier = Instantiate(barrier, new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y), Quaternion.identity) as GameObject;
    //    _barrier.transform.localScale *= Random.Range(ROOM_MIN_SIZE, MAP_HEIGHT / 2);
    //    _barrier.renderer.material.color = barrierType;
    //    barriers.Add(_barrier);
    //    prevBarrier = _barrier;
    //    if (barriers.Count == MAX_NUM_BARRIERS)
    //    {
    //        r = Random.Range(0, rooms.Count - 1);
    //        room = rooms[r];
    //        if (prevBarrier.collider.bounds.Contains(new Vector3(rooms[r].boundary.center.x, room.boundary.center.y)))
    //        {
    //            _key = Instantiate(key, new Vector3(room.boundary.center.x, 1.0f, room.boundary.center.y), Quaternion.identity) as GameObject;
    //            _key.renderer.material.color = prevBarrier.renderer.material.color;
    //            keys.Add(_key);
    //        }
    //        return;
    //    }
    //    PlacePuzzles(prevBarrier);

    //}
	
	// Generate the quadtree system
	void GenerateQuadTree(ref QuadTree _quadTree)
	{
		_quadTree.GenerateQuadTree(seed);
	}
	
	// Generate the list of rooms and dig them
	public void GenerateRooms(ref List<Room> _rooms, QuadTree _quadTree)
	{
		// Childless node
		if (_quadTree.northWest == null && _quadTree.northEast == null && _quadTree.southWest == null && _quadTree.southEast == null)
		{
			_rooms.Add(GenerateRoom(_quadTree));
			return;
		}
		
		// Recursive call
		if (_quadTree.northWest != null) GenerateRooms (ref _rooms,_quadTree.northWest);
		if (_quadTree.northEast != null) GenerateRooms (ref _rooms,_quadTree.northEast);
		if (_quadTree.southWest != null) GenerateRooms (ref _rooms,_quadTree.southWest);
		if (_quadTree.southEast != null) GenerateRooms (ref _rooms,_quadTree.southEast);
	}
	
	// Generate a single room
	public Room GenerateRoom(QuadTree _quadTree)
	{
		// Center of the room
		XY roomCenter = new XY();
		roomCenter.x = Random.Range(ROOM_WALL_BORDER + _quadTree.boundary.Left() + ROOM_MIN_SIZE/2.0f, _quadTree.boundary.Right() - ROOM_MIN_SIZE/2.0f - ROOM_WALL_BORDER);
		roomCenter.y = Random.Range(ROOM_WALL_BORDER + _quadTree.boundary.Bottom() + ROOM_MIN_SIZE/2.0f, _quadTree.boundary.Top() - ROOM_MIN_SIZE/2.0f - ROOM_WALL_BORDER);		
		
		// Half size of the room
		XY roomHalf = new XY();
		
		float halfX = (_quadTree.boundary.Right() - roomCenter.x - ROOM_WALL_BORDER);
		float halfX2 =(roomCenter.x - _quadTree.boundary.Left() - ROOM_WALL_BORDER);
		if (halfX2 < halfX) halfX = halfX2;
		if (halfX > ROOM_MAX_SIZE/2.0f) halfX = ROOM_MAX_SIZE/2.0f;
		
		float halfY = (_quadTree.boundary.Top() - roomCenter.y - ROOM_WALL_BORDER);
		float halfY2 =(roomCenter.y - _quadTree.boundary.Bottom() - ROOM_WALL_BORDER);
		if (halfY2 < halfY) halfY = halfY2;
		if (halfY > ROOM_MAX_SIZE/2.0f) halfY = ROOM_MAX_SIZE/2.0f;
		
		roomHalf.x = Random.Range((float)ROOM_MIN_SIZE/2.0f,halfX);
		roomHalf.y = Random.Range((float)ROOM_MIN_SIZE/2.0f,halfY);

		// Eliminate ugly zones
		if (ROOM_UGLY_ENABLED == false) 
		{
			float aspect_ratio = roomHalf.x / roomHalf.y;
			if (aspect_ratio > ROOM_MAX_RATIO || aspect_ratio < 1.0f/ROOM_MAX_RATIO) return GenerateRoom(_quadTree); 
		}
		
		// Create AABB
		AABB randomAABB = new AABB(roomCenter, roomHalf);
		
		// Dig the room in our tilemap
		DigRoom (randomAABB.BottomTile(), randomAABB.LeftTile(), randomAABB.TopTile()-1, randomAABB.RightTile()-1);
		
		// Return the room
		return new Room(randomAABB,_quadTree);
	}
	
	void GenerateCorridors()
	{
		quadTree.GenerateCorridors();
	}

	// Generate walls when there's something near
	public void GenerateWalls()
	{
		// Place walls
		for (int i = 0; i < MAP_HEIGHT; i++)
		{
			for (int j = 0; j < MAP_WIDTH; j++)
			{
				bool room_near = false;
				if (IsPassable(i,j)) continue;
				if (i > 0) if (IsPassable(i-1,j)) room_near = true;
				if (i < MAP_HEIGHT-1) if (IsPassable(i+1,j)) room_near = true;
				if (j > 0) if (IsPassable(i,j-1)) room_near = true;
				if (j < MAP_WIDTH-1) if (IsPassable(i,j+1)) room_near = true;
				if (room_near) SetWall(i,j);
			}
		}
	}
	
	// Read tilemap and instantiate GameObjects
	void GenerateGameObjects(QuadTree _quadtree)
	{
		// If it's an end quadtree, read every pos and make a chunk of combined meshes
		if (_quadtree.HasChildren() == false)
		{
			GameObject container = GameObject.Instantiate(meshCombiner) as GameObject;
			for (int row = _quadtree.boundary.BottomTile(); row <= _quadtree.boundary.TopTile()-1; row++)
			{
				for (int col = _quadtree.boundary.LeftTile(); col <= _quadtree.boundary.RightTile()-1; col++)
				{
					int id = tiles[row,col].id;
					if (id == Tile.TILE_ROOM || id == Tile.TILE_CORRIDOR)
					{
						GameObject floor = GameObject.Instantiate(prefabFloor01,new Vector3(col,0.0f,row),Quaternion.identity) as GameObject;
						floor.transform.parent = container.transform;
					}
					else if (id == Tile.TILE_WALL)
					{
						GameObject wall = GameObject.Instantiate(prefabWall01,new Vector3(col,1.5f,row),Quaternion.identity) as GameObject;
						wall.transform.parent = container.transform;
					}
				}
			}
			container.transform.parent = containerRooms.transform;
		}
		else 
		{
				GenerateGameObjects(_quadtree.northWest);
				GenerateGameObjects(_quadtree.northEast);
				GenerateGameObjects(_quadtree.southWest);
				GenerateGameObjects(_quadtree.southEast);
		}

	}
	
	void PaintDungeonTexture(ref Texture2D t)
	{
		for (int i = 0; i < MAP_WIDTH; i++) for (int j = 0; j < MAP_HEIGHT; j++) 
		{
			switch (tiles[j,i].id)
			{
			case Tile.TILE_EMPTY:
				t.SetPixel(i,j,Color.black);
				break;
			case Tile.TILE_ROOM:
				t.SetPixel(i,j,Color.white);
				break;
			case Tile.TILE_CORRIDOR:
				t.SetPixel(i,j,Color.grey);
				break;
			case Tile.TILE_WALL:
				t.SetPixel(i,j,Color.blue);
				break;
			}
		}

	}
	
	Texture2D DungeonToTexture()
	{
		Texture2D texOutput = new Texture2D((int) (MAP_WIDTH), (int) (MAP_HEIGHT),TextureFormat.ARGB32, false);
		PaintDungeonTexture(ref texOutput);
		texOutput.filterMode = FilterMode.Point;
		texOutput.wrapMode = TextureWrapMode.Clamp;
		texOutput.Apply();
		return texOutput;
	}
	
	// Helper Methods
	public bool IsEmpty(int row, int col) { return tiles[row,col].id == Tile.TILE_EMPTY; }
	
	public bool IsPassable(int row, int col) { return tiles[row,col].id == Tile.TILE_ROOM || tiles[row,col].id == Tile.TILE_CORRIDOR; }
	
	public bool IsPassable(XY xy) { return IsPassable((int) xy.y, (int) xy.x);}
	
	public void SetWall(int row, int col)
	{
		tiles[row,col].id = Tile.TILE_WALL;
	}

	// Dig a room, placing floor tiles
	public void DigRoom(int row_bottom, int col_left, int row_top, int col_right)
	{
		// Out of range
		if ( row_top < row_bottom ) 
		{
		    int tmp = row_top;
		    row_top = row_bottom;
		    row_bottom = tmp;
		}
		
		// Out of range
		if ( col_right < col_left ) 
		{
		    int tmp = col_right;
		    col_right = col_left;
		    col_left = tmp;
		}
		
		if (row_top > MAP_HEIGHT-1) return;
		if (row_bottom < 0) return;
		if (col_right > MAP_WIDTH-1) return;
		if (col_left < 0) return;
		
		// Dig floor
	    for (int row = row_bottom; row <= row_top; row++) 
	        for (int col = col_left; col <= col_right; col++) 
	            DigRoom (row,col);
	}
	
	public void DigRoom(int row, int col)
	{
		 tiles[row,col].id = Tile.TILE_ROOM;
	}
	
	public void DigCorridor(int row, int col)
	{
		 tiles[row,col].id = Tile.TILE_CORRIDOR;
	}
	
	public void DigCorridor(XY p1, XY p2)
	{
		int row1 = Mathf.RoundToInt(p1.y);
		int row2 = Mathf.RoundToInt(p2.y);
		int col1 = Mathf.RoundToInt(p1.x);
		int col2 = Mathf.RoundToInt(p2.x);
		
		DigCorridor(row1,col1,row2,col2);
	}
	
	public void DigCorridor(int row1, int col1, int row2, int col2)
	{		
		if (row1 <= row2)
		{
			for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
				for (int row = row1; row <= row2; row++)
					DigCorridor(row,col);
		}
		else 
		{
			for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
				for (int row = row2; row <= row1; row++)
					DigCorridor(row,col);
		}
		
		if (col1 <= col2)
		{
			for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
				for (int col = col1; col <= col2; col++)
					DigCorridor(row,col);
		}
		else 
		{
			for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
				for (int col = col2; col <= col1; col++)
					DigCorridor(row2,col);
		}
	}
	
	// Export a texture to a file
	public void TextureToFile(Texture2D t, string filename)
	{
		byte[] bytes = t.EncodeToPNG();
		FileStream myFile = new FileStream(Application.dataPath + "/Resources/Generated/" + filename + ".png",FileMode.OpenOrCreate,System.IO.FileAccess.ReadWrite);
		myFile.Write(bytes,0,bytes.Length);
		myFile.Close();
	}
	
}
