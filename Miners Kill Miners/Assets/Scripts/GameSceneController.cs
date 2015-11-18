using UnityEngine;
using System.Collections;

namespace Roland
{
    public class GameSceneController : MonoBehaviour
    {

        ChangeScenes changeScene = null;

        // Use this for initialization
        void Start()
        {
            changeScene = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global");

            //Check whether its sandbox or multiplayer.
            //If it is single player, generate the default map.
            //else, generate data and send over and generate again after taking.


        }

        public void ChangeScene(string newScene)
        {
            changeScene.LoadScene(newScene);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
