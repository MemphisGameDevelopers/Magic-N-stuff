using System;
using UnityEngine;

public interface VoxelStream
{

		void create (object o);
		byte GetBlockAtRelativeCoords (int x, int y, int z);
		byte[,,] GetAllBlocks ();
		Vector3 getBounds ();
	
}


