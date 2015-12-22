using UnityEngine;
using System.Collections;

namespace Roland
{
    public class DummyPlayer : MonoBehaviour
    {
        Transform ourTransform;
        public int id;
        Animator theAnimator;

        void Start()
        {
            theAnimator = GetComponent<Animator>();
            ourTransform = GetComponent<Transform>();
            DarkRift.DarkRiftAPI.onDataDetailed += ReceiveData;
            EventManager.OnMouseButtonDown += OnMouseButtonDown;
        }

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
                    }
                    else if (subject == NetworkingTags.PlayerSubjects.DestroyMapTile)
                    {
                        Vector2 dataV = (Vector2)data;
                        TileMapInterfacer.Instance.TileMap.DigTile(dataV, 99999);
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
                }

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
                    theAnimator.SetTrigger("Move Up");
                    break;
                case Direction.Down:
                    theAnimator.SetTrigger("Move Down");
                    break;
                case Direction.Left:

                    theAnimator.SetTrigger("MoveLeft");
                    break;
                case Direction.Right:

                    theAnimator.SetTrigger("MoveRight");
                    break;
                case Direction.Stop:
                    theAnimator.SetTrigger("Stop");
                    break;
            }
        }
    }
}

