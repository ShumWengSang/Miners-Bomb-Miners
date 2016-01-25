using System;
using System.Collections.Generic;
using DarkRift;
using System.Net;
using System.Net.Sockets;

namespace MinersBombMinersServerPlugin
{
    using Roland;
    [System.Serializable]
    public class PacketUseTypeID
    {
        public int thePlayerType;
        public ushort client_id;
    }
    public class MinersBombMinersServerPlugin : Plugin
    {
        Dictionary<int, int> theSpawnPoints = new Dictionary<int, int>();

        Dictionary<int, PlayerInfo> theClients = new Dictionary<int, PlayerInfo>();
        List<int> ListOfLosers = new List<int>();
        List<PacketUseTypeID> theListOfPlayers = new List<PacketUseTypeID>();
        PlayerAvailability PlayerColorsAvailableClass = new PlayerAvailability();
        List<ushort> IDsToIgnore = new List<ushort>();
        GameState CurrentGameState;
        enum GameState
        {
            Room = 0,
            Game
        }



        public class PlayerInfo
        {
            public int SpawnPoint;
            public PlayerType thePlayerType;
            public bool Lost = false;
            public bool ReadyToPlay = false;
            public int Money = 500;
        }

        public class PlayerAvailability
        {
            public bool[] PlayerColors = new bool[4];

            public PlayerAvailability()
            {
                for(int i = 0; i < PlayerColors.Length; i++)
                {
                    PlayerColors[i] = false;
                }
            }

            public void MakeColorAvailable(int color)
            {
                PlayerColors[color] = false;
            }

