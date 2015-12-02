using UnityEngine;
using System.Collections;
using DarkRift;
using System.Collections.Generic;

namespace Roland
{
    public class RoomController : MonoBehaviour
    {
        public Transform thePlayerBoxesParent;
        GameObject[] thePlayerBoxes;
        int currentPlayerCount;
        // Use this for initialization
        public Dictionary<int, GameObject> theActivePlayers = new Dictionary<int,GameObject>();

        void OnApplicationQuit()
        {
            if (DarkRiftAPI.isConnected)
            {
                Debug.Log("Disconnectin from room");
                DarkRiftAPI.onDataDetailed -= ReceiveData;
                DarkRiftAPI.Disconnect();
            }
        }

        public void GiveAmountOfPlayers()
        {
            CurrentPlayer.Instance.AmountOfPlayers = theActivePlayers.Count;
        }

        void Start()
        {
            if(!DarkRiftAPI.isConnected)
            {
                CustomNetworkManager.Instance.Connect("127.0.0.1");
            }
            DarkRiftAPI.onDataDetailed += ReceiveData;
            currentPlayerCount = 0;
            thePlayerBoxes = new GameObject[thePlayerBoxesParent.childCount];
            for (int i = 0; i < thePlayerBoxesParent.childCount; i++)
            {
                thePlayerBoxes[i] = thePlayerBoxesParent.GetChild(i).gameObject;
                thePlayerBoxes[i].SetActive(false);
            }
            DarkRiftAPI.SendMessageToAll(NetworkingTags.Room, NetworkingTags.RoomSubjects.JoinRoom, "");
            DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ChangeStateToRoom, "");
        }
        
        public int AddPlayer(int id)
        {
            if (!theActivePlayers.ContainsKey(id))
            {

                thePlayerBoxes[currentPlayerCount].SetActive(true);
                theActivePlayers.Add(id, thePlayerBoxes[currentPlayerCount]);
                return currentPlayerCount++;
            }
            return -1;
        }

        public void RemovePlayer(int i)
        {
            theActivePlayers[i].SetActive(false);
            theActivePlayers.Remove(i);
        }

        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            if(tag == NetworkingTags.Room)
            {
                if(subject == NetworkingTags.RoomSubjects.JoinRoom)
                {
                    AddPlayer(senderID);
                    DarkRiftAPI.SendMessageToID(senderID, NetworkingTags.Room, NetworkingTags.RoomSubjects.ReplayToJoin, "");
                }
                else if(subject == NetworkingTags.RoomSubjects.ExitRoom)
                {
                    RemovePlayer(senderID);
                }
                else if(subject == NetworkingTags.RoomSubjects.ReplayToJoin)
                {
                    AddPlayer(senderID);
                }
            }
        }
    }
}