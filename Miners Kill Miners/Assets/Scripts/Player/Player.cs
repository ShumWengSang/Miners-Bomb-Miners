
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

    public enum WinLoseDraw
    {
        Win = 0,
        Lose,
        Draw
    }

    public class Player : PlayerBase
    {
        public bool isControllable;
        bool DigCooldown = false;
        bool invul = false;

        public PlayerData thePlayerData;

        public int speed = 5;
        public int player_id;
        public int CurrentHealthPoints;

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

        Direction ourDirection;

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


            ourTransform = transform.parent;
            CurrentHealthPoints = CurrentPlayer.Instance.HealthPoints;
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
            //UpdateHealth(CurrentHealthPoints);
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
            CurrentPlayer.Instance.UpdateHealthPointInGame(health, true);
        }

        void OnDespawn()
        {
            DarkRiftAPI.SendMessageToServer(NetworkingTags.Server, NetworkingTags.ServerSubjects.ILose, "");
            OnDestroy();
        }


        void OnDestroy()
        {
            base.deInit();
        }

        public void MinusHealthPoints(int damage, ushort explosionid)
        {
            if (!invul)
            {
                StartCoroutine(InvulCoolDown());
                CurrentHealthPoints -= damage;

                UpdateHealth(CurrentHealthPoints);
                ourPlayerHealthBar.RunDamagedImage();
                if (CurrentHealthPoints <= 0)
                {
                    //we lose.
                    Lean.LeanPool.Despawn(this.transform.root);
                    //tell controller to check who wins, if any.
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.PlayerDied, explosionid);
                    Debug.Log("Adding kill player " + this.id + " explosion id " + explosionid);
                    KillTrackSystem.Instance.AddKill((ushort)this.player_id, explosionid);                    
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
                ourTransform.position += new Vector3(MoveDirection.x, MoveDirection.y, 0) * Time.deltaTime * speed;
                //DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.ChangeBlockToNonMovable, currentTilePos);
               
                
                //rb.velocity = MoveDirection * Time.deltaTime * speed;
            }
            else if(!DigCooldown)
            {
                //Dig. We start coroutine to do the cooldown as well.
                StartCoroutine(DigCooldownUpdate());
                theTileMap.DigTile(nextTilePos, CurrentPlayer.Instance.DigPower);
                //rb.velocity = Vector3.zero * Time.deltaTime * speed;
            }
        }
        protected override void OnMouseButtonDown(MouseButtons button, int id, int theItemID)
        {
            if (id == player_id)
            {
                if (button == MouseButtons.left)
                {
                    if (theTileMap.theMap.GetTileAt(currentTilePos) is Noblock || theTileMap.theMap.GetTileAt(currentTilePos) is InvisibleWallBlock)
                    {
                        if (TheCurrentItem is GrenadeData)
                        {
                            GrenadeData grenade = (GrenadeData)TheCurrentItem;
                            grenade.SetDirection(ourDirection);
                        }
                        GameObject obj = TheCurrentItem.PlayerSpawnBomb(theTileMap.ConvertTileToWorld(currentTilePos));
                        if (obj != null)
                        {
                            RemoteBomb remote = obj.GetComponent<RemoteBomb>();
                            if (remote != null)
                            {
                                remote.id = id;
                            }
                            if (DarkRiftAPI.isConnected)
                            {
                                DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.leftMouseButton, TheCurrentItem.OrderID);
                            }
                            obj.GetComponent<BombsParent>().ID = id;
                        }
                    }
                }
                else if(button == MouseButtons.right)
                {
                    ActivateRemote(DarkRiftAPI.id);
                    if (DarkRiftAPI.isConnected)
                    {
                        DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.rightMouseButton, DarkRiftAPI.id);
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
                ourDirection = theDirection;
                DarkRiftAPI.SendMessageToOthers(NetworkingTags.Player, NetworkingTags.PlayerSubjects.ChangeDir, theDirection);
                if (LastDirection != MoveDirection)
                {
                    if(Mathf.Abs(LastDirection.x - MoveDirection.x) != 0)
                    {
                        ourTransform.position = theTileMap.ConvertTileToWorld(theTileMap.ConvertWorldToTile(ourTransform.position));
                        ourTransform.position = new Vector3(ourTransform.position.x, ourTransform.position.y, -1);
                    }
                    else if(Mathf.Abs(LastDirection.y - MoveDirection.y) != 0)
                    {
                        ourTransform.position = theTileMap.ConvertTileToWorld(theTileMap.ConvertWorldToTile(ourTransform.position));
                        ourTransform.position = new Vector3(ourTransform.position.x, ourTransform.position.y, -1);
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

        string Explosion = "Explosion";
        string EndExplosion = "EndExplosion";
        string Gold = "Gold";
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(Explosion))
            {
                Explosion theEx = collider.GetComponentInParent<Explosion>();
                this.MinusHealthPoints((int)theEx.damage, (ushort)theEx.ID);
            }
            else if(collider.CompareTag(Gold))
            {
                CurrentPlayer.Instance.Money += collider.GetComponent<Gold>().MoneyGiven;
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Misc, NetworkingTags.MiscSubjects.GoldPickedUp, collider.transform.position);
            }
            else if(collider.CompareTag(EndExplosion))
            {
                this.MinusHealthPoints(999, 999);
            }
        }
    }
}