            public int GetNextAvailableColor()
            {
                for (int i = 0; i < PlayerColors.Length; i++)
                {
                   if(!PlayerColors[i])
                   {
                       PlayerColors[i] = true;
                       return i + 1;
                   }
                }
                return -1;
            }
        }

        
        [System.Serializable]
        public enum PlayerType
        {
            None = 0,
            Red,
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
        int PlayerNum;
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
            get 
            {
                return new Command[]
                {
                new Command ("Show_Connections", "Show current connections",FindConnections ),
                new Command ("Clear_Connections", "Clears all current connections", ClearConnections)
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
            ConnectionService.onPlayerDisconnect += OnPlayerDisconnect;
            ConnectionService.onPlayerConnect += OnPlayerFirstConnect;
            Interface.Log("------------------------------------------");
            Interface.Log("IPADDRESS FOR THIS SERVER IS: " + getIP());
            Interface.Log("Please connect to the IP Address");
            Interface.Log("------------------------------------------");
        }

        ~MinersBombMinersServerPlugin()
        {
            ConnectionService.onData -= OnDataReceived;
            ConnectionService.onPlayerDisconnect -= OnPlayerDisconnect;
        }

        public void ClearConnections(string [] parts)
        {
            ConnectionService[] allCons = DarkRiftServer.GetAllConnections();
            Interface.Log("Amount of connected connections are " + allCons.Length);
            for (int i = 0; i < allCons.Length; i++)
            {
                allCons[i].SendNetworkMessage(new NetworkMessage(0, DistributionType.Reply, 0, 255, 0, 0));
                allCons[i].Close();
            }
        }

        public void FindConnections(string [] parts)
        {
            ConnectionService[] allCons = DarkRiftServer.GetAllConnections();
            Interface.Log("Amount of connected connections are " + allCons.Length);
            for(int i = 0; i < allCons.Length; i++)
            {
                Interface.Log("The connection id is " + allCons[i].id);
            }
        }

        public void OnPlayerFirstConnect(ConnectionService con)
        {
            if(CurrentGameState != GameState.Room)
            {
                Interface.Log("Closing conenction");
                //con.SendReply(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.DisconnectYou, "");
                //IDsToIgnore.Add(con.id);
                con.Close();
            }
        }

        public void OnDataReceived(ConnectionService con, ref NetworkMessage msg)
        {
            if(IDsToIgnore.Contains(con.id))
            {
                Interface.Log("Ignoring id " + con.id);
                return;
            }
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

                        foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                        {
                            PacketUseTypeID PlayerData = new PacketUseTypeID();
                            PlayerData.client_id = (ushort)theKey.Key;
                            PlayerData.thePlayerType = (int)theKey.Value.thePlayerType;
                            theListOfPlayers.Add(PlayerData); ;
                        }

                        foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                        {
                            DarkRiftServer.GetConnectionServiceByID((ushort)theKey.Key).SendReply(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.StartGame, theListOfPlayers);
                            theKey.Value.ReadyToPlay = false;
                        }

                        theListOfPlayers.Clear();
                        CurrentGameState = GameState.Game;
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
                    con.SendReply(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.YouLose, "");
                    theClients[msg.senderID].Lost = true;

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
                    Interface.Log("ID of winner is " + IDofWinner);
                    Interface.Log("Amount of lost is " + AmountOfLost);
                    if(AmountOfLost == (theClients.Count - 1))
                    {
                        DarkRiftServer.GetConnectionServiceByID((ushort)IDofWinner).SendReply(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.YouWin, "");
                        //Reset lost counter.
                        foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                        {
                            theKey.Value.Lost = false;
                        }
                    }
                    else if(AmountOfLost == theClients.Count)
                    {
                        ConnectionService[] allID = DarkRiftServer.GetAllConnections();
                        for (int i = 0; i < allID.Length; i++)
                        {
                            allID[i].SendReply(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.Draw, "");
                        }
                    }
                }
                else if (msg.subject == NetworkingTags.ServerSubjects.PlayerRestarting)
                {
                    theClients[msg.senderID].ReadyToPlay = false;
                    msg.DecodeData();
                    int money = (int)msg.data;
                    theClients[msg.senderID].Money = (int)msg.data;
                    //Interface.Log("setting money to " + (int)msg.data);
                    bool allRestarted = true;
                    foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                    {
                        if (theKey.Value.ReadyToPlay)
                        {
                            allRestarted = false;
                            break;
                        }
                    }
                    if (allRestarted)
                    {
                        CurrentGameState = GameState.Room;
                    }
                }
                else if(msg.subject == NetworkingTags.ServerSubjects.GetMoneyForPlayer)
                {
                    Interface.Log("sending money : " + theClients[con.id].Money);
                    con.SendReply(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.GetMoneyForPlayer, theClients[con.id].Money);
                }
                else if(msg.subject == NetworkingTags.ServerSubjects.SetMoneyForPlayer)
                {
                    Interface.Log("setting money to " + (int)msg.data);
                    theClients[con.id].Money = (int)msg.data;
                }
            }
            else if(msg.tag == NetworkingTags.Controller)
            {
                if(msg.subject == NetworkingTags.ControllerSubjects.JoinMessage)
                {
                    if (CurrentGameState == GameState.Room)
                    {
                        if (!theClients.ContainsKey(msg.senderID))
                        {
                            PlayerNum++;
                            int color = PlayerColorsAvailableClass.GetNextAvailableColor();

                            PlayerInfo newPlayer = new PlayerInfo();
                            newPlayer.Lost = false;
                            newPlayer.ReadyToPlay = false;
                            newPlayer.SpawnPoint = PlayerNum;
                            newPlayer.thePlayerType = (PlayerType)color;

                            //we haven't add a condition to make sure its less than 4.
                            theClients.Add(msg.senderID, newPlayer);

                            msg.data = color;
                        }
                        else
                        {
                            msg.data = theClients[msg.senderID].thePlayerType;
                        }
                    }
                    else
                    {
                        //Interface.Log("Disconnecting player: game room is " + CurrentGameState);
                        //con.Close();
                    }
                }
            }
        }

        public void OnPlayerDisconnect(ConnectionService con)
        {
            if (con != null)
            {
                if (IDsToIgnore.Contains(con.id))
                {
                    Interface.Log("Removing player from ignore list " + con.id);
                    IDsToIgnore.Remove(con.id);
                }
                if (theClients.ContainsKey(con.id))
                {
                    PlayerColorsAvailableClass.MakeColorAvailable((int)theClients[con.id].thePlayerType - 1);
                    theClients.Remove(con.id);

                    PlayerNum--;
                    Interface.Log("Player number has decreased to " + PlayerNum);
                }
                if (theClients.Count <= 0)
                {
                    CurrentGameState = GameState.Room;
                }
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

        static string getIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
    }
}
