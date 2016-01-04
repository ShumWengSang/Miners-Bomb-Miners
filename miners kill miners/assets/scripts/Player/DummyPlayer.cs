using UnityEngine;
using System.Collections;

namespace Roland
{
    public class DummyPlayer : MonoBehaviour
    {
        Transform ourTransform;
        public int id;
        Animator theAnimator;
        TileMap theTileMap;

        Vector3 Offset;
        SpriteRenderer sp;
        void Start()
        {
            sp = GetComponent<SpriteRenderer>();
            theAnimator = GetComponent<Animator>();
            ourTransform = GetComponent<Transform>();
            DarkRift.DarkRiftAPI.onDataDetailed += ReceiveData;
            DarkRift.DarkRiftAPI.onPlayerDisconnected += OnThePlayerDisconnected;
            EventManager.OnMouseButtonDown += OnMouseButtonDown;
            theTileMap = TileMapInterfacer.Instance.TileMap;
            tilePosBlocker = new Vector2(-1, -1);
        }

        Vector2 tilePosBlocker;
        Vector2 currentTilePos;
        void OnDestroy()
        {
            DarkRift.DarkRiftAPI.onDataDetailed -= ReceiveData;
            EventManager.OnMouseButtonDown -= OnMouseButtonDown;
        }
        // Update is called once per frame
        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            if (senderID == id)
            {
                if (tag == NetworkingTags.Player)
                {
                    if (subject == NetworkingTags.PlayerSubjects.UpdatePostion)
                    {
                        Vector3 dataV = (Vector3)data;
                        ourTransform.position = dataV;

                        currentTilePos = theTileMap.ConvertWorldToTile(dataV - Offset);
                        theTileMap.UpdateTexture(tilePosBlocker, new Noblock());
                        theTileMap.UpdateTexture(currentTilePos, new InvisibleWallBlock());
                        tilePosBlocker = currentTilePos;
                    }
                    else if (subject == NetworkingTags.PlayerSubjects.DestroyMapTile)
                    {
                        Vector2 dataV = (Vector2)data;
                        TileMapInterfacer.Instance.TileMap.DigTile(dataV, 99999, null, false);
                    }
                    else if (subject == NetworkingTags.PlayerSubjects.ChangeDir)
                    {
                        Direction theDir = (Direction)data;
                        ChangeDirection(theDir);
                    }
                    else if(subject == NetworkingTags.PlayerSubjects.PlayerDied)
                    {
                        Destroy(this.gameObject);
                    }
                    else if(subject == NetworkingTags.PlayerSubjects.ChangeBlockToNonMovable)
                    {
                        Vector2 dataV = (Vector2)data;
                        theTileMap.theMap.SetTileAt(dataV, new InvisibleWallBlock());
                       // .theMap.SetTileAt(currentTilePos, new InvisibleWallBlock());
                    }
                }
            }
        }
        void OnThePlayerDisconnected(ushort id )
        {
            if(id == this.id)
            {
                Destroy(this.gameObject);
            }
        }

        void MouseButtonSpawn(Items_e theItem)
        {
            
            switch (theItem)
            {
                case Items_e.SmallBomb:
                   ObjectSpawner.SpawnObject("SmallBomb", ourTransform.position);
                    break;
                case Items_e.BigBomb:
                    ObjectSpawner.SpawnObject("BigBomb", ourTransform.position);
                    break;
                case Items_e.TNTBomb:
                   ObjectSpawner.SpawnObject("TNTBomb", ourTransform.position);
                    break;
                case Items_e.NuclearBomb:

                    break;
                default:
                    break;
            }
        }

        public void OnMouseButtonDown(MouseButtons button, int id, Items_e theItem)
        {
            if (id == this.id)
            {
                if (button == MouseButtons.left)
                {
                    Debug.Log("Spawning bomb");
                    MouseButtonSpawn(theItem);
                }
            }
        }

        public void ChangeDirection(Direction theDirection)
        {
            //We make sure this is talking to us

            switch (theDirection)
            {
                case Direction.Up:
                    if (sp.sprite != null)
                    {
                        Offset = sp.sprite.bounds.extents;
                    }
                    theAnimator.SetTrigger("Move Up");
                    break;
                case Direction.Down:
                    if (sp.sprite != null)
                    {
                        Offset = -sp.sprite.bounds.extents;
                        Offset -= Offset * 0.01f;

                    }
                    theAnimator.SetTrigger("Move Down");
                    break;
                case Direction.Left:
                    if (sp.sprite != null)
                    {
                        Offset = -sp.sprite.bounds.extents;
                        Offset -= Offset * 0.01f;
                    }
                    theAnimator.SetTrigger("MoveLeft");
                    break;
                case Direction.Right:
                    if (sp.sprite != null)
                    {
                        Offset = sp.sprite.bounds.extents;
                    }
                    theAnimator.SetTrigger("MoveRight");
                    break;
                case Direction.Stop:
                    theAnimator.SetTrigger("Stop");
                    break;
            }
        }
    }
}

