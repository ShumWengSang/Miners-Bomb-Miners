using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roland
{
    public class NuclearBomb : BombsParent
    {
        WaitForSeconds wait;
        List<Vector2Class> ListToExplode;
        public int RadiusByTiles = 30;

        void Start()
        {
            wait = new WaitForSeconds(0.01f);
        }

        protected override void Explode()
        {
            ListToExplode = theTileMap.GetCircleTiles(new Vector2(x, y), RadiusByTiles);

            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = null;
            StartCoroutine(BlowItUpBaby());
        }

        IEnumerator BlowItUpBaby()
        {
            for (int i = 0; i < ListToExplode.Count; i++)
            {
                DigSpawnTile(new Vector2(ListToExplode[i].x,ListToExplode[i].z), BombPower);
                yield return wait;
            }
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}