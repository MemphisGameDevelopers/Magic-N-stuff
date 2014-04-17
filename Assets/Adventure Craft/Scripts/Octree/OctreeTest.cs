//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;

public class OctreeTest : MonoBehaviour
{
		private Octree tree;
		public int initialSize = 512;
		private int[,,] data;
		
		void Start ()
		{
				createVoxelData ();
				tree = new Octree (initialSize, new Vector3 (0.0f, 0.0f, 0.0f));
		}
		
		private void createVoxelData ()
		{
				data = new int[initialSize, initialSize, initialSize];
				int height = 0;
				for (int x=0; x<data.GetLength(0); x++) {
						for (int z=0; z<data.GetLength(2); z++) {
								for (int y=0; y<data.GetLength(1); y++) {
										if (y <= height) {
												data [x, y, z] = 1;
										} else {
												data [x, y, z] = 0;
										}
								}	
						}
						height += 1;
				}	
		}
}


