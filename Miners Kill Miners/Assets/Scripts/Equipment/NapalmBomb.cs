using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roland
{
    public class NapalmBomb : BombsParent
    {
        List<Vector2> BlocksToExplode = new List<Vector2>();
        public int TotalAmountOfBlocksToDestroy = 10;

        protected override void Explode()
        {
            int Count = 0;
            Queue<Vector2> q = new Queue<Vector2>();
            q.Enqueue(new Vector2(x, y));
            while (q.Count > 0)
            {
                Vector2 current = q.Dequeue();
                if (current == null)
                    continue;
                if (theTileMap.theMap.GetTileAt(new Vector2(x + 1, y)) is Noblock || theTileMap.theMap.GetTileAt(new Vector2(x + 1, y)) is InvisibleWallBlock)
                {
                    q.Enqueue(new Vector2(x + 1, y));
                }
                if (theTileMap.theMap.GetTileAt(new Vector2(x - 1, y)) is Noblock || theTileMap.theMap.GetTileAt(new Vector2(x - 1, y)) is InvisibleWallBlock)
                {
                    q.Enqueue(new Vector2(x - 1, y));
                }
                if (theTileMap.theMap.GetTileAt(new Vector2(x, y + 1)) is Noblock || theTileMap.theMap.GetTileAt(new Vector2(x , y + 1)) is InvisibleWallBlock)
                {
                    q.Enqueue(new Vector2(x, y + 1));
                }
                if (theTileMap.theMap.GetTileAt(new Vector2(x, y - 1)) is Noblock || theTileMap.theMap.GetTileAt(new Vector2(x , y - 1)) is InvisibleWallBlock)
                {
                    q.Enqueue(new Vector2(x, y - 1));
                }

                BlocksToExplode.Add(current);
                Count++;
                if (Count >= TotalAmountOfBlocksToDestroy)
                {
                    break;
                }  
            }

            StartCoroutine(explodeNapalm());

            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            DestroyObject(this.gameObject);
        }

        IEnumerator explodeNapalm()
        {
            for(int i = 0; i < BlocksToExplode.Count; i++)
            {
                SpawnExplosion(BlocksToExplode[i]);
                yield return new WaitForSeconds(0.1f);
            }
        }

        void SpawnExplosion(int x, int y)
        {
            ObjectSpawner.SpawnObject("Explosion", new Vector2(x, y));
        }
        void SpawnExplosion(Vector2 tile)
        {
            ObjectSpawner.SpawnObject("Explosion", tile);
        }
    }
}
