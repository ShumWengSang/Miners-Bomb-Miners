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
        List<int> ListOfLosers = new List<int>();
        PlayerAvailability PlayerColorsAvailableClass = new PlayerAvailability();
        Dictionary<int, PlayerInfo> theClients = new Dictionary<int, PlayerInfo>();
        
        enum GameState
        {
            Room = 0,
            Game
        }

        GameState CurrentGameState;


        public class PlayerInfo
        {
            public int SpawnPoint;
            public PlayerType thePlayerType;
            public bool Lost = false;
            public bool ReadyToPlay = false;
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
                       return i;
                   }
                }
                return -1;
            }
        }

        public List<PacketUseTypeID> theListOfPlayers = new List<PacketUseTypeID>();
        
        [System.Serializable]
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
            ConnectionService.onPlayerDisconnect += OnPlayerDisconnect;
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
                        Interface.Log("All are ready to play.");
                        //ConnectionService [] AllServices = DarkRiftServer.GetAllConnections();

                        foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                        {
                            PacketUseTypeID PlayerData = new PacketUseTypeID();
                            PlayerData.client_id = (ushort)theKey.Key;
                            PlayerData.thePlayerType = (int)theKey.Value.thePlayerType;
                            theListOfPlayers.Add(PlayerData); ;
                        }

                        foreach (KeyValuePair<int, PlayerInfo> theKey in theClients)
                        {
                            Interface.Log("The message 2 is " + DarkRiftServer.GetConnectionServiceByID((ushort)theKey.Key).SendReply(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.StartGame, theListOfPlayers));
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
                    if(AmountOfLost == (PlayerNum - 1))
                    {
                        NetworkMessage newMessage = new NetworkMessage(msg.destinationID, (byte)DistributionType.ID, (ushort)IDofWinner, NetworkingTags.Controller, NetworkingTags.ControllerSubjects.YouWin,"");
                        DarkRiftServer.GetConnectionServiceByID((ushort)IDofWinner).SendNetworkMessage(newMessage);
                    }
                }
                else if(msg.subject == NetworkingTags.ServerSubjects.SendMeSomething)
                {

                }
                else if(msg.subject == NetworkingTags.ServerSubjects.RestartGame)
                {

                }
            }
            else if (msg.tag == NetworkingTags.Events)
            {
                
            }
            else if(msg.tag == NetworkingTags.Controller)
            {
                if(msg.subject == NetworkingTags.ControllerSubjects.JoinMessage)
                {
                    if (CurrentGameState == GameState.Room)
                    {
                        PlayerNum++;
                        int color = PlayerColorsAvailableClass.GetNextAvailableColor();
                        Interface.Log("Player number has increased to " + PlayerNum);
                        Interface.Log("assining playertype to " + (PlayerType)color);

                        PlayerInfo newPlayer = new PlayerInfo();
                        newPlayer.Lost = false;
                        newPlayer.ReadyToPlay = false;
                        newPlayer.SpawnPoint = PlayerNum;
                        newPlayer.thePlayerType = (PlayerType)color;

                        //we haven't add a condition to make sure its less than 4.
                        theClients.Add(msg.senderID, newPlayer);
                    }
                    else
                    {
                        con.Close();
                    }
                }
            }
        }

        public void OnPlayerDisconnect(ConnectionService con)
        {
            PlayerNum--;
            Interface.Log("Player number has decreased to " + PlayerNum);
            
            if(theClients.ContainsKey(con.id))
            {
                Interface.Log("Removing player from client list");
                PlayerColorsAvailableClass.MakeColorAvailable((int)theClients[con.id].thePlayerType);
                theClients.Remove(con.id);
            }
            if(theClients.Count <= 0)
            {
                Interface.Log("Clients amount is " + theClients.Count);
                CurrentGameState = GameState.Room;
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
