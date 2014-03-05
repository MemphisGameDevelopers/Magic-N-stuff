using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleSQL;

public class DB_Tester : MonoBehaviour
{

		public SimpleSQL.SimpleSQLManager dbManager;
		private bool run = true;
		// Use this for initialization
		void OnGUI ()
		{
				
		}
	
	
		private void createTables ()
		{
				string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='ChunkDataDO' ";

				SimpleSQL.SimpleDataTable dt = dbManager.QueryGeneric (sql);
				
				bool found = false;
				foreach (SimpleSQL.SimpleDataRow dr in dt.rows) {
						if (dr [0].ToString () == "ChunkDataDO") {
								found = true;
								break;
						}
				}
				if (!found) {
						sql = "CREATE TABLE 'ChunkDataDO' " + 
								"('x' INTEGER NOT NULL, " + 
								" 'y' INTEGER NOT NULL, " + 
								" 'z' INTEGER NOT NULL, " + 
								" 'blocks' BLOB NOT NULL," + 
								" PRIMARY KEY ( 'x','y','z')) ";

						dbManager.Execute (sql);
				}
				//Insert a chunk
				ChunkData.ChunkDataDO d = new ChunkData.ChunkDataDO ();
				d.x = 2;
				d.y = 2;
				d.z = 2;
				d.blocks = "testset";
				dbManager.Insert (d);
				
				List<ChunkData.ChunkDataDO> data = dbManager.Query<ChunkData.ChunkDataDO> ("SELECT * FROM ChunkDataDO");
				foreach (ChunkData.ChunkDataDO chunk in data) {
						Debug.Log (chunk.x + "," + chunk.y + "," + chunk.z);
				}
				
		}
		public void test ()
		{
				
		}
		
		void Update ()
		{
				if (run) {
						run = false;
						createTables ();
				}
		}
		
		//CREATE  TABLE "main"."regions" ("x" INTEGER NOT NULL , "y" INTEGER NOT NULL , "z" INTEGER NOT NULL , "blocks" BLOB, PRIMARY KEY ("x", "y", "z"))
}
