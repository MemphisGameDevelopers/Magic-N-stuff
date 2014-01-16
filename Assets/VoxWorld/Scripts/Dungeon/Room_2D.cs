using UnityEngine;
using System.Collections;

[System.Serializable]
public class Room_2D
{
	
		public AABB boundary;
		public QuadTreeMapLayer quadtree;
	
		public Room_2D (AABB b)
		{
				boundary = b;
		}
	
		public Room_2D (AABB b, QuadTreeMapLayer q)
		{
				boundary = b;
				quadtree = q;
				quadtree.room = this;
		}
	
}
