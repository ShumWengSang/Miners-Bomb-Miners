
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


        Vector2 MoveDirection = new Vector2(0, 0);
        Animator theAnimator;
        Transform ourTransform;
        void Awake()
        {
            theAnimator = GetComponent<Animator>();
            InitializePlayer();
            thePlayerData = new PlayerData();

            EventManager.OnKeyboardButtonDown += OnButtonPressed;
            EventManager.OnMouseButtonDown += OnMouseButtonDown;
            ourTransform = this.transform;
            
        }

        void Start()
        {
            theTileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
            if(theTileMap == null)
            {
                Debug.LogWarning("Tile Map not found! Error!");
            }
        }
        void OnDestroy()
        {
            EventManager.OnKeyboardButtonDown -= OnButtonPressed;
            EventManager.OnMouseButtonDown -= OnMouseButtonDown;
        }

        public void InitializePlayer()
        {
            List<Items_e> AllItems = Items_e.GetValues(typeof(Items_e)).Cast<Items_e>().ToList();
            for (int i = 0; i < AllItems.Count; ++i)
            {
                AmountOfItems.Add(AllItems[i], 0);
            }
        }

        void Update()
        {        
           // ourTransform.position = theTileMap.ConvertWorldToTile(ourTransform.position);
            Vector2 CheckNextPosition = MoveDirection + theTileMap.ConvertWorldToTile(transform.position);
            
           // Debug.Log("Our tile pos is " + theTileMap.ConvertWorldToTile(transform.position));
            Block theNextBlock = theTileMap.theMap.GetTileAt(CheckNextPosition);

            if (theNextBlock is Noblock)
            {
                //Debug.Log("Changing transform and move direction is" + MoveDirection);
                ourTransform.localPosition += new Vector3(MoveDirection.x, MoveDirection.y, 0) * Time.deltaTime * speed;
            }
            else
            {
                //dig
               // Debug.Log("Digging Through");
                theTileMap.DigTile(CheckNextPosition, DigPower);
            }
        }
        public void OnMouseButtonDown(int button, int id, Items_e theItem)
        {
            Debug.Log("Mouse pressed");
            if(id == player_id)
            {
                switch(theItem)
                {
                    case Items_e.SmallBomb:
                        GameObject.Instantiate(Resources.Load("SmallBomb"), transform.position, Quaternion.identity);
                        break;
                    default:
                        break;
                }
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
            }
        }

    }
}