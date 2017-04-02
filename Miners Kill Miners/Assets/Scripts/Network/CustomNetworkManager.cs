﻿using UnityEngine;
using System.Collections;
using DarkRift;

namespace Roland
{
    public class CustomNetworkManager : Singleton<CustomNetworkManager>
    {
        public bool Connect(string serverIP)
        {
            //Connect to the DarkRift Server using the Ip specified (will hang until connected or timeout)
            try {
                DarkRiftAPI.Connect(serverIP);
            }
            catch(ConnectionFailedException e)
            {
                return false;
            }
            return true;
            
            //Tell others that we've entered the game and to instantiate a player object for us.
            if (DarkRiftAPI.isConnected)
            {
                //Get everyone else to tell us to spawn them a player (this doesn't need the data field so just put whatever)
               // DarkRiftAPI.SendMessageToOthers(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.JoinMessage, "hi");
                //Then tell them to spawn us a player! (this time the data is the spawn position)
                //DarkRiftAPI.SendMessageToAll(TagIndex.Controller, TagIndex.ControllerSubjects.SpawnPlayer,i);
            }
            else
                Debug.Log("Failed to connect to DarkRift Server!");
        }

        void OnApplicationQuit()
        {
            if (DarkRiftAPI.isConnected)
            {
                Debug.Log("Disconnectin");
                DarkRiftAPI.Disconnect();
            }
        }

        new void OnDestroy()
        {
            if (DarkRiftAPI.isConnected)
            {
                Debug.Log("Disconnectin");
                DarkRiftAPI.Disconnect();
            }
        }
    }
}