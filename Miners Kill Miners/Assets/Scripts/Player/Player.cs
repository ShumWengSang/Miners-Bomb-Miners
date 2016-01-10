
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
        NuclearBomb,
        NapalmBomb,
        CrossBomb,
        Mine,
        RemoteBomb,
        BigRemoteBomb
    }

    public class Player : PlayerBase
    {
        public bool isControllable;
        bool DigCooldown = false;
        bool invul = false;

        public PlayerData thePlayerData;


        public int DigPower = 1;
        public int speed = 5;
        public int player_id;
        public int HealthPoints = 3;
        public int CurrentHealthPoints;
        public int Money = 100;
        public int Score = 0;

        public GameSceneController theController;

        public EquipmentBase TheCurrentItem;

        WaitForSeconds WaitHalfSecond;

        public float WaitTimeForDig = 0.5f;
        public float invulTime = 1;

        WaitForSeconds invulCD;
        WaitForSeconds WaitForUpdate;


        Vector2 MoveDirection = new Vector2(0, 0);
        Vector2 LastDirection = new Vector2(0, 0);
        Vector2 nextTilePos;

        public Text HP;
        public Text NumberOfBombs;
        
        public Image BombType;

        public Sprite[] theSprites;

        public HealthBar ourPlayerHealthBar;

        public List<EquipmentBase> theEquipments;
        IEnumerator InvulCoolDown()
        {
            invul = true;
            yield return invulCD;
            invul = false;
        }
        void Awake()
        {
            theAnimator = GetComponent<Animator>();
            thePlayerData = new PlayerData();
            WaitForUpdate = new WaitForSeconds(0.001f);
        }

        void Start()
        {
            base.Init();
        

            CurrentHealthPoints = HealthPoints;
            WaitHalfSecond = new WaitForSeconds(WaitTimeForDig);
            invulCD = new WaitForSeconds(invulTime);

            for (int i = 0; i < theEquipments.Count; i++)
            {
                if (theEquipments[i] is SmallBombData)
                {
                    TheCurrentItem = theEquipments[i];
                }
            }

            UiHolder theHolder = GameObject.Find("GameSceneController").GetComponent<UiHolder>();
            ourPlayerHealthBar = GetComponent<HealthBar>();

            HP = theHolder.HP;
            NumberOfBombs = theHolder.AmountOfBombs;
            BombType = theHolder.TypeOfBomb;
            UpdateUI(TheCurrentItem);
            UpdateHealth(CurrentHealthPoints);
            if(this.player_id == DarkRiftAPI.id)
                StartCoroutine(UpdatePosition());

            tilePosBlocker = new Vector2(-1, -1);
        }

        IEnumerator UpdatePosition()
        {
            while (true)
            {
               // DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.ChangeBlockToNonMovable, currentTilePos);
                DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.UpdatePostion, ourTransform.position);
                yield return WaitForUpdate;
            }
        }

        public void DestroyPlayer()
        {
            StopAllCoroutines();
        }

        public void UpdateUI(EquipmentBase theBomb)
        {
            theBomb.UpdateInGameUI(NumberOfBombs , BombType);
        }
        public void UpdateHealth(int health)
        {
            ourPlayerHealthBar.UpdateHealthBar(health, HealthPoints);
            HP.text = health.ToString();
        }

        void OnDestroy()
        {
            base.deInit();
        }

        public void MinusHealthPoints(int damage)
        {
            if (!invul)
            {
                StartCoroutine(InvulCoolDown());
                CurrentHealthPoints -= damage;
                UpdateHealth(CurrentHealthPoints);
                if (CurrentHealthPoints <= 0)
                {
                    //we lose.
                    gameObject.SetActive(false);
                    //tell controller to check who wins, if any.
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.PlayerDied, "");
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
            currentTilePos = theTileMap.ConvertWorldToTile(ourTransform.position - Offset);
            nextTilePos = MoveDirection + theTileMap.ConvertWorldToTile(ourTransform.position - Offset);

           // Debug.Log("Our tile pos is " + theTileMap.ConvertWorldToTile(transform.position));
            Block theNextBlock = theTileMap.theMap.GetTileAt(nextTilePos);

            theTileMap.UpdateTexture(tilePosBlocker, new Noblock());
            theTileMap.UpdateTexture(currentTilePos, new InvisibleWallBlock());
            tilePosBlocker = currentTilePos;
            if (theNextBlock is Noblock)
            {
                //Debug.Log("Changing transform and move direction is" + MoveDirection);
                ourTransform.localPosition += new Vector3(MoveDirection.x, MoveDirection.y, 0) * Time.deltaTime * speed;
                //DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.ChangeBlockToNonMovable, currentTilePos);
               
                
                //rb.velocity = MoveDirection * Time.deltaTime * speed;
            }
            else if(!DigCooldown)
            {
                //Dig. We start coroutine to do the cooldown as well.
                StartCoroutine(DigCooldownUpdate());
                theTileMap.DigTile(nextTilePos, DigPower);
                //rb.velocity = Vector3.zero * Time.deltaTime * speed;
            }
        }
        protected override void OnMouseButtonDown(MouseButtons button, int id, int theItemID)
        {
            if (id == player_id)
            {
                if (button == MouseButtons.left)
                {
                    TheCurrentItem.PlayerSpawnBomb(ourTransform.position);
                    if (DarkRiftAPI.isConnected)
                    {
                        DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.leftMouseButton, TheCurrentItem.OrderID);
                    }
                }
                else if (button == MouseButtons.ScrollDown)
                {
                    bool Changed = false;
                    for (int i = TheCurrentItem.OrderID + 1; i < theEquipments.Count; i++)
                    {
                        if (theEquipments[i].Amount > 0)
                        {
                            TheCurrentItem = theEquipments[i];
                            Changed = true;
                            break;
                        }
                    }
                    if (!Changed)
                    {
                        TheCurrentItem = theEquipments[0];
                    }
                }
                else if (button == MouseButtons.ScrollUp)
                {
                    bool Changed = false;
                    if (TheCurrentItem.OrderID == 0)
                    {
                        for (int i = theEquipments.Count - 1; i >= 0; i--)
                        {
                            if (theEquipments[i].Amount > 0)
                            {
                                TheCurrentItem = theEquipments[i];
                                Changed = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = TheCurrentItem.OrderID - 1; i >= 0; i--)
                        {
                            if (theEquipments[i].Amount > 0)
                            {
                                TheCurrentItem = theEquipments[i];
                                Changed = true;
                                break;
                            }
                        }
                    }
                    if (!Changed)
                    {
                        TheCurrentItem = theEquipments[0];
                    }
                }             
            }
            UpdateUI(TheCurrentItem);
        }


        protected override void OnButtonPressed(Direction theDirection, int id)
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
                    if(Mathf.Abs(LastDirection.x - MoveDirection.x) != 0)
                    {
                        ourTransform.localPosition = theTileMap.ConvertTileToWorld(theTileMap.ConvertWorldToTile(ourTransform.position));
                        ourTransform.localPosition = new Vector3(ourTransform.localPosition.x, ourTransform.localPosition.y, -1);
                    }
                    else if(Mathf.Abs(LastDirection.y - MoveDirection.y) != 0)
                    {
                        ourTransform.localPosition = theTileMap.ConvertTileToWorld(theTileMap.ConvertWorldToTile(ourTransform.position));
                        ourTransform.localPosition = new Vector3(ourTransform.localPosition.x, ourTransform.localPosition.y, -1);
                    }
                    LastDirection = MoveDirection;
                   
                }
            }
        }

        protected override void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            if (senderID == player_id)
            {
                if (tag == NetworkingTags.Player)
                {
                    if(subject == NetworkingTags.PlayerSubjects.UpdatePostion)
                    {
                        Vector2 newTilePos = (Vector2)data;
                        if(newTilePos != nextTilePos)
                        {
                            transform.position = theTileMap.ConvertTileToWorld(newTilePos);
                        }
                    }
                }
            }
        }
    }
}