using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roland
{
    public class NapalmBomb : BombsParent
    {
        List<Vector2> BlocksToExplode = new List<Vector2>();
        List<Vector2> BlocksChecked = new List<Vector2>();
        public int TotalAmountOfBlocksToDestroy = 10;

        protected override void Explode()
        {
            int Count = 0;
            Queue<Vector2> q = new Queue<Vector2>();
            q.Enqueue(new Vector2(x, y));
            while (q.Count > 0)
            {
                Vector2 current = q.Dequeue();
                BlocksChecked.Add(current);
                Vector2 up = (new Vector2(current.x + 1, current.y));
                Vector2 down = (new Vector2(current.x - 1, current.y));
                Vector2 left = (new Vector2(current.x, current.y + 1));
                Vector2 right = (new Vector2(current.x, current.y - 1));

              //  if (current == null)
                   // continue;
                if (theTileMap.theMap.GetTileAt(right) is Noblock || theTileMap.theMap.GetTileAt(right) is InvisibleWallBlock)
                {
                    if (!BlocksChecked.Contains(right))
                        q.Enqueue(right);
                }
                if (theTileMap.theMap.GetTileAt(left) is Noblock || theTileMap.theMap.GetTileAt(left) is InvisibleWallBlock)
                {
                    if (!BlocksChecked.Contains(left))
                        q.Enqueue(left);
                }
                if (theTileMap.theMap.GetTileAt(up) is Noblock || theTileMap.theMap.GetTileAt(up) is InvisibleWallBlock)
                {
                    if (!BlocksChecked.Contains(up))
                        q.Enqueue(up);
                }
                if (theTileMap.theMap.GetTileAt(down) is Noblock || theTileMap.theMap.GetTileAt(down) is InvisibleWallBlock)
                {
                    if (!BlocksChecked.Contains(down))
                        q.Enqueue(down);
                }

                BlocksToExplode.Add(current);
                Debug.Log("Added " + current + " to explosion list");
                Count++;
                if (Count >= TotalAmountOfBlocksToDestroy)
                {
                    break;
                }
            }

            StartCoroutine(explodeNapalm());

            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            GetComponent<SpriteRenderer>().sprite = null;
           // this.gameObject.SetActive(false);
            //DestroyObject(this.gameObject);
        }

        IEnumerator explodeNapalm()
        {
            for(int i = 0; i < BlocksToExplode.Count; i++)
            {
                SpawnExplosion(BlocksToExplode[i]);
                yield return new WaitForSeconds(0.1f);
            }
            DestroyObject(this.gameObject);
        }
    }
}
