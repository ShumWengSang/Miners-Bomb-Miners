using UnityEngine;
using System.Collections;
using DarkRift;

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
            StartCoroutine(waitTime());
        }

        protected override void OnSpawn()
        {
            Init();
            DontActivate = true;
            DarkRiftAPI.onData += ReceiveData;
        }

        protected override void OnDespawn()
        {
            base.OnDespawn();
            DarkRiftAPI.onData -= ReceiveData;
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

        void ReceiveData(byte tag, ushort subject, object data)
        {
            if(tag == NetworkingTags.Misc)
            {
                if(subject == NetworkingTags.MiscSubjects.MineExplode)
                {
                    Vector2 pos = (Vector2)data;
                    if(pos.Equals(this.Pos))
                    {
                        MineExplosion();
                    }
                }
            }
        }

        string Player = "Player";
        string Explosion = "Explosion";

        override protected void OnTriggerEnter2D(Collider2D theCollider)
        {
            if (!DontActivate)
            {
                if (theCollider.CompareTag(Player) || theCollider.CompareTag(Explosion))
                {
                    MineExplosion();
                }
            }
        }

        void MineExplosion()
        {
            DigSpawnTile(x, y, BombPower);
            DigSpawnTile(x + 1, y, BombPower);
            DigSpawnTile(x - 1, y, BombPower);
            DigSpawnTile(x, y + 1, BombPower);
            DigSpawnTile(x, y - 1, BombPower);
            DarkRiftAPI.SendMessageToOthers(NetworkingTags.Misc, NetworkingTags.MiscSubjects.MineExplode, Pos);
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
