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
        enum GameState
        {
            Room = 0,
            Game
        }

        GameState CurrentGameState;

        struct intHolder
        {
            public int data;
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
                if (msg.subject == NetworkingTags.ServerSubjects.GetRandomSpawn)
                {

                    NetworkMessage newMessage = new NetworkMessage();
                    newMessage.data = (object)theSpawnPoints;
                    newMessage.senderID = msg.senderID;
                    newMessage.subject = NetworkingTags.ServerSubjects.GetRandomSpawn;
                    newMessage.tag = NetworkingTags.Server;
                    newMessage.distributionType = DistributionType.ID;
                    con.SendNetworkMessage(newMessage);

                    newMessage = new NetworkMessage();
                    newMessage.data = (object)msg.senderID;
                    newMessage.senderID = msg.senderID;
                    newMessage.subject = NetworkingTags.ControllerSubjects.SpawnPlayer;
                    newMessage.tag = NetworkingTags.Controller;
                    newMessage.distributionType = DistributionType.ID;
                    con.SendNetworkMessage(newMessage);
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
                    ListOfLosers.Add(msg.senderID);
                    if(ListOfLosers.Count == (theSpawnPoints.Count - 1))
                    {
                        //so we only got one player who hasn't lost, meaning he won.
                        foreach (int id in theSpawnPoints.Keys)
                        {
                            if(!ListOfLosers.Contains(id))
                            {
                                //so we don't have this id in losers, meaning he is the winner.
                                //send him a winner message.
                                Interface.Log("ID: " + id + " has won");
                                NetworkMessage newMessage = new NetworkMessage();
                                newMessage = new NetworkMessage();
                                newMessage.data = "";
                                newMessage.senderID = (ushort)id;
                                newMessage.subject = NetworkingTags.ControllerSubjects.YouWin;
                                newMessage.tag = NetworkingTags.Controller;
                                newMessage.distributionType = DistributionType.ID;
                                DarkRiftServer.GetConnectionServiceByID((ushort)id).SendNetworkMessage(newMessage);

                            }
                        }
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
                    theSpawnPoints.Add(msg.senderID, PlayerNum);
                }
            }


        }

        public void OnPlayerFirstConnect(ConnectionService con)
        {
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
            theSpawnPoints.Remove(con.id);
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
