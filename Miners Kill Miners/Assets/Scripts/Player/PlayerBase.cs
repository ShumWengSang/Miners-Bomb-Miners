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

        public delegate void RemoteBombExplode(int id);
        public static event RemoteBombExplode OnRemoteActivate;

        protected void ActivateRemote(int id)
        {
            if(OnRemoteActivate != null)
                OnRemoteActivate(id);
        }

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

    }
}
