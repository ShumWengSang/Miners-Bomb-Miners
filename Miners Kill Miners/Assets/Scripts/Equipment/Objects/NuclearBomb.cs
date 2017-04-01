using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roland
{
    public class NuclearBomb : BombsParent
    {
        [Range(0, 100)]
        public float PercentageDamage;
        List<Vector2> ListToExplode;
        public int RadiusByTiles = 30;

        protected override void Init()
        {
            base.Init();
            PercentageDamage *= 0.01f;

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
                DigSpawnTile(ListToExplode[i], BombPower, BombDamage);
                yield return StartCoroutine(WaitForRealSeconds(0.01f)) ;
            }
            theSrc.Play(theClipToPlayWhenExplode);
            Lean.LeanPool.Despawn(this.gameObject);

        }

        public static IEnumerator WaitForRealSeconds(float delay)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + delay)
            {
                yield return null;
            }
        }
    }
}