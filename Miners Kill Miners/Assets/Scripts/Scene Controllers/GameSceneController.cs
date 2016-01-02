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
        public GameObject PlayerDummy;
        public Text WinLose;

        bool SpawnedAllPlayers = false;

        //temp
        TileMap theTileMap;
        GameObject theObj;

        public GameObject ReadyText;
        public GameObject Game;
        public GameObject Shop;
        public GameObject RestartButton;

        Dictionary<int, int> theListOfSpawns;

        public RuntimeAnimatorController[] PlayerAnimators;
        public Sprite[] StartingSprite;

        [System.Serializable]
        public enum PlayerType
        {
            Red = 0,
            Green,
            Blue,
            Yellow
        }


        public void RestartLevel()
        {
            //first we send our $$$ + score to the server.
            //Send a message to restart the server status/ reset spawns and players joined.

            //reset the tile map.
            theTileMap.TileMapReset();
            Start();

        }

        void Awake()
        {
            DarkRiftAPI.workInBackground = false;   
            changeScene = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global");

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
            waitForTimer = new WaitForSeconds(1);
            PlayerReady = 0;
            theTileMap = TileMapInterfacer.Instance.TileMap;
            if (Sandbox == true)
            {
                theObj.transform.position = theTileMap.ConvertTileToWorld(new Vector2(1, 1));
            }
            else if(!Sandbox && !DarkRiftAPI.isConnected)
            {
                if(!DarkRiftAPI.isConnected)
                {
                    string IPAddress = System.IO.File.ReadAllText("ipaddress.txt");
                    CustomNetworkManager.Instance.Connect(IPAddress);
                }
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.JoinMessage, "hi");
                DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ChangeStateToGame, "");
            }
            DarkRiftAPI.onDataDetailed += ReceiveData;
        }

        public void ChangeScene(string newScene)
        {
            changeScene.LoadScene(newScene);
        }

        void OnDestroy()
        {
            DarkRiftAPI.onDataDetailed -= ReceiveData;
        }

        bool ClientReady = false;
        public void StartTimer()
        {
            ReadyText.SetActive(true);
            ClientReady = !ClientReady;
            if(ClientReady)
                DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ClientReadyToPlay,"");
            else
                DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ClientNotReady, "");
        }

        void updatePlayerItems()
        {
            CurrentPlayer.Instance.ThePlayer.UpdateItems();
        }

        IEnumerator StartCountdown()
        {
            Game.SetActive(true);
            Shop.SetActive(false);
            for (int i = 0; i < timeToWaitToStart; i++)
            {
                TimerCountDownText.text = (timeToWaitToStart - i).ToString();
                yield return waitForTimer;
            }
            GameHasStarted = true;
            TimerCountDownText.gameObject.SetActive(false);

        }

        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            //When any data is received it will be passed here, 
            //we then need to process it if it's got a tag of 0 and, if 
            //so, create an object. This is where you'd handle most adminy 
            //stuff like that.

            //Ok, if data has a Controller tag then it's for us
            Debug.Log("Receiving data");
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
                    ////THIS IS ONLY FROM THE SERVER. CHANGE IS BECAUSE OF NEED TO CHANGE FROM 
                    ////FAILED THOUGHT IDEA. WE SHOULD ONLY GET THIS ONCE
                    //foreach (KeyValuePair<int, int> entry in theListOfSpawns)
                    //{
                    //    int spawnPoint = entry.Value;
                    //    Vector2 SpawnTile = new Vector2(0, 0);
                    //    switch (spawnPoint)
                    //    {
                    //        case 1:
                    //            SpawnTile = new Vector2(1, 1);
                    //            break;
                    //        case 2:
                    //            SpawnTile = new Vector2(theTileMap.size_x - 2, theTileMap.size_z - 2);
                    //            break;
                    //        case 3:
                    //            SpawnTile = new Vector2(theTileMap.size_x - 2, 1);
                    //            break;
                    //        case 4:
                    //            SpawnTile = new Vector2(1, theTileMap.size_z - 2);
                    //            break;
                    //        default:
                    //            Debug.LogError("Not associated spawn_id. " + spawnPoint);
                    //            break;
                    //    }                        //Instantiate the player
                    //    GameObject clone;
                    //    if (entry.Key == DarkRiftAPI.id)
                    //    {
                    //        clone = (GameObject)Instantiate(PlayerPrefab, theTileMap.ConvertTileToWorld(SpawnTile), Quaternion.identity);
                    //        Player thePlayer = clone.GetComponent<Player>();
                    //        thePlayer.player_id = (ushort)entry.Key;
                    //        thePlayer.theController = this;
                    //        CurrentPlayer.Instance.ThePlayer = thePlayer;
                    //    }
                    //    else
                    //    {
                    //        clone = (GameObject)Instantiate(PlayerDummy, theTileMap.ConvertTileToWorld(SpawnTile), Quaternion.identity);
                    //        DummyPlayer thePlayer = clone.GetComponent<DummyPlayer>();
                    //        thePlayer.id = (ushort)entry.Key;
                    //    }
                    //}
                    //CurrentPlayer.Instance.ThePlayer.AmountOfItems = CurrentPlayer.Instance.AmountOfItems;
                    //SpawnedAllPlayers = true;
                }
                else if (subject == NetworkingTags.ControllerSubjects.ReadyToStartGame)
                {
                    PlayerReady++;
                }
                else if(subject == NetworkingTags.ControllerSubjects.ReplyToJoin)
                {
                    CurrentPlayer.Instance.AmountOfPlayers++;
                }
                else if (subject == NetworkingTags.ControllerSubjects.YouWin)
                {
                    CheckWinLose(true);
                }
                else if(subject == NetworkingTags.ControllerSubjects.GameOver)
                {
                    RestartButton.SetActive(true);
                }
                else if(subject == NetworkingTags.ControllerSubjects.StartGame)
                {
                    Debug.Log("Starting game");

                    List<MinersBombMinersServerPlugin.PacketUseTypeID> PacketPlayerData = (List<MinersBombMinersServerPlugin.PacketUseTypeID>)data;
                    for (int i = 0; i < PacketPlayerData.Count; i++)
                    {
                        Vector2 SpawnPoint = Vector2.zero;
                        RuntimeAnimatorController theColoredPlayer = PlayerAnimators[0];
                        Color thePlayerColor = Color.white;
                        switch (PacketPlayerData[i].thePlayerType)
                        {
                            case 0:
                                theColoredPlayer = PlayerAnimators[0];
                                SpawnPoint = new Vector2(1, 1);
                                thePlayerColor = Color.red;
                                break;
                            case 1:
                                SpawnPoint = new Vector2(theTileMap.size_x - 2, theTileMap.size_z - 2);
                                theColoredPlayer = PlayerAnimators[1];
                                thePlayerColor = Color.green;
                                break;
                            case 2:
                                SpawnPoint = new Vector2(theTileMap.size_x - 2, 1);
                                theColoredPlayer = PlayerAnimators[2];
                                thePlayerColor = Color.blue;
                                break;
                            case 3:
                                SpawnPoint = new Vector2(1, theTileMap.size_z - 2);
                                theColoredPlayer = PlayerAnimators[3];
                                thePlayerColor = Color.yellow;
                                break;
                            default:
                                Debug.LogWarning("No such player type found! Logged " + PacketPlayerData[i].thePlayerType);
                                break;
                        }
                        GameObject clone;
                        if (PacketPlayerData[i].client_id == DarkRiftAPI.id)
                        {

                            clone = (GameObject)Instantiate(PlayerPrefab, theTileMap.ConvertTileToWorld(SpawnPoint), Quaternion.identity);
                            Player thePlayer = clone.GetComponent<Player>();
                            thePlayer.player_id = PacketPlayerData[i].client_id;
                            thePlayer.theController = this;
                            CurrentPlayer.Instance.ThePlayer = thePlayer;
                            UiHolder theHolder = GetComponent<UiHolder>();
                            HealthBar healthBar = clone.GetComponent<HealthBar>();
                            healthBar.healthSlider = theHolder.HealthSlider;
                            healthBar.damageImage = theHolder.DamageHealth;
                            theHolder.HealthFill.color = thePlayerColor;
                        }
                        else
                        {
                            clone = (GameObject)Instantiate(PlayerDummy, theTileMap.ConvertTileToWorld(SpawnPoint), Quaternion.identity);
                            DummyPlayer thePlayer = clone.GetComponent<DummyPlayer>();
                            thePlayer.id = PacketPlayerData[i].client_id;
                        }
                        clone.GetComponent<Animator>().runtimeAnimatorController = theColoredPlayer;
                    }
                    StartCoroutine(StartCountdown());

                }
                else if(subject == NetworkingTags.ControllerSubjects.SendTest)
                {
                    Debug.Log("Test completed successfully");
                }
            }
        }

        public void CheckWinLose(bool win)
        {
            if(win)
            {
                WinLose.text = "YOU WIN";
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.GameOver, "");
            }
            else
            {
                DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ILose, "");
                WinLose.text = "YOU LOSE";
            }
           
            WinLose.gameObject.SetActive(true);
        }
    }
}
