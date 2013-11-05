using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuadTree {
	// Pointer to dungeon instance
	private DungeonGenerator dungeon;
	
	// The Random Seed
	public int seed;
	
	// QuadTree boundary
	public AABB boundary;
	
	// Children
	public QuadTree parent;
	public QuadTree northWest;
	public QuadTree northEast;
	public QuadTree southWest;
	public QuadTree southEast;
	
	// Aux
	private int roomRealMinSize;
	
	// Room it contains
	public Room room;
	
	// Constructor
	public QuadTree(AABB _aabb)
	{
		boundary = _aabb;
		dungeon = DungeonGenerator.instance;
		roomRealMinSize = dungeon.ROOM_MIN_SIZE + dungeon.ROOM_WALL_BORDER*2; // room floor size + "BORDER" walls for each axis
	}
	
	/*
	 * METHODS
	 */
	
	// Clean QuadTree
	public void Clear()
	{
		seed = -1;
		boundary = new AABB();
		northWest = null;
		northEast = null;
		southWest = null;
		southEast = null;
		room = null;
	}
	
	// Create a random slice inside the QuadTree boundary
	public bool RandomSlice(int numTry, int maxTries, ref XY output)
	{
		if (numTry > maxTries) return false;
		
		// Get a random slice point
		float sliceX = Mathf.Round(Random.Range(boundary.Left(), boundary.Right()));
		float sliceY = Mathf.Round(Random.Range(boundary.Bottom(), boundary.Top()));
		
		// Check if we can fit 4 rooms using that slice, if not just try again until maxTries
		if (Mathf.Abs(sliceX - boundary.Left ()) < roomRealMinSize) return RandomSlice(numTry+1,maxTries,ref output);
		if (Mathf.Abs(boundary.Right() - sliceX) < roomRealMinSize) return RandomSlice(numTry+1,maxTries,ref output);
		if (Mathf.Abs(boundary.Top () - sliceY) < roomRealMinSize) return RandomSlice(numTry+1,maxTries,ref output);
		if (Mathf.Abs(sliceY - boundary.Bottom()) < roomRealMinSize) return RandomSlice(numTry+1,maxTries,ref output);
				
		output = new XY(sliceX, sliceY);
		return true;
	}
	
	// Procedurally generate QuadTrees
	// Make a maximum of "max_depth" levels of depth
	public void GenerateZones(int depth, int max_depth)
	{
		// Reached max depth
		if (depth > max_depth) return;
		
		// No place for 4 rooms? return
		if (boundary.half.x < roomRealMinSize) return;
		if (boundary.half.y < roomRealMinSize) return;
		
		// Random possibilty of not generating zone
		int r = Random.Range(0,100);
		if (r < dungeon.CHANCE_STOP && depth > 1) return;
		
		// Try to make a slice with space for 4 rooms
		// Retry "N=SLICE_TRIES" times
		XY slice = new XY();
		bool new_slice = RandomSlice(1,dungeon.SLICE_TRIES,ref slice);
		if (new_slice == false) return;

		// Generate children QuadTrees
		// NORTHWEST
		AABB aabb1 = new AABB();
		XY size = new XY(slice.x - boundary.Left (), boundary.Top() - slice.y);
		aabb1.center = new XY(slice.x - size.x/2.0f, slice.y + size.y/2.0f);
		aabb1.half = new XY(size.x/2.0f,size.y/2.0f);
		northWest = new QuadTree(aabb1);
		northWest.parent = this;
		northWest.GenerateZones(depth+1,max_depth);
		// NORTHEAST
		AABB aabb2 = new AABB();
		size = new XY(boundary.Right() - slice.x, boundary.Top() - slice.y);
		aabb2.center = new XY(slice.x + size.x/2.0f, slice.y + size.y/2.0f);
		aabb2.half = new XY(size.x/2.0f,size.y/2.0f);
		northEast = new QuadTree(aabb2);
		northEast.parent = this;
		northEast.GenerateZones(depth+1,max_depth);
		// SOUTHWEST
		AABB aabb3 = new AABB();
		size = new XY(slice.x - boundary.Left (), slice.y - boundary.Bottom());
		aabb3.center = new XY(slice.x - size.x/2.0f, slice.y - size.y/2.0f);
		aabb3.half = new XY(size.x/2.0f,size.y/2.0f);
		southWest = new QuadTree(aabb3);
		southWest.parent = this;
		southWest.GenerateZones(depth+1,max_depth);
		// SOUTEAST
		AABB aabb4 = new AABB();
		size = new XY(boundary.Right() - slice.x, slice.y - boundary.Bottom());
		aabb4.center = new XY(slice.x + size.x/2.0f, slice.y - size.y/2.0f);
		aabb4.half = new XY(size.x/2.0f,size.y/2.0f);
		southEast = new QuadTree(aabb4);
		southEast.parent = this;
		southEast.GenerateZones(depth+1,max_depth);
	}
	
	public void GenerateCorridors()
	{
		// got a room, nothing to do
		if (room != null) return;
		// quadtree with 4 rooms, connect them
		if (HasChildrenWithRoom())
		{
			// Get rooms center
			XY nw_center = northWest.room.boundary.center;
			XY ne_center = northEast.room.boundary.center;
			XY sw_center = southWest.room.boundary.center;
			XY se_center = southEast.room.boundary.center;
			
			// Dig the 4 paths connecting the rooms
			dungeon.DigCorridor(nw_center, ne_center);
			dungeon.DigCorridor(ne_center, se_center);
			dungeon.DigCorridor(se_center, sw_center);
			dungeon.DigCorridor(sw_center, nw_center);
		}
		else
		{
			// Dig a corridor from one of my rooms to the closest neighbour room
			dungeon.DigCorridor(northWest.GetClosestRoomCenter(boundary.center), northEast.GetClosestRoomCenter(boundary.center));
			dungeon.DigCorridor(northEast.GetClosestRoomCenter(boundary.center), southEast.GetClosestRoomCenter(boundary.center));
			dungeon.DigCorridor(southEast.GetClosestRoomCenter(boundary.center), southWest.GetClosestRoomCenter(boundary.center));
			dungeon.DigCorridor(southWest.GetClosestRoomCenter(boundary.center), northWest.GetClosestRoomCenter(boundary.center));
			
			// Recursive call to children
			northWest.GenerateCorridors();
			northEast.GenerateCorridors();
			southWest.GenerateCorridors();
			southEast.GenerateCorridors();	
		}
	}
	
	// Get the center of any room inside this QuadTree
	XY GetAnyRoomCenter()
	{
		// Check room
		if (room != null) return room.boundary.center;
		
		// Take a random child
		if (HasChildren() == false) return null;
		int r = Random.Range(1,4);
		switch(r)
		{
		case 1:
			if (northWest != null) return northWest.GetAnyRoomCenter();
			else return GetAnyRoomCenter();
		case 2:
			if (northEast != null) return northEast.GetAnyRoomCenter();
			else return GetAnyRoomCenter();
		case 3:
			if (southWest != null) return southWest.GetAnyRoomCenter();
			else return GetAnyRoomCenter();
		case 4:
			if (southEast != null) return southEast.GetAnyRoomCenter();
			else return GetAnyRoomCenter();
		}	
		return null;
	}
	
	// Get the center of the closest room inside this QuadTree
	XY GetClosestRoomCenter(XY closeToThis)
	{
		// Check room
		if (room != null) return room.boundary.center;
		
		// Take a random child
		if (HasChildren() == false) return null;
		float dist1 = XY.Distance(northWest.GetClosestRoomCenter(closeToThis), closeToThis);
		float dist2 = XY.Distance(northEast.GetClosestRoomCenter(closeToThis),closeToThis);
		float dist3 = XY.Distance(southWest.GetClosestRoomCenter(closeToThis),closeToThis);
		float dist4 = XY.Distance(southEast.GetClosestRoomCenter(closeToThis),closeToThis);
		
		int r = 1;
		float closest = dist1;
		if (dist2 < closest) 
		{
			r = 2;
			closest = dist2;
		}
		if (dist3 < closest) 
		{	
			r = 3;
			closest = dist3;
		}
		if (dist4 < closest) 
		{
			r = 4;
			closest = dist4;
		}
		
		switch(r)
		{
		case 1:
			return northWest.GetClosestRoomCenter(closeToThis);
		case 2:
			return northEast.GetClosestRoomCenter(closeToThis);
		case 3:
			return southWest.GetClosestRoomCenter(closeToThis);
		case 4:
			return southEast.GetClosestRoomCenter(closeToThis);
		}	
		return null;
	}
	
	// Symmetric division
	public void Subdivide(int level, int maxLevels)
	{
		if (level > maxLevels) return;
		northWest = new QuadTree(new AABB(new XY ((boundary.center.x - boundary.half.x/2), (boundary.center.y + boundary.half.y/2)), boundary.half/2));
		northEast = new QuadTree(new AABB(new XY ((boundary.center.x + boundary.half.x/2), (boundary.center.y + boundary.half.y/2)), boundary.half/2));
		southWest = new QuadTree(new AABB(new XY ((boundary.center.x - boundary.half.x/2), (boundary.center.y - boundary.half.y/2)), boundary.half/2));
		southEast = new QuadTree(new AABB(new XY ((boundary.center.x + boundary.half.x/2), (boundary.center.y - boundary.half.y/2)), boundary.half/2));
		northWest.Subdivide(level+1, maxLevels);
		northEast.Subdivide(level+1, maxLevels);
		southWest.Subdivide(level+1, maxLevels);
		southEast.Subdivide(level+1, maxLevels);
	}

	// Start our QuadTree algorithm
	public void GenerateQuadTree(int s)
	{
		seed = s;
		Debug.Log("Generating QuadTree with seed " + seed);
		GenerateZones(1,dungeon.MAX_DEPTH);
	}
	
	// Print a QuadTree to Texture
	public void PaintQuadTree(ref Texture2D output)
	{
		Color color = new Color(Random.Range (0.0f,1.0f), Random.Range (0.0f,1.0f),Random.Range(0.0f,1.0f));
		for (int x = (int)boundary.Bottom(); x < (int)boundary.Top(); x++) // From bottom to top
			for (int y = (int)boundary.Left(); y < (int)boundary.Right(); y++) // From left to right
				output.SetPixel(y,x,color);
		
		if (northWest != null) northWest.PaintQuadTree(ref output);
		if (northEast != null) northEast.PaintQuadTree(ref output);
		if (southWest != null) southWest.PaintQuadTree(ref output);
		if (southEast != null) southEast.PaintQuadTree(ref output);
	} 
	
	// Convert our dungeon to a texture
	public Texture2D QuadTreeToTexture()
	{
		Texture2D texOutput = new Texture2D((int) (dungeon.MAP_WIDTH), (int) (dungeon.MAP_HEIGHT),TextureFormat.ARGB32, false);
		PaintQuadTree(ref texOutput );
		texOutput.filterMode = FilterMode.Point;
		texOutput.wrapMode = TextureWrapMode.Clamp;
		texOutput.Apply();
		return texOutput;
	}
	
	// Has children QuadTree
	public bool HasChildren()
	{
		return northWest != null || northEast != null || southWest != null || southEast != null;
	}
	
	// All children have rooms
	public bool HasChildrenWithRoom()
	{
		if (!HasChildren()) return false;
		if (northWest.room != null && northEast.room != null && southWest.room != null && southEast.room != null) return true;
		return false;
	}
	
}
