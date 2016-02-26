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
        
        public Transform HPIconsParent;
        

        public Text NumberOfDigs;

        public RuntimeAnimatorController[] PlayerAnimators;
        public Sprite[] StartingSprite;

        public Text MoneyShop;
        public Text MoneyGame;
        public Text Round;

        public DisplayKDUI KDObject;

        public GameObject PauseObj;

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
            int money = CurrentPlayer.Instance.Money;
            Debug.Log("Sennding " + money);
            DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.PlayerRestarting, money);

            CurrentPlayer.Instance.Restart();

            ChangeScene("MultiplayerGame");
        }

        void Awake()
        {
            Random.seed = 1;
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

            Shop.SetActive(true);
            Game.SetActive(false);
            ReadyText.SetActive(false);
            CurrentPlayer.Instance.GetHPIcons(HPIconsParent);
            CurrentPlayer.Instance.DigPowerUI = NumberOfDigs;
            CurrentPlayer.Instance.controller = this;
            MoneyShop = GameObject.Find("Money").GetComponent<Text>();
            KillTrackSystem.Instance.theUI = KDObject;

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
            DarkRiftAPI.onDataDetailed += ReceiveData;
            DarkRiftAPI.onPlayerDisconnected += OnPlayerDisconnect;
            EventManager.OnDisplayKD += DisplayKD;

            string showBomb = System.IO.File.ReadAllText("showBomb.txt");
            if(showBomb == "True" || showBomb == "true")
            {
                RevealFogExplosion.Trigger = true;
            }
            else
            {
                RevealFogExplosion.Trigger = false;
            }

            CurrentPlayer.Instance.Start();
        }



        void Update()
        {
            if(!DarkRiftAPI.isConnected)
            {
                Shop.SetActive(false);
                NotConnected.SetActive(true);
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Pause(true);
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
            EventManager.OnDisplayKD -= DisplayKD;
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
            HPIconsParent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            Game.SetActive(true);
            MoneyGame = GameObject.Find("GameMoney").GetComponent<Text>();
            Shop.SetActive(false);
            CurrentPlayer.Instance.DeleteHorizontalLayout(HPIconsParent);
            for (int i = 0; i < timeToWaitToStart; i++)
            {
                TimerCountDownText.text = (timeToWaitToStart - i).ToString();
                yield return waitForTimer;
            }
            GameHasStarted = true;
            KDObject.UpdateUIAll();
            CurrentPlayer.Instance.Money = CurrentPlayer.Instance.Money;
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
                    else
                    {
                        DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.GetMoneyForPlayer, DarkRiftAPI.id);
                    }
                    color -= 1;

                    KillTrackSystem.Instance.AddPlayer(senderID, new PlayerStats());
                    ConnectDisconnect.instance.AddPlayer(color, senderID);
                    CurrentPlayer.Instance.HPIconSprite = StartingSprite[ConnectDisconnect.instance.GetPlayerColor(DarkRiftAPI.id)];
                    CurrentPlayer.Instance.UpdateHealthPointInGame();
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
                    KillTrackSystem.Instance.AddPlayer(senderID, new PlayerStats());
                }
                else if (subject == NetworkingTags.ControllerSubjects.YouWin)
                {
                    CheckWinLose(WinLoseDraw.Win);
                }
                else if (subject == NetworkingTags.ControllerSubjects.Draw)
                {
                    CheckWinLose(WinLoseDraw.Draw);
                }
                else if (subject == NetworkingTags.ControllerSubjects.YouLose)
                {
                    CheckWinLose(WinLoseDraw.Lose);
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
                        switch (PacketPlayerData[i].thePlayerType)
                        {
                            case 1:
                                theColoredPlayer = PlayerAnimators[0];
                                SpawnPoint = new Vector2(1, 1);
                                break;
                            case 2:
                                SpawnPoint = new Vector2(theTileMap.size_x - 2, theTileMap.size_z - 2);
                                theColoredPlayer = PlayerAnimators[1];
                                break;
                            case 3:
                                SpawnPoint = new Vector2(theTileMap.size_x - 2, 1);
                                theColoredPlayer = PlayerAnimators[2];
                                break;
                            case 4:
                                SpawnPoint = new Vector2(1, theTileMap.size_z - 2);
                                theColoredPlayer = PlayerAnimators[3];
                                break;
                            default:
                                Debug.LogWarning("No such player type found! Logged " + PacketPlayerData[i].thePlayerType);
                                break;
                        }
                        GameObject clone;
                        if (PacketPlayerData[i].client_id == DarkRiftAPI.id)
                        {

                            clone = Lean.LeanPool.Spawn(PlayerPrefab, theTileMap.ConvertTileToWorld(SpawnPoint), Quaternion.identity);
                            Player thePlayer = clone.GetComponentInChildren<Player>();
                            thePlayer.player_id = PacketPlayerData[i].client_id;
                            thePlayer.theController = this;
                            thePlayer.theEquipments = CurrentPlayer.Instance.AmountOfEquipments;
                            CurrentPlayer.Instance.ThePlayer = thePlayer;
                            UiHolder theHolder = GetComponent<UiHolder>();
                            HealthBar healthBar = clone.GetComponentInChildren<HealthBar>();
                            healthBar.damageImage = theHolder.DamageHealth;
                        }
                        else
                        {
                            clone = Lean.LeanPool.Spawn(PlayerDummy, theTileMap.ConvertTileToWorld(SpawnPoint), Quaternion.identity);
                            DummyPlayer thePlayer = clone.GetComponentInChildren<DummyPlayer>();
                            thePlayer.id = PacketPlayerData[i].client_id;
                        }
                        clone.GetComponentInChildren<Animator>().runtimeAnimatorController = theColoredPlayer;
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


        public void CheckWinLose(WinLoseDraw cond)
        {
            Debug.Log("Changing win lose text");
            switch (cond)
            {
                case WinLoseDraw.Draw:
                    WinLose.text = "DRAW";
                    DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.GameOver, "");
                    break;
                case WinLoseDraw.Win:
                     WinLose.text = "YOU WIN";
                    DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.GameOver, "");
                    break;
                case WinLoseDraw.Lose:
                    //DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ILose, "");
                    WinLose.text = "YOU LOSE";
                    break;
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
            ConnectDisconnect.instance.RemovePlayer(id);
            KillTrackSystem.Instance.RemovePlayer(id);
        }



        bool KDdisplay = false;
        public void DisplayKD()
        {
            KDdisplay = !KDdisplay; 
            KDObject.gameObject.SetActive(KDdisplay);
        }

        bool GameHasStartedTemp;
        public void Pause(bool pause)
        {
            if(pause)
            {
                GameHasStartedTemp = GameHasStarted;
                GameHasStarted = false;
                PauseObj.SetActive(true);
            }
            else
            {
                GameHasStarted = GameHasStartedTemp;
                PauseObj.SetActive(false);
            }
        }
    }
}
