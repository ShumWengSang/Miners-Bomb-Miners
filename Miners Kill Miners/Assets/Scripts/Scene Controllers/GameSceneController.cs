using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DarkRift;
using System.Collections.Generic;
namespace Roland
{
    public enum Direction
    {
        Up = 0,
        Down,
        Left,
        Right,
        Stop
    }

    [System.Serializable]
    struct intHolder
    {
        public int data;
    }

    public class GameSceneController : MonoBehaviour
    {
        ChangeScenes changeScene = null;
        public bool Sandbox = false;
        public bool GameHasStarted = false;
        public float timeToWaitToStart = 5;

        int PlayerReady;

        WaitForSeconds waitForTimer;
        public Text TimerCountDownText;
        public GameObject PlayerPrefab;

        bool SpawnedAllPlayers = false;

        //temp
        TileMap theTileMap;
        GameObject theObj;

        public GameObject ReadyText;
        public GameObject Game;
        public GameObject Shop;

        Dictionary<int, int> theListOfSpawns;

        int Spawn_id;
        void Awake()
        {
            PlayerReady = 0;
            theTileMap = TileMapInterfacer.Instance.TileMap;
            changeScene = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global");
            waitForTimer = new WaitForSeconds(1);
            //Check whether its sandbox or multiplayer.
            //If it is single player, generate the default map.
            //else, generate data and send over and generate again after taking.

            if (Sandbox == true)
            {
                Debug.Log("Tile is at" + theTileMap.ConvertTileToWorld(new Vector2(1, 1)));
                theObj = GameObject.Instantiate(PlayerPrefab, theTileMap.ConvertTileToWorld(new Vector2(1, 1)), Quaternion.identity) as GameObject;
                Player genericplayer = theObj.GetComponent<Player>();
                genericplayer.thePlayerData.CreatePlayerData("Generic Player", 0);
                //CurrentPlayer.Instance.ThePlayer = genericplayer;
            }
        }

        void Start()
        {
            CurrentPlayer.Instance.AmountOfPlayers = 0;
            if (Sandbox == true)
            {
                theObj.transform.position = theTileMap.ConvertTileToWorld(new Vector2(1, 1));
            }
            else
            {
                if(!DarkRiftAPI.isConnected)
                {
                    CustomNetworkManager.Instance.Connect("127.0.0.1");
                }
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.JoinMessage, "hi");
               // DarkRiftAPI.SendMessageToOthers(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.SpawnPlayer, "");
                DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ChangeStateToGame, "");
                DarkRiftAPI.onDataDetailed += ReceiveData;
            }
        }

        public void ChangeScene(string newScene)
        {
            changeScene.LoadScene(newScene);
        }

        

        public void StartTimer()
        {
            ReadyText.SetActive(true);
            StartCoroutine(StartCountdown());
        }

        IEnumerator WaitForOtherPlayers()
        {
            //send we are ready.
            DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.ReadyToStartGame, "");
            DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.GetRandomSpawn, "");
            if(DarkRiftAPI.isConnected)
            {
                Debug.Log("Amount of ready is " + PlayerReady + " Amount of players is" + CurrentPlayer.Instance.AmountOfPlayers);
                while (PlayerReady < CurrentPlayer.Instance.AmountOfPlayers || !SpawnedAllPlayers)
                {
                    yield return null;
                }
                Debug.Log("I'm through");
            }
        }

        IEnumerator StartCountdown()
        {
            yield return StartCoroutine(WaitForOtherPlayers());
            for (int i = 0; i < timeToWaitToStart; i++)
            {
                TimerCountDownText.text = (timeToWaitToStart - i).ToString();
                yield return waitForTimer;
            }
            GameHasStarted = true;
            TimerCountDownText.gameObject.SetActive(false);
            Shop.SetActive(false);
            Game.SetActive(true);
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
                //Also internally increase the amount of players.
                if (subject == NetworkingTags.ControllerSubjects.JoinMessage)
                {
                    CurrentPlayer.Instance.AmountOfPlayers++;              
                    if(senderID != DarkRiftAPI.id)
                        DarkRiftAPI.SendMessageToID(senderID, NetworkingTags.Controller, NetworkingTags.ControllerSubjects.ReplyToJoin, "");

                }

                else if (subject == NetworkingTags.ControllerSubjects.SpawnPlayer)
                {
                    //THIS IS ONLY FROM THE SERVER. CHANGE IS BECAUSE OF NEED TO CHANGE FROM 
                    //FAILED THOUGHT IDEA. WE SHOULD ONLY GET THIS ONCE
                    foreach (KeyValuePair<int, int> entry in theListOfSpawns)
                    {
                        int spawnPoint = entry.Value;
                        Vector2 SpawnTile = new Vector2(0, 0);
                        switch (spawnPoint)
                        {
                            case 1:
                                SpawnTile = new Vector2(1, 1);
                                break;
                            case 2:
                                SpawnTile = new Vector2(theTileMap.size_x - 2, theTileMap.size_z - 2);
                                break;
                            case 3:
                                SpawnTile = new Vector2(theTileMap.size_x - 2, 1);
                                break;
                            case 4:
                                SpawnTile = new Vector2(1, theTileMap.size_z - 2);
                                break;
                            default:
                                Debug.LogError("Not associated spawn_id. " + Spawn_id);
                                break;
                        }
                        Debug.Log("I'm spawning player at tile " + entry.Value);
                        //Instantiate the player
                        GameObject clone = (GameObject)Instantiate(PlayerPrefab, theTileMap.ConvertTileToWorld(SpawnTile), Quaternion.identity);
                        Player thePlayer = clone.GetComponent<Player>();
                        Debug.Log("Data is " + data);
                        thePlayer.player_id = (ushort)entry.Key;
                        CurrentPlayer.Instance.AddActivePlayer(entry.Key, thePlayer);
                    }
                    SpawnedAllPlayers = true;
                }
                else if (subject == NetworkingTags.ControllerSubjects.ReadyToStartGame)
                {
                    PlayerReady++;
                }
                else if(subject == NetworkingTags.ControllerSubjects.ReplyToJoin)
                {
                    CurrentPlayer.Instance.AmountOfPlayers++;

                }
            }
            else if(tag == NetworkingTags.Server)
            {
                if(subject == NetworkingTags.ServerSubjects.GetRandomSpawn)
                {
                    theListOfSpawns = (Dictionary<int, int>)data;
                }
            }
        }
    }

}
