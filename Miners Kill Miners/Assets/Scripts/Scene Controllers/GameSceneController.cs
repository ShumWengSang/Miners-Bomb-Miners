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

    public class GameSceneController : MonoBehaviour
    {
        ChangeScenes changeScene = null;
        public bool Sandbox = false;
        public bool GameHasStarted = false;
        public float timeToWaitToStart = 5;
        public int BaseIncomeGold = 200;

        int PlayerReady;

        WaitForSeconds waitForTimer;
        public Text TimerCountDownText;
        public Text WinLose;
        //temp
        TileMap theTileMap;

        GameObject theObj;
        public GameObject PlayerPrefab;
        public GameObject PlayerDummy;
        public GameObject ReadyText;
        public GameObject Game;
        public GameObject Shop;
        public GameObject RestartButton;
        public GameObject NotConnected;

        public RuntimeAnimatorController[] PlayerAnimators;
        public Sprite[] StartingSprite;

        MinersBombMinersServerPlugin.MinersBombMinersServerPlugin.PlayerType ourColor;

        public void RestartLevel()
        {
            //first we send our $$$ + score to the server.
            //Send a message to restart the server status/ reset spawns and players joined.

            //reset the tile map.
            theTileMap.TileMapReset();
            Start();
            CurrentPlayer.Instance.AmountOfPlayers = 0;
        }
        public void SendRestart()
        {
            CurrentPlayer.Instance.Money += BaseIncomeGold;
            //DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.SetMoneyForPlayer, CurrentPlayer.Instance.Money);
            Debug.Log("Sending money: " + CurrentPlayer.Instance.Money);
            int money = CurrentPlayer.Instance.Money;
            DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.PlayerRestarting, money);

            ChangeScene("MultiplayerGame");
        }

        void Awake()
        { 
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
            Map.Offset += 2.0f;
            waitForTimer = new WaitForSeconds(1);
            PlayerReady = 0;
            theTileMap = TileMapInterfacer.Instance.TileMap;
            if (Sandbox == true)
            {
                theObj.transform.position = theTileMap.ConvertTileToWorld(new Vector2(1, 1));
            }
            else if(!Sandbox)
            {
                if(!DarkRiftAPI.isConnected)
                {
                    string IPAddress = System.IO.File.ReadAllText("ipaddress.txt");
                    CustomNetworkManager.Instance.Connect(IPAddress);
                }
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.JoinMessage, "hi");
                //DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ChangeStateToGame, "");
            }
            DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.GetMoneyForPlayer, DarkRiftAPI.id);
            DarkRiftAPI.onDataDetailed += ReceiveData;
            DarkRiftAPI.onPlayerDisconnected += OnPlayerDisconnect;
        }



        void Update()
        {
            if(!DarkRiftAPI.isConnected)
            {
                Shop.SetActive(false);
                NotConnected.SetActive(true);
            }
        }

        public void ChangeScene(string newScene)
        {
            changeScene.LoadScene(newScene);
        }

        void OnDestroy()
        {
            DarkRiftAPI.onDataDetailed -= ReceiveData;
            DarkRiftAPI.onPlayerDisconnected -= OnPlayerDisconnect;
        }

        bool ClientReady = false;
        public void StartTimer()
        {
            ClientReady = !ClientReady;
            ReadyText.SetActive(ClientReady);
            if (ClientReady)
            {
                DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ClientReadyToPlay, "");
            }
            else
            {
                DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ClientNotReady, "");
            }
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
            if (tag == NetworkingTags.Controller)
            {
                //If a player has joined tell them to give us a player
                //Also internally increase the amount of players.
                if (subject == NetworkingTags.ControllerSubjects.JoinMessage)
                {
                    CurrentPlayer.Instance.AmountOfPlayers++;
                    int color = (int)data;
                    MinersBombMinersServerPlugin.MinersBombMinersServerPlugin.PlayerType player = (MinersBombMinersServerPlugin.MinersBombMinersServerPlugin.PlayerType)color;
                    if(ourColor == MinersBombMinersServerPlugin.MinersBombMinersServerPlugin.PlayerType.None)
                    {
                        ourColor = player;
                    }
                    if (senderID != DarkRiftAPI.id)
                    {
                        DarkRiftAPI.SendMessageToID(senderID, NetworkingTags.Controller, NetworkingTags.ControllerSubjects.ReplyToJoin, ourColor);
                    }
                    color -= 1;
                    ConnectDisconnect.instance.AddPlayer(color, senderID);
                }

                else if (subject == NetworkingTags.ControllerSubjects.SpawnPlayer)
                {
                }
                else if (subject == NetworkingTags.ControllerSubjects.ReadyToStartGame)
                {
                    PlayerReady++;
                }
                else if(subject == NetworkingTags.ControllerSubjects.ReplyToJoin)
                {
                    CurrentPlayer.Instance.AmountOfPlayers++;
                    MinersBombMinersServerPlugin.MinersBombMinersServerPlugin.PlayerType player = (MinersBombMinersServerPlugin.MinersBombMinersServerPlugin.PlayerType)data;
                    int color = (int)player;
                    color -= 1;
                    ConnectDisconnect.instance.AddPlayer(color, senderID);
                }
                else if (subject == NetworkingTags.ControllerSubjects.YouWin)
                {
                    CheckWinLose(true);
                }
                else if(subject == NetworkingTags.ControllerSubjects.GameOver)
                {
                    RestartButton.SetActive(true);
                }
                else if(subject == NetworkingTags.ControllerSubjects.GetMoneyForPlayer)
                {
                    CurrentPlayer.Instance.Money = (int)data;
                }
                else if(subject == NetworkingTags.ControllerSubjects.StartGame)
                {
                    List<MinersBombMinersServerPlugin.PacketUseTypeID> PacketPlayerData = (List<MinersBombMinersServerPlugin.PacketUseTypeID>)data;
                    for (int i = 0; i < PacketPlayerData.Count; i++)
                    {
                        Vector2 SpawnPoint = Vector2.zero;
                        RuntimeAnimatorController theColoredPlayer = PlayerAnimators[0];
                        Color thePlayerColor = Color.white;
                        switch (PacketPlayerData[i].thePlayerType)
                        {
                            case 1:
                                theColoredPlayer = PlayerAnimators[0];
                                SpawnPoint = new Vector2(1, 1);
                                thePlayerColor = Color.red;
                                break;
                            case 2:
                                SpawnPoint = new Vector2(theTileMap.size_x - 2, theTileMap.size_z - 2);
                                theColoredPlayer = PlayerAnimators[1];
                                thePlayerColor = Color.green;
                                break;
                            case 3:
                                SpawnPoint = new Vector2(theTileMap.size_x - 2, 1);
                                theColoredPlayer = PlayerAnimators[2];
                                thePlayerColor = Color.blue;
                                break;
                            case 4:
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
                            thePlayer.theEquipments = CurrentPlayer.Instance.AmountOfEquipments;
                            thePlayer.HealthPoints += CurrentPlayer.Instance.AddedHealth;
                            CurrentPlayer.Instance.AddedHealth = 0;
                            thePlayer.DigPower += CurrentPlayer.Instance.AddedDig;
                            CurrentPlayer.Instance.AddedDig = 0;
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
                else if(subject == NetworkingTags.ControllerSubjects.DisconnectYou)
                {
                    DarkRiftAPI.Disconnect();
                }
            }
            else if(tag == NetworkingTags.Server)
            {
                if (subject == NetworkingTags.ServerSubjects.PlayerRestarting)
                {
                    Debug.Log("Here data is " + data);
                    
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
            GameHasStarted = false;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        void OnPlayerDisconnect(ushort id)
        {
            Debug.Log("Disconnecting id " + id);
            ConnectDisconnect.instance.RemovePlayer(id);
        }
    }
}
