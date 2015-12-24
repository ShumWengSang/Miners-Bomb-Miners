using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
namespace MinersBombMinersServerPlugin
{
    using Roland;
    public class MinersBombMinersServerPlugin  : Plugin
    {
        Dictionary<int, int> theSpawnPoints = new Dictionary<int, int>();
        List<int> ListOfLosers = new List<int>();

        Dictionary<int, PlayerInfo> theClients = new Dictionary<int, PlayerInfo>();
        enum GameState
        {
            Room = 0,
            Game
        }

        GameState CurrentGameState;

        PlayerType LatestPlayer = (PlayerType)0;

        public class PlayerInfo
        {
            public int SpawnPoint;
            public PlayerType thePlayerType;
            public bool Lost = false;
            public bool ReadyToPlay = false;
        }

        public class PacketUseTypeID
        {
            public PlayerType thePlayerType;
            public ushort client_id;
        }
        public class PacketPlayerData
        {
            public PacketUseTypeID[] theListOfPlayers;
        }

        public enum PlayerType
        {
            Red = 0,
            Green,
            Blue,
            Yellow
        }
        public class Vector2
        {
            int x = 0;
            int y = 0;
            public Vector2()
            {
                x = 0;
                y = 0;
            }
            public Vector2(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

        }

        bool log;
        static int PlayerNum;
        int TotalNumberOfPlayers = 0;
        public override string name
        {
            get { return "Miners Bomb Miners Plugin"; }
        }
        public override string version
        {
            get { return "1.1"; }
        }
        public override Command[] commands
        {
            get {
                return new Command[]
            {
                new Command ("SetLog", "Turns Data on or off", set_logCommand)
            };
            }
        }
        public override string author
        {
            get { return "RolandShum"; }
        }
        public override string supportEmail
        {
            get { return "shumwengsang@gmail.com"; }
        }

        public MinersBombMinersServerPlugin()
        {
            CurrentGameState = GameState.Room;
            PlayerNum = 0;
            ConnectionService.onData += OnDataReceived;
            ConnectionService.onPlayerConnect += OnPlayerFirstConnect;
            ConnectionService.onPlayerDisconnect += OnPlayerDisconnect;
        }

        ~MinersBombMinersServerPlugin()
        {
            ConnectionService.onData -= OnDataReceived;
            ConnectionService.onPlayerConnect -= OnPlayerFirstConnect;
            ConnectionService.onPlayerDisconnect -= OnPlayerDisconnect;
        }

        public void OnDataReceived(ConnectionService con, ref NetworkMessage msg)
        {
            if (msg.tag == NetworkingTags.Server)
            {
                if (msg.subject == NetworkingTags.ServerSubjects.ClientReadyToPlay)
                {
                    theClients[msg.senderID].ReadyToPlay = true;
                    bool allReadyToPlay = true;
                    foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                    {
                        if(!theKey.Value.ReadyToPlay)
                        {
                            allReadyToPlay = false;
                            break;
                        }
                    }
                    if(allReadyToPlay)
                    {
                        ConnectionService [] AllServices = DarkRiftServer.GetAllConnections();

                        PacketPlayerData TotalPlayerData = new PacketPlayerData();
                        TotalPlayerData.theListOfPlayers = new PacketUseTypeID[theClients.Count];
                        int count = 0;
                        foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                        {
                            PacketUseTypeID PlayerData = new PacketUseTypeID();
                            PlayerData.client_id = (ushort)theKey.Key;
                            PlayerData.thePlayerType = theKey.Value.thePlayerType;
                            TotalPlayerData.theListOfPlayers[count] = PlayerData;
                            count++;
                        }

                        for(int i = 0; i < AllServices.Length; i++)
                        {
                            if(theClients.ContainsKey(AllServices[i].id))
                            {
                                NetworkMessage newMessage = new NetworkMessage();
                                newMessage.subject = NetworkingTags.ControllerSubjects.StartGame;
                                newMessage.tag = NetworkingTags.Controller;
                                newMessage.data = TotalPlayerData;
                                AllServices[i].SendNetworkMessage(newMessage);
                            }
                        }
                    }

                }
                else if(msg.subject == NetworkingTags.ServerSubjects.ClientNotReady)
                {
                    theClients[msg.senderID].ReadyToPlay = false;
                }
                else if (msg.subject == NetworkingTags.ServerSubjects.ChangeStateToRoom)
                {
                   // CurrentGameState = GameState.Room;
                }
                else if (msg.subject == NetworkingTags.ServerSubjects.ChangeStateToGame)
                {
                    //CurrentGameState = GameState.Game;
                }
                else if(msg.subject == NetworkingTags.ServerSubjects.ILose)
                {
                    theClients[msg.senderID].Lost = false;

                    int AmountOfLost = 0;
                    int IDofWinner = -1;
                    foreach(KeyValuePair<int, PlayerInfo> theKey in theClients)
                    {
                        if(theKey.Value.Lost)
                        {
                            AmountOfLost++;
                        }
                        else
                        {
                            IDofWinner = theKey.Key;
                        }
                    }
                    if(AmountOfLost == (PlayerNum - 1))
                    {
                        NetworkMessage newMessage = new NetworkMessage(msg.destinationID, (byte)DistributionType.ID, (ushort)IDofWinner, NetworkingTags.Controller, NetworkingTags.ControllerSubjects.YouWin,"");
                        DarkRiftServer.GetConnectionServiceByID((ushort)IDofWinner).SendNetworkMessage(newMessage);
                    }
                }
            }
            else if (msg.tag == NetworkingTags.Events)
            {
                
            }
            else if(msg.tag == NetworkingTags.Controller)
            {
                if(msg.subject == NetworkingTags.ControllerSubjects.JoinMessage)
                {  
                    PlayerNum++;
                    Interface.Log("Player number has increased to " + PlayerNum);

                    PlayerInfo newPlayer = new PlayerInfo();
                    newPlayer.Lost = false;
                    newPlayer.ReadyToPlay = false;
                    newPlayer.SpawnPoint = PlayerNum;
                    newPlayer.thePlayerType = LatestPlayer;

                    LatestPlayer = (PlayerType)((int)(LatestPlayer)++);
                    //we haven't add a condition to make sure its less than 4.
                    theClients.Add(msg.senderID, newPlayer);
                }
            }


        }

        public void OnPlayerFirstConnect(ConnectionService con)
        {
            TotalNumberOfPlayers++;
            Interface.Log("Total amount of players is " + TotalNumberOfPlayers);
            if(!(CurrentGameState == GameState.Room) || PlayerNum > 4)
            {
                //NetworkMessage newMessage = new NetworkMessage();
                //newMessage.data = (object)PlayerNum;
                //newMessage.subject = NetworkingTags.ServerSubjects.GetRandomSpawn;
                //newMessage.tag = NetworkingTags.Server;
                //newMessage.distributionType = DistributionType.Reply;
                //con.SendNetworkMessage(newMessage);
                con.Close();
            }
        }

        public void OnPlayerDisconnect(ConnectionService con)
        {
            PlayerNum--;
            Interface.Log("Player number has decreased to " + PlayerNum);
            
            if(theClients.ContainsKey(con.id))
            {
                theClients.Remove(con.id);
            }
        }

        public void set_logCommand(string [] parts)
        {
            if(parts.Length != 1)
            {
                Interface.LogError("Setlog should only have on arg. On or off");
            }
            if(parts[0] != "on" && parts[0] != "off")
            {
                Interface.LogError("Setlog should be on or off, nothnig else");
            }
            log = (parts[0] == "on") ? true : false; 
        }
    }
}
