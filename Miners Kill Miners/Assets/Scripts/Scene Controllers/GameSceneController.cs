using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roland
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Stop
    }
    public class GameSceneController : MonoBehaviour
    {
        ChangeScenes changeScene = null;
        public bool Sandbox = false;
        public bool GameHasStarted = false;
        public float timeToWaitToStart = 5;

        WaitForSeconds waitForTimer;
        public Text TimerCountDownText;
        public GameObject PlayerPrefab;

        Vector3 currentTileCoord;

        //temp
        TileMap theTileMap;
        GameObject theObj;

        void Awake()
        {
            theTileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
            changeScene = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global");
            waitForTimer = new WaitForSeconds(1);
            //Check whether its sandbox or multiplayer.
            //If it is single player, generate the default map.
            //else, generate data and send over and generate again after taking.

            if (Sandbox == true)
            {
                Debug.Log("Tile is at" + theTileMap.ConvertTileToWorld(new Vector2(1, 1)));
                theObj = GameObject.Instantiate(PlayerPrefab, theTileMap.ConvertTileToWorld(new Vector2(1, 1)), Quaternion.identity) as GameObject;
                Player genericplayer = theObj.GetComponent<Player>();
                genericplayer.thePlayerData.CreatePlayerData("Generic Player", 0);
                CurrentPlayer.Instance.ThePlayer = genericplayer;
            }

        }

        void Start()
        {
            theObj.transform.position = theTileMap.ConvertTileToWorld(new Vector2(1, 1));
        }

        public void ChangeScene(string newScene)
        {
            changeScene.LoadScene(newScene);
        }


        public void StartTimer()
        {
            StartCoroutine(StartCountdown());
        }

        IEnumerator StartCountdown()
        {
            for (int i = 0; i < timeToWaitToStart; i++)
            {
                TimerCountDownText.text = (timeToWaitToStart - i).ToString();
                yield return waitForTimer;
            }
            GameHasStarted = true;
            TimerCountDownText.gameObject.SetActive(false);
        }

        
    }

}
