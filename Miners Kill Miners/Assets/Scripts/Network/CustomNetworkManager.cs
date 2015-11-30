using UnityEngine;
using System.Collections;
using DarkRift;

namespace Roland
{
    public class CustomNetworkManager : Singleton<CustomNetworkManager>
    {
        //The player that we will instantiate when someone joins.
        public GameObject playerObject;

        //A reference to our player
        Transform player;
        TileMap theInstance;
        // Use this for initialization
        void Start()
        {
            theInstance = TileMapInterfacer.Instance.theTileMap;
        }

        public void Connect(string serverIP)
        {
            //Connect to the DarkRift Server using the Ip specified (will hang until connected or timeout)
            DarkRiftAPI.Connect(serverIP);
            //Setup a receiver so we can create players when told to.
            DarkRiftAPI.onDataDetailed += ReceiveData; 
            
            //Tell others that we've entered the game and to instantiate a player object for us.
            if (DarkRiftAPI.isConnected)
            {
                //Get everyone else to tell us to spawn them a player (this doesn't need the data field so just put whatever)
                DarkRiftAPI.SendMessageToOthers(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.JoinMessage, "hi");
                //Then tell them to spawn us a player! (this time the data is the spawn position)
                //DarkRiftAPI.SendMessageToAll(TagIndex.Controller, TagIndex.ControllerSubjects.SpawnPlayer,i);
            }
            else
                Debug.Log("Failed to connect to DarkRift Server!");
        }

        void OnApplicationQuit()
        {
            DarkRiftAPI.onDataDetailed -= ReceiveData;
            DarkRiftAPI.Disconnect();
        }

        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            //When any data is received it will be passed here, 
            //we then need to process it if it's got a tag of 0 and, if 
            //so, create an object. This is where you'd handle most adminy 
            //stuff like that.

            //Ok, if data has a Controller tag then it's for us
            if (tag == NetworkingTags.Controller)
            {
                //If a player has joined tell them to give us a player
                if (subject == NetworkingTags.ControllerSubjects.JoinMessage)
                {
                    //Basically reply to them.
                    DarkRiftAPI.SendMessageToID(senderID, NetworkingTags.Controller, NetworkingTags.ControllerSubjects.SpawnPlayer, player.position);
                }

                if(subject == NetworkingTags.ControllerSubjects.SpawnPlayer)
                {
                    int Spawn_id = (int)data;

                    Vector2 SpawnTile = new Vector2(0,0);
                    switch (Spawn_id)
                    {
                        case 1:
                            SpawnTile = new Vector2(1,1);
                            break;
                        case 2:
                            SpawnTile = new Vector2(theInstance.size_x - 2, theInstance.size_z - 2);
                            break;
                        case 3:
                            SpawnTile = new Vector2(theInstance.size_x - 2,1);
                            break;
                        case 4:
                            SpawnTile = new Vector2(1,theInstance.size_z - 2);
                            break;
                        default:
                            Debug.LogError("Not associated spawn_id. " + Spawn_id);
                            break;
                    }
                    Debug.Log("Spawn id is " + Spawn_id);
                    //Instantiate the player
                    GameObject clone = (GameObject)Instantiate(playerObject, theInstance.ConvertTileToWorld(SpawnTile), Quaternion.identity);
                    //Tell the network player who owns it so it tunes into the right updates.
                    clone.GetComponent<NetworkPlayer>().networkID = senderID;

                    //If it's our player being created allow control and set the reference
                    if (senderID == DarkRiftAPI.id)
                    {
                        clone.GetComponent<Player>().isControllable = true;
                        player = clone.transform;
                    }
                }
            }
        }
    }
}