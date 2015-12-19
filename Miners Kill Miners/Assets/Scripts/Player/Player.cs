
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using DarkRift;
using UnityEngine.UI;

namespace Roland
{
    public enum Items_e
    {
        SmallBomb = 0,
        BigBomb,
        TNTBomb,
        NuclearBomb
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

        public Items_e TheCurrentItem;

        Vector2 MoveDirection = new Vector2(0, 0);
        Animator theAnimator;
        Transform ourTransform;

        Vector2 LastDirection = new Vector2(0, 0);

        WaitForSeconds WaitHalfSecond;
        bool DigCooldown = false;

        public float invulTime = 1;
        bool invul = false;
        WaitForSeconds invulCD;

        Rigidbody2D rb;
        Vector3 Offset;
        SpriteRenderer sp;

        public Text HP;
        public Text NumberOfBombs;
        public Image BombType;
        public Sprite[] theSprites;

        IEnumerator InvulCoolDown()
        {
            invul = true;
            yield return invulCD;
            invul = false;
        }
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            theAnimator = GetComponent<Animator>();
            InitializePlayer();
            thePlayerData = new PlayerData();

            EventManager.OnKeyboardButtonDown += OnButtonPressed;
            EventManager.OnMouseButtonDown += OnMouseButtonDown;
            DarkRiftAPI.onDataDetailed += ReceiveData;
            ourTransform = this.transform;
            WaitForUpdate = new WaitForSeconds(0.001f);
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
            sp = GetComponent<SpriteRenderer>();
            TheCurrentItem = Items_e.SmallBomb;

            UiHolder theHolder = GameObject.Find("GameSceneController").GetComponent<UiHolder>();

            HP = theHolder.HP;
            NumberOfBombs = theHolder.AmountOfBombs;
            BombType = theHolder.TypeOfBomb;
            UpdateUI(TheCurrentItem, AmountOfItems[TheCurrentItem]);
            UpdateHealth(HealthPoints);
            if(this.player_id == DarkRiftAPI.id)
                StartCoroutine(UpdatePosition());
        }

