using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roland
{
    using MinersBombMinersServerPlugin;
    public class PlayerStats
    {
        public PlayerStats()
        {
            kills = 0;
            deaths = 0;
            suicides = 0;
        }

        public int Kills
        {
            set { kills = value; }
            get { return kills; }
        }

        public int Deaths
        {
            set { deaths = value; }
            get { return deaths; }
        }

        public int Suicides
        {
            set { suicides = value; }
            get { return suicides; }
        }

        public void AddKills()
        {
            Kills++;
        }

        public void AddDeaths()
        {
            Deaths++;
        }

        public void AddSuicides()
        {
            Suicides++;
        }

        int kills;
        int deaths;
        int suicides;

    }
    public class KillTrackSystem : Singleton<KillTrackSystem>
    {
        public DisplayKDUI theUI;
        protected KillTrackSystem() { }

        protected Dictionary<ushort, PlayerStats> DictionaryOfKills = new Dictionary<ushort, PlayerStats>();

        public PlayerStats GetPlayerStats(ushort id)
        {
            if (DictionaryOfKills.ContainsKey(id))
                return DictionaryOfKills[id];
            else
                return null;
        }

        public void RemovePlayer(ushort id)
        {
            if(DictionaryOfKills.ContainsKey(id))
            {
                DictionaryOfKills.Remove(id);
            }
        }

        public void AddPlayer(ushort id, PlayerStats theStats)
        {
            if(!DictionaryOfKills.ContainsKey(id))
            {
                DictionaryOfKills.Add(id, theStats);
            }
        }

        public void AddKill(ushort playerid, ushort explosionid)
        {
            if(DictionaryOfKills.ContainsKey(playerid))
            {
                if(explosionid == playerid)
                {
                    //Suicide
                    DictionaryOfKills[playerid].AddSuicides();
                }
                else if(explosionid == 999)
                {
                    DictionaryOfKills[playerid].AddDeaths();
                }
                else
                {
                    DictionaryOfKills[playerid].AddDeaths();
                    DictionaryOfKills[explosionid].AddKills();
                }
                theUI.UpdateUI(playerid);
                theUI.UpdateUI(explosionid);
            }
        }
    }
}
