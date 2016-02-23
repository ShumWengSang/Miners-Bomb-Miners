using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roland
{
    public class NuclearBomb : BombsParent
    {
        WaitForSeconds wait;
        List<Vector2> ListToExplode;
        public int RadiusByTiles = 30;

        void Start()
        {
            wait = new WaitForSeconds(0.01f);
        }

        protected override void OnSpawn()
        {
            Init();
            GetComponent<Animator>().enabled = true;
        }

        protected override void Explode()
        {
            ListToExplode = theTileMap.GetCircleTiles(Pos, RadiusByTiles);

            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = null;
            StartCoroutine(BlowItUpBaby());
            Exploded = true;
        }

        IEnumerator BlowItUpBaby()
        {
            for (int i = 0; i < ListToExplode.Count; i++)
            {
                DigSpawnTile(ListToExplode[i], BombPower);
                yield return wait;
            }
            theSrc.Play();
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}