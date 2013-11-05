using UnityEngine;
using System.Collections;

[System.Serializable]
public class Room {

	public AABB boundary;
	public QuadTree quadtree;
	
	public Room (AABB b)
	{
		boundary = b;
	}
	
	public Room (AABB b, QuadTree q)
	{
		boundary = b;
		quadtree = q;
		quadtree.room = this;
	}
	
}
