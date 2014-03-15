using System;
using UnityEngine;

public class WorldGeneration
{
		private static volatile WorldGeneration instance;
	
		public static int groundHeight = 75;
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
		
		public void generateRegion (Region babyRegion)
		{
				//Create base level 
				createBaseGround (babyRegion.data, babyRegion.regionY);
				
				//Create basic terrain (heightmap)
				babyRegion.flags = createPerlin2D (babyRegion.data, babyRegion.getBlockOffsetX (), babyRegion.getBlockOffsetY (), babyRegion.getBlockOffsetZ ());
				
				TreePlanter.generateTrees (babyRegion);
				
				//Create overhangs
				//createPerlin3D (data, regionX, regionY, regionZ);
		}
		private RegionFlags createPerlin2D (byte[,,] data, int regionX, int regionY, int regionZ)
		{
				RegionFlags flags = new RegionFlags ();
				for (int x=0; x<data.GetLength(0); x++) {
						for (int z=0; z<data.GetLength(2); z++) {
								int xi = regionX + x;
								int zi = regionZ + z;
						

								int stone = PerlinNoise (xi, 751, zi, 550, 7f, 3f);
								int lowHills = PerlinNoise (xi, 400, zi, 200, 6f, 3f);
								int roughness1 = PerlinNoise (xi, 231, zi, 20, 2f, 2.5f);
								int roughness2 = PerlinNoise (xi, 845, zi, 50, 2f, 1.5f);
								int roughness3 = PerlinNoise (xi, 19, zi, 100, 3f, 3.5f);
								int val = stone + lowHills + roughness1 + roughness2 + roughness3;
								for (int y=0; y<data.GetLength(1); y++) {
										int yi = regionY + y;
										if (yi <= val) {
												data [x, y, z] = 1;
												flags.isEmptyAir = false;
										}
					
								}
						}
				}
				return flags;
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
										if (yi < groundHeight) {
												data [x, y, z] = 1;
										} else {
												data [x, y, z] = 0;
										}
								}
						}
				}	
		}
		
		private void placeTrees (Region region)
		{
				
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
	
}

