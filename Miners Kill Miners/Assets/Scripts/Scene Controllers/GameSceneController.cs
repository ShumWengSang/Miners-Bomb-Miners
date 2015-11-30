using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DarkRift;
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

        Transform OurPlayerTransform;
        

        void Awake()
        {
            theTileMap = TileMapInterfacer.Instance.TileMap;
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
                //CurrentPlayer.Instance.ThePlayer = genericplayer;
            }
        }

        void Start()
        {
            
            if (Sandbox == true)
            {
                theObj.transform.position = theTileMap.ConvertTileToWorld(new Vector2(1, 1));
            }
            else
            {
                if(!DarkRiftAPI.isConnected)
                {
                    CustomNetworkManager.Instance.Connect("127.0.0.1");
                }
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Controller, NetworkingTags.ControllerSubjects.JoinMessage, "hi");
                DarkRiftAPI.SendMessageToAll(TagIndex.Controller, TagIndex.ControllerSubjects.SpawnPlayer, "");
                DarkRiftAPI.onDataDetailed += ReceiveData;
            }
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

        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            //When any data is received it will be passed here, 
            //we then need to process it if it's got a tag of 0 and, if 
            //so, create an object. This is where you'd handle most adminy 
            //stuff like that.

            //Ok, if data has a Controller tag then it's for us

            Debug.Log("I receive data.");
            if (tag == NetworkingTags.Controller)
            {
                //If a player has joined tell them to give us a player
                if (subject == NetworkingTags.ControllerSubjects.JoinMessage)
                {
                    //Basically reply to them.
                    int a = 1;
                    DarkRiftAPI.SendMessageToID(senderID, NetworkingTags.Controller, NetworkingTags.ControllerSubjects.SpawnPlayer, a);
                }

                if (subject == NetworkingTags.ControllerSubjects.SpawnPlayer)
                {
                    int Spawn_id = (int)data;

                    Vector2 SpawnTile = new Vector2(0, 0);
                    switch (Spawn_id)
                    {
                        case 1:
                            SpawnTile = new Vector2(1, 1);
                            break;
                        case 2:
                            SpawnTile = new Vector2(theTileMap.size_x - 2, theTileMap.size_z - 2);
                            break;
                        case 3:
                            SpawnTile = new Vector2(theTileMap.size_x - 2, 1);
                            break;
                        case 4:
                            SpawnTile = new Vector2(1, theTileMap.size_z - 2);
                            break;
                        default:
                            Debug.LogError("Not associated spawn_id. " + Spawn_id);
                            break;
                    }
                    //Instantiate the player
                    GameObject clone = (GameObject)Instantiate(PlayerPrefab, theTileMap.ConvertTileToWorld(SpawnTile), Quaternion.identity);

                    CurrentPlayer.Instance.AddActivePlayer(senderID, clone.GetComponent<Player>());

                    //If it's our player being created allow control and set the reference
                    if (senderID == DarkRiftAPI.id)
                    {
                        clone.GetComponent<Player>().isControllable = true;
                        OurPlayerTransform = clone.transform;
                    }
                }
            }
        }
    }

}
