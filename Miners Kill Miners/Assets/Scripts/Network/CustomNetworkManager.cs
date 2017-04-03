using UnityEngine;
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
            catch(ConnectionFailedException)
            {
                return false;
            }
            return true;
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