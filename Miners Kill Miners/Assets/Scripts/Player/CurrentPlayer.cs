using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using DarkRift;
using System.Linq.Expressions;
using System.Linq;
namespace Roland
{
    public class CurrentPlayer : Singleton<CurrentPlayer>
    {
        PlayerDataContainer playerDataColleciton = null;
        bool loaded = false;
        //public Dictionary<int, Player> theActivePlayers = new Dictionary<int, Player>();
        Player thePlayer;
        public int OurID;
        public int AmountOfPlayers = 0;

        //temp holder for our player. Since the player class only creates after ready is clicked
        //we need a place to store the weapons the user has.
        //this will be changed as we move into a full game and do not need to rely on scenes
        //so for now its like this, but eventually all the required data will be gotten
        //from the room scene. then we don't need to use this.
        public Dictionary<Items_e, int> AmountOfItems = new Dictionary<Items_e, int>();

        protected CurrentPlayer()
        {
            if (!loaded)
            {
               // playerDataColleciton = PlayerDataContainer.Load(Path.Combine(Application.dataPath, "PlayerData.xml"));
                loaded = true;
            }
            InitializeList();
        }
        ~CurrentPlayer()
        {
            //playerDataColleciton.Save(Path.Combine(Application.dataPath, "PlayerData.xml"));
        }


        public Player ThePlayer
        {
            get { return thePlayer; }
            set { thePlayer = value; }
        }
        
        public void AddItem(Items_e theItem)
        {
            if (!AmountOfItems.ContainsKey(theItem))
            {
                AmountOfItems.Add(theItem, 1);
            }
            else
            {
                AmountOfItems[theItem]++;
            }
        }

        public PlayerDataContainer GetPlayerDataCollection
        {
            get { return playerDataColleciton; }
        }

        public void AddPlayer(PlayerData AddThisPlayer)
        {
            playerDataColleciton.playerDatas.Add(AddThisPlayer);
        }

        public PlayerData GetPlayerData(string name)
        {
            for( int i = 0; i < playerDataColleciton.playerDatas.Count; i++)
            {
                if(playerDataColleciton.playerDatas[i].Name == name)
                {
                    return playerDataColleciton.playerDatas[i];
                }
            }
            return null;
        }

        public PlayerData[] GetAllPlayerDatas()
        {
            return playerDataColleciton.playerDatas.ToArray();
        }

        public string[] GetPlayerDataNames()
        {
            string[] nameArray = new string[playerDataColleciton.playerDatas.Count];
            for(int i = 0; i < playerDataColleciton.playerDatas.Count; i++)
            {
                nameArray[i] = playerDataColleciton.playerDatas[i].Name;
            }
            return nameArray;
        }

        void OnApplicationQuit()
        {
            //playerDataColleciton.Save(Path.Combine(Application.dataPath, "PlayerData.xml"));
        }

        void Awake()
        {
            if (!loaded)
            {
                playerDataColleciton = PlayerDataContainer.Load(Path.Combine(Application.dataPath, "PlayerData.xml"));
                loaded = true;
            }
        }

        public void InitializeList()
        {
            List<Items_e> AllItems = Items_e.GetValues(typeof(Items_e)).Cast<Items_e>().ToList();
            for (int i = 0; i < AllItems.Count; ++i)
            {
                AmountOfItems.Add(AllItems[i], 0);
            }
        }
    }
}