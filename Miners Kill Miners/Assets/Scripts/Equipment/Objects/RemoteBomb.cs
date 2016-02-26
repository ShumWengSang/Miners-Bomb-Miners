using UnityEngine;
using System.Collections;

namespace Roland
{
    public class RemoteBomb : BombsParent
    {
        public int id;
        protected override void OnDespawn()
        {
            base.OnDespawn();
            Player.OnRemoteActivate -= RemoteExplode;
        }
        protected override void OnSpawn()
        {
            Init();
            Player.OnRemoteActivate += RemoteExplode;
        }


        protected override void Explode()
        {
        }

        protected virtual void RemoteExplode(int id)
        {
            if (id == this.id)
            {
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        DigSpawnTile(i, j, BombPower);
                    }
                }
                DigSpawnTile(x + 2, y, BombPower);
                DigSpawnTile(x - 2, y, BombPower);
                DigSpawnTile(x, y + 2, BombPower);
                DigSpawnTile(x, y - 2, BombPower);
                theSrc.Play();
                SpawnExplosion(x, y, BombDamage);
                StartCoroutine(DestroyObject());
            }
        }

        protected IEnumerator DestroyObject()
        {
            transform.position = new Vector3(-1000,-1000,-1000);
            yield return null;
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
