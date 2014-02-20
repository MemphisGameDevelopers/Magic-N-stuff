using System;
using UnityEngine;

public interface VoxelStream
{

		void create ();
		byte GetBlockAtRelativeCoords (int x, int y, int z);
		byte[,,] GetAllBlocks ();
		Vector3 getBounds ();
	
}


