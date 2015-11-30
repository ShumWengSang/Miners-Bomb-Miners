using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using DarkRift;
namespace Roland
{
    public class CurrentPlayer : Singleton<CurrentPlayer>
    {
        PlayerDataContainer playerDataColleciton = null;
        bool loaded = false;
        public Dictionary<int, Player> theActivePlayers = new Dictionary<int, Player>();
        public int OurID;
        public int AmountOfPlayers = 0;


        protected CurrentPlayer()
        {
            if (!loaded)
            {
                playerDataColleciton = PlayerDataContainer.Load(Path.Combine(Application.dataPath, "PlayerData.xml"));
                loaded = true;
            }
            OurID = DarkRiftAPI.id;
        }
        ~CurrentPlayer()
        {
            //playerDataColleciton.Save(Path.Combine(Application.dataPath, "PlayerData.xml"));
        }
        public void AddActivePlayer(int id, Player thePlayer)
        {
            theActivePlayers.Add(id, thePlayer);
        }

        public Player ThePlayer
        {
            get { return theActivePlayers[OurID]; }
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
            playerDataColleciton.Save(Path.Combine(Application.dataPath, "PlayerData.xml"));
        }

        void Awake()
        {
            if (!loaded)
            {
                playerDataColleciton = PlayerDataContainer.Load(Path.Combine(Application.dataPath, "PlayerData.xml"));
                loaded = true;
            }
        }
    }
}