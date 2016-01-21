using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Roland
{
    [System.Serializable]
    public class MyArray
    {
        public Graphic[] myarray;
    }
    public class DisplayKDUI : MonoBehaviour
    {
        public MyArray[] KD;
        protected DisplayKDUI() { }

        public void UpdateUIAll()
        {
            SetText(0, false, 999, 999, 999);
            SetText(1, false, 999, 999, 999);
            SetText(2, false, 999, 999, 999);
            SetText(3, false, 999, 999, 999);

            ushort[] ids = ConnectDisconnect.instance.GetAllPlayer();
            for(int i = 0; i < ids.Length; i++)
            {
                UpdateUI(ids[i]);
            }
            
        }

        public void UpdateUI(int id)
        {
            int color = ConnectDisconnect.instance.GetPlayerColor((ushort)id);
            Debug.Log("Getting color " + color + " for id " + id);
            PlayerStats stats = KillTrackSystem.Instance.GetPlayerStats((ushort)id);
            if(stats == null)
            {
                SetText(color, false, 999, 999, 999);
            }
            else
            {
                SetText(color, true, stats.Kills, stats.Deaths, stats.Suicides);
            }
            
        }

        void SetText(int id, bool setActive, int kills, int deaths, int suicides)
        {
            string s_kills = kills != 999 ? kills.ToString() : "";
            string s_deaths = deaths != 999 ? deaths.ToString() : "";
            string s_suicides = suicides != 999 ? suicides.ToString() : "";
            KD[id].myarray[0].gameObject.SetActive(setActive);
            Text killtext = (Text)KD[id].myarray[1];
            Text deathtext = (Text)KD[id].myarray[2];
            Text suicidetext = (Text)KD[id].myarray[3];
            killtext.text = s_kills;
            deathtext.text = s_deaths;
            suicidetext.text = s_suicides;
        }
    }
}
