using UnityEngine;
using System.Collections;

namespace zelda
{
    public class Dungeon : MonoSingleton<Dungeon>
    {
        // Dungeon Rooms
        public int DUNGEON_SIZE_X = 20;
        public int DUNGEON_SIZE_Y = 20;

        // Size of 3D Model Prefab in World Space
        public int ROOM_SIZE_X = 14;
        public int ROOM_SIZE_Z = 9;

        // Demo Room Prefab
        public GameObject RoomBasicPrefab;

        // Room structure
        public Room[,] rooms;

        // Pointer to Boss Room "Demo" GameObject
        private GameObject bossRoom;

        public override void Init()
        {
            GenerateDungeon();
            GenerateGameRooms();

            // Camera looking at Boss Room for the demo
            Camera.main.transform.position = new Vector3(bossRoom.transform.position.x, 10,
                                                         bossRoom.transform.position.z - 10);
        }

        private void Update()
        {

        }

        public void GenerateDungeon()
        {
            // Create room structure
            rooms = new Room[DUNGEON_SIZE_X,DUNGEON_SIZE_Y];

            // Create our first room at a random position
            int roomX = Random.Range(0, DUNGEON_SIZE_X);
            int roomY = Random.Range(0, DUNGEON_SIZE_Y);

            Room firstRoom = AddRoom(null, roomX, roomY); // null parent because it's the first node

            // Generate childrens
            firstRoom.GenerateChildren();
        }

        private void GenerateGameRooms()
        {
            // For each room in our matrix generate a 3D Model from Prefab
            foreach (Room room in rooms)
            {
                if (room == null) continue;

                // Real world position
                float worldX = room.x*ROOM_SIZE_X;
                float worldZ = room.y*ROOM_SIZE_Z;

                GameObject g =
                    GameObject.Instantiate(RoomBasicPrefab, new Vector3(worldX, 0, worldZ), Quaternion.identity) as
                    GameObject;

                // Add the room info to the GameObject main script (Demo)
                GameRoom gameRoom = g.GetComponent<GameRoom>();
                gameRoom.room = room;

                if (room.IsFirstNode())
                {
                    bossRoom = g;
                    g.name = "Boss Room";
                }
                else g.name = "Room " + room.x + " " + room.y;
            }
        }

        // Helper Methods


        public Room AddRoom(Room parent, int x, int y)
        {
            Room room = new Room(parent, x, y);
            rooms[x, y] = room;
            return room;
        }
    }
}
