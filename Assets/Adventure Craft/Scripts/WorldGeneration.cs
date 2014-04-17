using System;
using UnityEngine;
using System.Collections.Generic;

public class WorldGeneration
{
		private static volatile WorldGeneration instance;
	
		public static int seaLevel = 50;
		public int treeHeightLine = 150;
		private WorldGeneration ()
		{

		}

		public static WorldGeneration Instance {
				get {
						if (instance == null) 
								instance = new WorldGeneration ();
						return instance;
				}
		}
		
		public void genRegionv2 (Region region)
		{
				BlockCounter counter = new BlockCounter (Region.regionXZ * Region.regionXZ * Region.regionY);
				int x_offset = region.getBlockOffsetX ();
				int y_offset = region.getBlockOffsetY ();
				int z_offset = region.getBlockOffsetZ ();
				for (int x=0; x<Region.regionXZ; x++) {
						for (int z=0; z<Region.regionXZ; z++) {
								int xi = x_offset + x;
								int zi = z_offset + z;
						
								//Read a tile from the overland map.
								
						}
				}
		}
		public void generateRegion (Region babyRegion)
		{
				//Create base level 
				//createBaseGround (babyRegion.data, babyRegion.getBlockOffsetY ());
				
				//Create basic terrain (heightmap)
				createPlains (babyRegion);
				
				//TreePlanter.generateTrees (babyRegion);
				
				//Create overhangs
				//createPerlin3D (data, regionX, regionY, regionZ);
		}
		
		private void createMountains (Region region)
		{
				BlockCounter counter = new BlockCounter (Region.regionXZ * Region.regionXZ * Region.regionY);
				int x_offset = region.getBlockOffsetX ();
				int y_offset = region.getBlockOffsetY ();
				int z_offset = region.getBlockOffsetZ ();
				for (int x=0; x<Region.regionXZ; x++) {
						for (int z=0; z<Region.regionXZ; z++) {
								int xi = x_offset + x;
								int zi = z_offset + z;
						

								int stone = PerlinNoise (xi, 751, zi, 550, 15f, 2.2f) + 200;
								int lowHills = PerlinNoise (xi, 400, zi, 200, 10f, 2.2f);
								int roughness1 = PerlinNoise (xi, 231, zi, 20, 2f, 1.5f);
								int roughness2 = PerlinNoise (xi, 845, zi, 50, 2f, 2.5f);
								int roughness3 = PerlinNoise (xi, 19, zi, 100, 3f, 3.5f);
								int val = stone + lowHills + roughness1 + roughness2 + roughness3;
								for (int y=0; y<Region.regionY; y++) {
										int yi = y_offset + y;
										if (yi <= val) {
												region.data [x, y, z] = 1;
												counter.add (1);
										} else {
												counter.add (0);
										}
					
								}
						}
				}
				
				//process flags
				if (counter.isAirOnly ()) {
						region.flags.isEmptyAir = true;
				} else if (counter.isSingleSolidOnly ()) {
						region.flags.isSingleSolid = true;
				}

		}
		
		private void createPlains (Region region)
		{
				BlockCounter counter = new BlockCounter (Region.regionXZ * Region.regionXZ * Region.regionY);
				int x_offset = region.getBlockOffsetX ();
				int y_offset = region.getBlockOffsetY ();
				int z_offset = region.getBlockOffsetZ ();
				for (int x=0; x<region.data.GetLength(0); x++) {
						for (int z=0; z<region.data.GetLength(2); z++) {
								int xi = x_offset + x;
								int zi = z_offset + z;
				
				
								int highHills = PerlinNoise (xi, 751, zi, 50, 10f, 1.0f) + 128;
								int lowHills = PerlinNoise (xi, 407, zi, 50, 10f, 1.0f) + 128;
								//int val = (int)Math.Max (highHills, lowHills);
								for (int y=0; y<region.data.GetLength(1); y++) {
										int yi = y_offset + y;
										if (yi <= highHills) {
												counter.add (DirtBlock.blockId);
												region.data [x, y, z] = DirtBlock.blockId;
										} else if (yi <= lowHills) {
												counter.add (GrassBlock.blockId);
												region.data [x, y, z] = GrassBlock.blockId;
										} else {
												counter.add (0);
										}
					
								}
						}
				}
				
				//process flags
				if (counter.isAirOnly ()) {
						region.flags.isEmptyAir = true;
				} else if (counter.isSingleSolidOnly ()) {
						region.flags.isSingleSolid = true;
				}
		}
	
		private void createPerlin3D (byte[,,] data, int regionX, int regionY, int regionZ)
		{
				for (int x=0; x<data.GetLength(0); x++) {
						for (int z=0; z<data.GetLength(2); z++) {
								for (int y = 0; y<data.GetLength(1); y++) {
										int xi = regionX + x;
										int yi = regionY + y;
										int zi = regionZ + z;
				
				
										int stone = PerlinNoise (xi, yi, zi, 250, 10f, 0F);

										if (yi >= 1 && stone > 5) {
												data [x, y, z] = 0;
										}
					
								}
						}
				}
		}

		private void createBaseGround (byte[,,] data, int regionY)
		{
				for (int x=0; x<data.GetLength(0); x++) {
						for (int z=0; z<data.GetLength(2); z++) {
								for (int y=0; y<data.GetLength(1); y++) {

										int yi = regionY + y;
										if (yi < seaLevel) {
												data [x, y, z] = 1;
										} else {
												data [x, y, z] = 0;
										}
								}
						}
				}	
		}

		private int PerlinNoise (float x, int y, float z, float scale, float height, float power)
		{
				float rValue;
				rValue = TutorialNoise.GetNoise (((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
				rValue *= height;
		
				if (power != 0) {
						rValue = Mathf.Pow (rValue, power);
				}
		
				return (int)rValue;
		}
	
		private class BlockCounter
		{
				private Dictionary<int, int> blocks;
				private int regionVolume = 0;
				public BlockCounter (int inVolume)
				{
						regionVolume = inVolume;
						blocks = new Dictionary<int, int> (10);
				}
				
				public void add (int blockType)
				{
						if (!blocks.ContainsKey (blockType)) {
								blocks.Add (blockType, 1);
						} else {
								int currentCount = blocks [blockType];
								blocks.Remove (blockType);
								blocks.Add (blockType, currentCount + 1);
						}
				}
				
				public bool isAirOnly ()
				{
						if (blocks.Count > 1) {
								return false;  					//There are at least two block types 
						} else if (blocks.ContainsKey (0)) {
								int airCount = blocks [0];		//Air block type
								if (airCount == regionVolume) {
										return true;
								} else {
										return false;
								}
						} else {
								return false;
						}
				}
				
				public bool isSingleSolidOnly ()
				{
						if (blocks.Count > 1) {
								return false;  //contains at least two block types.
						} else {
								foreach (KeyValuePair<int,int> kvp in blocks) {
										if (kvp.Key != 0 && kvp.Value == regionVolume) {
												return true;
										}
								}
								return false;
						}
				
				}
				
		}
}

