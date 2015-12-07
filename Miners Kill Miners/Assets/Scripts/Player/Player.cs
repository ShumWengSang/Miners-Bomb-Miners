
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using DarkRift;

namespace Roland
{
    public enum Items_e
    {
        SmallBomb = 0,
        BigBomb,
        verybigbomb
    }
    [System.Serializable]
    public class Player : MonoBehaviour
    {
        public bool isControllable;
        
        TileMap theTileMap;
        public PlayerData thePlayerData;
        public Dictionary<Items_e, int> AmountOfItems = new Dictionary<Items_e, int>();
        public int DigPower = 1;
        public int speed = 5;
        public int player_id;
        public float WaitTimeForDig = 0.5f;
        public int HealthPoints = 100;
        public int Money = 100;
        public int Score = 0;
        public GameSceneController theController;

        Vector2 MoveDirection = new Vector2(0, 0);
        Animator theAnimator;
        Transform ourTransform;

        Vector2 LastDirection = new Vector2(0, 0);

        WaitForSeconds WaitHalfSecond;
        bool DigCooldown = false;

        public float invulTime = 1;
        bool invul = false;
        WaitForSeconds invulCD;

        IEnumerator InvulCoolDown()
        {
            invul = true;
            yield return invulCD;
            invul = false;
        }
        void Awake()
        {
            theAnimator = GetComponent<Animator>();
            InitializePlayer();
            thePlayerData = new PlayerData();

            EventManager.OnKeyboardButtonDown += OnButtonPressed;
            EventManager.OnMouseButtonDown += OnMouseButtonDown;
            DarkRiftAPI.onDataDetailed += ReceiveData;
            ourTransform = this.transform;
            
        }

        void Start()
        {
            theTileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
            if(theTileMap == null)
            {
                Debug.LogWarning("Tile Map not found! Error!");
            }
            WaitHalfSecond = new WaitForSeconds(WaitTimeForDig);
            invulCD = new WaitForSeconds(invulTime);
        }

        public void UpdateItems()
        {
            DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.GiveItemDic, AmountOfItems);
        }



        void OnDestroy()
        {
            EventManager.OnKeyboardButtonDown -= OnButtonPressed;
            EventManager.OnMouseButtonDown -= OnMouseButtonDown;
            DarkRiftAPI.onDataDetailed -= ReceiveData;
        }

        public void InitializePlayer()
        {
            List<Items_e> AllItems = Items_e.GetValues(typeof(Items_e)).Cast<Items_e>().ToList();
            for (int i = 0; i < AllItems.Count; ++i)
            {
                AmountOfItems.Add(AllItems[i], 0);
            }
        }

        public void MinusHealthPoints(int damage)
        {
            if (!invul)
            {
                StartCoroutine(InvulCoolDown());
                HealthPoints -= damage;
                if (HealthPoints <= 0)
                {
                    //we lose.
                    gameObject.SetActive(false);
                    //tell controller to check who wins, if any.
                    if(player_id == DarkRiftAPI.id)
                        theController.CheckWinLose(false);
                    
                }
            }
        }

        IEnumerator DigCooldownUpdate()
        {
            DigCooldown = true;
            yield return WaitHalfSecond;
            DigCooldown = false;
        }

        void Update()
        {        
           // ourTransform.position = theTileMap.ConvertWorldToTile(ourTransform.position);
            Vector2 CheckNextPosition = MoveDirection + theTileMap.ConvertWorldToTile(ourTransform.position);
            
           // Debug.Log("Our tile pos is " + theTileMap.ConvertWorldToTile(transform.position));
            Block theNextBlock = theTileMap.theMap.GetTileAt(CheckNextPosition);

            if (theNextBlock is Noblock)
            {
                //Debug.Log("Changing transform and move direction is" + MoveDirection);
                ourTransform.localPosition += new Vector3(MoveDirection.x, MoveDirection.y, 0) * Time.deltaTime * speed;
            }
            else if(!DigCooldown)
            {
                //Dig. We start coroutine to do the cooldown as well.
                StartCoroutine(DigCooldownUpdate());
                theTileMap.DigTile(CheckNextPosition, DigPower);
            }
        }
        public void OnMouseButtonDown(int button, int id, Items_e theItem)
        {
            if(id == player_id)
            {
                if (AmountOfItems[theItem] > 0)
                {
                    AmountOfItems[theItem]--;
                    MouseButtonSpawn(theItem);
                }
            }
        }

        void MouseButtonSpawn(Items_e theItem)
        {
            switch (theItem)
            {
                case Items_e.SmallBomb:
                    GameObject bomb = ObjectSpawner.SpawnObject("SmallBomb", transform.position);
                    bomb.GetComponent<SmallBomb>().ParentPlayer = this;
                    break;
                default:
                    break;
            }
        }

        public void OnButtonPressed(Direction theDirection, int id)
        {
            //We make sure this is talking to us
            if(id == player_id)
            {
                switch (theDirection)
                {
                    case Direction.Up:
                        MoveDirection = new Vector2(0,1);
                        theAnimator.SetTrigger("Move Up");
                        break;
                    case Direction.Down:
                        MoveDirection = new Vector2(0, -1);
                        theAnimator.SetTrigger("Move Down");
                        break;
                    case Direction.Left:
                        MoveDirection = new Vector2(-1, 0);
                        theAnimator.SetTrigger("MoveLeft");
                        break;
                    case Direction.Right:
                        MoveDirection = new Vector2(1, 0);
                        theAnimator.SetTrigger("MoveRight");
                        break;
                    case Direction.Stop:
                        MoveDirection = new Vector2(0, 0);
                        theAnimator.SetTrigger("Stop");
                        break;
                }
                if (LastDirection != MoveDirection)
                {
                    LastDirection = MoveDirection;
                    ourTransform.localPosition = theTileMap.ConvertTileToWorld(theTileMap.ConvertWorldToTile(ourTransform.position));
                }
            }
        }

        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            if (senderID == player_id)
            {
                if (tag == NetworkingTags.Player)
                {
                    if (subject == NetworkingTags.PlayerSubjects.GiveItemDic)
                    {
                        Debug.Log("Updated items");
                        AmountOfItems = (Dictionary<Items_e, int>)data;
                    }
                }
            }
        }
    }
}