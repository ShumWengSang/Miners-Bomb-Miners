using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Roland
{
    public class ConnectDisconnect : MonoBehaviour
    {
        public Text[] Texts;
        public static ConnectDisconnect instance;

        Dictionary<ushort, int> Players = new Dictionary<ushort, int>();
        
        class PlayerColor
        {
            public int color;
            public ushort id;
            public PlayerColor()
            { }
            public PlayerColor(int color, ushort id)
            {
                this.color = color;
                this.id = id;
            }
        }

        void Awake()
        {
            instance = this;
        }

        public int GetPlayerColor(ushort id)
        {
            if(Players.ContainsKey(id))
                return Players[id];
            return -1;
        }

        public void SetPlayerColor(ushort id, int color)
        {
            if(!Players.ContainsKey(id))
            {
                Players.Add(id, color);
            }
            else
            {
                Players[id] = color;
            }
        }

        public void AddPlayer(int color, ushort id)
        {
            SetPlayerColor(id, color);
            if (!(id == DarkRift.DarkRiftAPI.id))
                UpdateConnectedTexts(color);
            else
                UpdateYouTexts(color);
        }

        void UpdateYouTexts(int color)
        {
            Texts[color].text = "YOU";
            Texts[color].color = Color.green;
        }
        public void RemovePlayer(ushort id)
        {
            UpdateDisconnectTexts(id);

            if (Players.ContainsKey(id))
                Players.Remove(id);
            else
                Debug.LogWarning("Cannot find id to remove");
        }

        public void UpdateConnectedTexts(int color)
        {
            Texts[color].text = "CONNECTED";
            Texts[color].color = Color.green;
        }

        public void UpdateDisconnectTexts(ushort id)
        {
            int color = Players[id];
            Texts[color].text = "NOT CONNECTED";
            Texts[color].color = Color.red;
        }

        public ushort[] GetAllPlayer()
        {
            return Players.Keys.ToArray();
        }
    }
}
