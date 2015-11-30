﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Roland
{
    public class CurrentPlayer : Singleton<CurrentPlayer>
    {
        Player theCurrentPlayer = null;
        PlayerDataContainer playerDataColleciton = null;
        bool loaded = false;
        public Dictionary<int, Player> theActivePlayers = new Dictionary<int, Player>();

        protected CurrentPlayer()
        {
            if (!loaded)
            {
                playerDataColleciton = PlayerDataContainer.Load(Path.Combine(Application.dataPath, "PlayerData.xml"));
                loaded = true;
            }
        }
        ~CurrentPlayer()
        {
            //playerDataColleciton.Save(Path.Combine(Application.dataPath, "PlayerData.xml"));
        }

        public Player ThePlayer
        {
            get { return theCurrentPlayer; }
            set
            {
                theCurrentPlayer = value;
                if(value.thePlayerData.Name == "Generic Player")
                {
                    return;
                }
                if(!playerDataColleciton.playerDatas.Contains(value.thePlayerData))
                {
                    AddPlayer(value.thePlayerData);
                }
                theCurrentPlayer = value;
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