        WaitForSeconds WaitForUpdate;
        IEnumerator UpdatePosition()
        {
            while (true)
            {
                DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.UpdatePostion, ourTransform.position);
                yield return WaitForUpdate;
            }
        }

        public void DestroyPlayer()
        {
            StopAllCoroutines();
            InitializePlayer();
        }

        public void UpdateItems()
        {
            DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.GiveItemDic, AmountOfItems);
        }

        public void UpdateUI(Items_e theBomb, int amount)
        {
            int rep = (int)theBomb;
            string itemsprite = "item spritesheet_" + rep.ToString();
            BombType.sprite = theSprites[rep];
            NumberOfBombs.text = amount.ToString();

        }
        public void UpdateHealth(int health)
        {
            HP.text = health.ToString();
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
                UpdateHealth(HealthPoints);
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

        Vector2 nextTilePos;
        Vector2 prevTilePos;
        Vector2 currentTilePos;
        Vector2 tilePosBlocker;

        void Update()
        {
           // ourTransform.position = theTileMap.ConvertWorldToTile(ourTransform.position);
            nextTilePos = MoveDirection + theTileMap.ConvertWorldToTile(ourTransform.position - Offset);
            
           // Debug.Log("Our tile pos is " + theTileMap.ConvertWorldToTile(transform.position));
            Block theNextBlock = theTileMap.theMap.GetTileAt(nextTilePos);
            if (prevTilePos != currentTilePos)
            {
                prevTilePos = nextTilePos;
               // theTileMap.theMap.SetTileAt(tilePosBlocker, new Noblock());

               // if(this.player_id == DarkRiftAPI.id)
                   // DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.UpdatePostion, nowTilePos);
            }
            if (theNextBlock is Noblock)
            {
                //Debug.Log("Changing transform and move direction is" + MoveDirection);
                ourTransform.localPosition += new Vector3(MoveDirection.x, MoveDirection.y, 0) * Time.deltaTime * speed;
                
               // tilePosBlocker = theTileMap.theMap.SetTileAt(theTileMap.ConvertWorldToTile(ourTransform.position - Offset), new InvisibleWallBlock());
                //rb.velocity = MoveDirection * Time.deltaTime * speed;
            }
            else if(!DigCooldown)
            {
                //Dig. We start coroutine to do the cooldown as well.
                StartCoroutine(DigCooldownUpdate());
                theTileMap.DigTile(nextTilePos, DigPower);
                DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.DestroyMapTile, nextTilePos);
                //rb.velocity = Vector3.zero * Time.deltaTime * speed;
            }


        }
        public void OnMouseButtonDown(MouseButtons button, int id, Items_e theItem)
        {
            if(id == player_id)
            {
                if (button == MouseButtons.left)
                {
                    if (AmountOfItems[theItem] > 0)
                    {
                        AmountOfItems[theItem]--;
                        MouseButtonSpawn(theItem);
                    }
                }
                else if(button == MouseButtons.ScrollDown)
                {
                    bool Changed = false;
                    List<Items_e> theList = AmountOfItems.Keys.ToList<Items_e>();
                    int index = -1;
                    for (int i = 0; i < theList.Count; i++)
                    {
                        if(theList[i] == TheCurrentItem)
                        {
                            index = i;
                            break;
                        }
                    }

                    for (int i = index - 1; i >= 0; i--)
                    {
                        if (AmountOfItems[theList[i]] != 0)
                        {
                            TheCurrentItem = theList[i];
                            Changed = true;
                            break;
                        }
                    }
                    if(!Changed )
                    {
                        for (int i = theList.Count - 1; i >= 0; i--)
                        {
                            if (AmountOfItems[theList[i]] != 0)
                            {
                                TheCurrentItem = theList[i];
                                Changed = true;
                                break;
                            }
                        }
                    }
                }
                else if (button == MouseButtons.ScrollUp)
                {
                    List<Items_e> theList = AmountOfItems.Keys.ToList<Items_e>();
                    int index = -1;
                    bool Changed = false;
                    for (int i = 0; i < theList.Count; i++)
                    {
                        if (theList[i] == TheCurrentItem)
                        {
                            index = i;
                            break;
                        }
                    }

                    for (int i = index + 1; i < theList.Count; i++)
                    {
                        if (AmountOfItems[theList[i]] != 0)
                        {
                            TheCurrentItem = theList[i];
                            Changed = true;
                            break;
                        }
                    }
                    if (!Changed)
                    {
                        for (int i = 0; i < theList.Count; i++)
                        {
                            if (AmountOfItems[theList[i]] != 0)
                            {
                                TheCurrentItem = theList[i];
                                Changed = true;
                                break;
                            }
                        }
                    }
                }             
            }

            UpdateUI(TheCurrentItem, AmountOfItems[TheCurrentItem]);
        }

        void MouseButtonSpawn(Items_e theItem)
        {
            GameObject Object; 
            switch (theItem)
            {
                case Items_e.SmallBomb:
                    Object = ObjectSpawner.SpawnObject("SmallBomb", transform.position);
                    Object.GetComponent<SmallBomb>().ParentPlayer = this;
                    break;
                case Items_e.BigBomb:
                    Object = ObjectSpawner.SpawnObject("BigBomb", transform.position);
                    Object.GetComponent<BigBomb>().ParentPlayer = this;
                    break;
                case Items_e.TNTBomb:
                    Object = ObjectSpawner.SpawnObject("TNTBomb", transform.position);
                    Object.GetComponent<TNTBomb>().ParentPlayer = this;
                    break;
                case Items_e.NuclearBomb:

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
                        if (sp.sprite != null)
                        {
                            Offset = sp.sprite.bounds.extents;
                        }
                        MoveDirection = new Vector2(0,1);
                        theAnimator.SetTrigger("Move Up");
                        break;
                    case Direction.Down:
                        if (sp.sprite != null)
                        {
                            Offset = -sp.sprite.bounds.extents;
                            Offset -= Offset * 0.01f;

                        }
                        MoveDirection = new Vector2(0, -1);
                        theAnimator.SetTrigger("Move Down");
                        break;
                    case Direction.Left:
                        if (sp.sprite != null)
                        {
                            Offset = -sp.sprite.bounds.extents;
                            Offset -= Offset * 0.01f;
                        }
                        MoveDirection = new Vector2(-1, 0);
                        theAnimator.SetTrigger("MoveLeft");
                        break;
                    case Direction.Right:
                        if (sp.sprite != null)
                        {
                            Offset = sp.sprite.bounds.extents;
                        }
                        MoveDirection = new Vector2(1, 0);
                        theAnimator.SetTrigger("MoveRight");
                        break;
                    case Direction.Stop:
                        MoveDirection = new Vector2(0, 0);
                        theAnimator.SetTrigger("Stop");
                        break;
                }
                DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.ChangeDir, theDirection);
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
                        AmountOfItems = (Dictionary<Items_e, int>)data;
                    }
                    if(subject == NetworkingTags.PlayerSubjects.UpdatePostion)
                    {
                        Vector2 newTilePos = (Vector2)data;
                        if(newTilePos != nextTilePos)
                        {
                            Debug.Log("Receive data");
                            transform.position = theTileMap.ConvertTileToWorld(newTilePos);
                        }
                    }
                    if(subject == NetworkingTags.PlayerSubjects.DestroyMapTile)
                    {
                        //Vector2 Vdata = (Vector2)data;
                        //theTileMap.DigTile(Vdata, 999);
                    }
                }
            }
        }
    }
}