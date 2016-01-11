using UnityEngine;
using System.Collections;

namespace Roland
{
    public class DummyPlayer : PlayerBase
    {
        Direction theDirection;
        void Start()
        {
            base.Init();
        }
        void OnDestroy()
        {
            base.deInit();
        }
        // Update is called once per frame
        protected override void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
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
                        TileMapInterfacer.Instance.TileMap.DigTile(dataV, 99999, false);
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
        protected override void OnThePlayerDisconnected(ushort id)
        {
            if(id == this.id)
            {
                Destroy(this.gameObject);
            }
        }

        protected override void OnMouseButtonDown(MouseButtons button, int id, int theItem)
        {
            if (id == this.id)
            {
                if (button == MouseButtons.left)
                {
                    if (CurrentPlayer.Instance.AmountOfEquipments[theItem] is GrenadeData)
                    {
                        GrenadeData grenade = (GrenadeData)CurrentPlayer.Instance.AmountOfEquipments[theItem];
                        grenade.SetDirection(theDirection);
                    }
                    GameObject obj = CurrentPlayer.Instance.AmountOfEquipments[theItem].DummySpawnBomb(this.ourTransform.position);
                    RemoteBomb remote = obj.GetComponent<RemoteBomb>();
                    if(remote != null)
                    {
                        remote.id = id;
                    }
                }
                else if (button == MouseButtons.right)
                {
                    ActivateRemote(id);
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
            this.theDirection = theDirection;
        }
    }
}

