using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractBlock
{

		public abstract int  getID ();
		public abstract int  render (Chunk chunk,
	                          int x, int y, int z, 
	                          List<Vector3> newVertices, 
	                          List<int> newTriangles,
	                          List<Vector2> newUV,
	                          int faceCount);
	
		protected static string getHash (int x, int y, int z)
		{
				return x + "," + y + "," + z;
		}
}
