using UnityEngine;
using System.Collections;
using DarkRift;

namespace Roland
{
    public class RoomController : MonoBehaviour
    {
        public Transform thePlayerBoxesParent;
        GameObject[] thePlayerBoxes;
        int currentPlayerCount;
        // Use this for initialization
        void Start()
        {
            DarkRiftAPI.onData += ReceiveData; 
            currentPlayerCount = 0;
            for (int i = 0; i < thePlayerBoxesParent.child; i++)
            {
                thePlayerBoxes[i] = thePlayerBoxesParent.GetChild(i).gameObject;
                thePlayerBoxes[i].SetActive(false);
            }

            DarkRiftAPI.SendMessageToAll(NetworkingTags.Room, NetworkingTags.RoomSubjects.JoinRoom, "");
        }
        
        public int AddPlayer()
        {
            if(currentPlayerCount > thePlayerBoxesParent.childCount )
            {
                return -1;
            }
            thePlayerBoxes[currentPlayerCount].SetActive(true);
            return currentPlayerCount++;
        }

        public void RemovePlayer(int i)
        {
            if(i >= 0 && i < thePlayerBoxesParent.childCount)
            {
                thePlayerBoxes[currentPlayerCount].SetActive(false);
            }
        }

        void ReceiveData(byte tag, ushort subject, object data)
        {
            if(tag == NetworkingTags.Room)
            {
                if(subject == NetworkingTags.RoomSubjects.JoinRoom)
                {
                    AddPlayer();
                }
                else if(subject == NetworkingTags.RoomSubjects.ExitRoom)
                {
                    RemovePlayer(currentPlayerCount - 1);
                }
            }
        }

        bool down1 = false;
        bool down2 = false;
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.P) && !down1)
            {
                down1 = true;
                AddPlayer();
            }
            if(Input.GetKeyDown(KeyCode.O) && !down2)
            {
                down2 = true;
                RemovePlayer(currentPlayerCount - 1);
            }
        }
    }
}