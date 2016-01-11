using UnityEngine;
using System.Collections;

namespace Roland
{
    public class MineBomb : BombsParent
    {
        protected bool DontActivate = true;
        public float time;
        WaitForSeconds wait;
        protected override void Explode()
        {
            wait = new WaitForSeconds(time);
            theTileMap.theMap.SetTileAt(Pos, new Noblock());
            Debug.Log("Setting tile to noblock " + Pos);
            StartCoroutine(waitTime());
        }

        protected override void OnSpawn()
        {
            Init();
            DontActivate = true;
        }

        IEnumerator waitTime()
        {
            yield return wait;
            DontActivate = false;
        }

        public override void Update()
        {
            //Do nothing. We need this to override the base update.
        }

        void OnTriggerEnter2D(Collider2D theCollider)
        {
            Debug.Log("TRIGGERED");
            if (!DontActivate)
            {
                if (theCollider.CompareTag("Player"))
                {
                    DigSpawnTile(x , y, BombPower);
                    DigSpawnTile(x + 1, y, BombPower);
                    DigSpawnTile(x - 1, y, BombPower);
                    DigSpawnTile(x, y + 1, BombPower);
                    DigSpawnTile(x, y - 1, BombPower);
                    Lean.LeanPool.Despawn(this.gameObject);
                }
            }
        }
    }
}
