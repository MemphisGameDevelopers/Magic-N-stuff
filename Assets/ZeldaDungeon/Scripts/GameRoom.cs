using UnityEngine;
using System.Collections;

namespace zelda
{
    public class GameRoom : MonoBehaviour
    {

        public GameObject doorWest, doorEast, doorNorth, doorSouth;
        public Room room;

        private void Start()
        {
            // Remove walls if connected
            if (room.IsConnectedTo(room.GetLeft())) doorWest.SetActive(false);
            else doorWest.SetActive(true);

            if (room.IsConnectedTo(room.GetRight())) doorEast.SetActive(false);
            else doorEast.SetActive(true);

            if (room.IsConnectedTo(room.GetTop())) doorNorth.SetActive(false);
            else doorNorth.SetActive(true);

            if (room.IsConnectedTo(room.GetBottom())) doorSouth.SetActive(false);
            else doorSouth.SetActive(true);
        }

        private void Update()
        {

        }
    }
}