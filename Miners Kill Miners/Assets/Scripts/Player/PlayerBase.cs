using UnityEngine;
using System.Collections;
namespace Roland
{
    public class PlayerBase : MonoBehaviour
    {
        protected Transform ourTransform;
        public int id;
        protected Animator theAnimator;
        protected TileMap theTileMap;

        protected Vector3 Offset;

        protected Vector2 tilePosBlocker;
        protected Vector2 currentTilePos;

        protected SpriteRenderer sp;

        // Use this for initialization
        protected void Init()
        {
            sp = GetComponent<SpriteRenderer>();
            theAnimator = GetComponent<Animator>();
            ourTransform = GetComponent<Transform>();
            DarkRift.DarkRiftAPI.onDataDetailed += ReceiveData;
            DarkRift.DarkRiftAPI.onPlayerDisconnected += OnThePlayerDisconnected;
            EventManager.OnMouseButtonDown += OnMouseButtonDown;
            EventManager.OnKeyboardButtonDown += OnButtonPressed;
            theTileMap = TileMapInterfacer.Instance.TileMap;
            tilePosBlocker = new Vector2(-1, -1);
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        }

        protected void deInit()
        {
            DarkRift.DarkRiftAPI.onDataDetailed -= ReceiveData;
            DarkRift.DarkRiftAPI.onPlayerDisconnected -= OnThePlayerDisconnected;
            EventManager.OnMouseButtonDown -= OnMouseButtonDown;
            EventManager.OnKeyboardButtonDown -= OnButtonPressed;
        }
        virtual protected void OnButtonPressed(Direction theDirection, int id)
        {

        }
        virtual protected void OnMouseButtonDown(MouseButtons button, int id, int theItem)
        {

        }
        virtual protected void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {

        }
        virtual protected void OnThePlayerDisconnected(ushort id)
        {

        }


        // Update is called once per frame
        void Update()
        {

        }

        //protected void MouseButtonSpawn(Items_e theItem)
        //{
        //    GameObject Object;
        //    switch (theItem)
        //    {
        //        case Items_e.SmallBomb:
        //            Object = ObjectSpawner.SpawnObject("SmallBomb", transform.position);
        //            Object.GetComponent<BombsParent>().ParentPlayer = this;
        //            break;
        //        case Items_e.BigBomb:
        //            Object = ObjectSpawner.SpawnObject("BigBomb", transform.position);
        //            Object.GetComponent<BombsParent>().ParentPlayer = this;
        //            break;
        //        case Items_e.TNTBomb:
        //            Object = ObjectSpawner.SpawnObject("TNTBomb", transform.position);
        //            Object.GetComponent<BombsParent>().ParentPlayer = this;
        //            break;
        //        case Items_e.NuclearBomb:

        //            break;
        //        case Items_e.NapalmBomb:
        //            Object = ObjectSpawner.SpawnObject("NapalmBomb", transform.position);
        //            Object.GetComponent<BombsParent>().ParentPlayer = this;
        //            break;
        //        default:
        //            Debug.LogWarning("Item not found or not implemented yet");
        //            break;
        //    }
        //}
    }
}
