using UnityEngine;
using System.Collections;
using System.IO;

namespace Roland
{
    public class SelectModeScene : MonoBehaviour
    {
        ChangeScenes changeScene = null;
        PlayerDataContainer playerDataCollection = null;
        
        // Use this for initialization
        void Start()
        {
            changeScene = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global"); 
        }
        
        public void ChangeScene(string newScene)
        {
            changeScene.LoadScene(newScene);
        }
        public void TurnOnMultiplayer()
        {
            //Load the XML
            playerDataCollection = CurrentPlayer.Instance.GetPlayerDataCollection;
            if(playerDataCollection.playerDatas.Count > 0)
            {
                //Display the new create scene.

            }
            else
            {
                //Display the scene that allows the player to select which player he is, or create a new player.
            }
        }

        public void Connect(string ipAddress)
        {
            CustomNetworkManager.Instance.Connect(ipAddress);
        }
    }
}