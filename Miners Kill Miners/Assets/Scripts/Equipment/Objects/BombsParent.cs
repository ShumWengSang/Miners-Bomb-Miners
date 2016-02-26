using UnityEngine;
using System.Collections;

namespace Roland
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BombsParent : MonoBehaviour
    {
        public int BombDamage = 1;
        public int BombPower;
        public int TimeToExplode = 2;
        public PlayerBase ParentPlayer = null;
        WaitForSeconds WaitTillExplode;
        protected TileMap theTileMap;
        protected int x, y;
        protected Vector2 Pos;
        public AudioClip theClipToPlayWhenExplode = null;
        public int ID = 999;
        protected AudioSource theSrc;
        protected bool Exploded = false;
        // Use this for initialization
        protected virtual void Init()
        {
            Exploded = false;
            WaitTillExplode = new WaitForSeconds(TimeToExplode);
            theTileMap = TileMapInterfacer.Instance.TileMap;
            Vector2 tilePos = theTileMap.ConvertWorldToTile(transform.position);
            SetTilePos((int)tilePos.x, (int)tilePos.y);
            transform.position = theTileMap.ConvertTileToWorld(tilePos);
            StartCoroutine(CountDown());

            theSrc = this.gameObject.AddComponent<AudioSource>();
            theSrc.clip = theClipToPlayWhenExplode;
            theSrc.playOnAwake = false;
            theSrc.loop = false;
            //  InvisibleWallBlock
            theTileMap.theMap.SetTileAt(Pos, new InvisibleWallBlock());
        }
        protected virtual void OnSpawn()
        {
            Init();
        }
        protected virtual void OnDespawn()
        {
            theTileMap.UpdateTexture(Pos, new Noblock());
        }

        IEnumerator CountDown()
        {
            yield return WaitTillExplode;
            Explode();
        }

        public void SetTilePos(int x, int y)
        {
            Pos = new Vector2(x, y);
            this.x = x;
            this.y = y;
        }

        protected virtual void Explode() { }

        public GameObject SpawnExplosion(int x, int y, int damage)
        {
            GameObject explo = ObjectSpawner.SpawnObject("Explosion", new Vector2(x, y));
            Explosion exp = explo.GetComponent<Explosion>();
            exp.ID = this.ID;
            exp.damage = damage;
            
            return explo;
        }
        public GameObject SpawnExplosion(Vector2 tile, int damage)
        {
             GameObject explo = ObjectSpawner.SpawnObject("Explosion", tile);
             Explosion exp = explo.GetComponent<Explosion>();
             exp.ID = this.ID;
             exp.damage = damage;
             return explo;
        }
        
        public virtual void Update()
        {
            if (!(theTileMap.theMap.GetTileAt(new Vector2(x,y)) is InvisibleWallBlock))
                theTileMap.theMap.SetTileAt(new Vector2(x, y), new InvisibleWallBlock());
        }

        protected GameObject DigSpawnTile(int x, int y, int BombPower, int Damage = 1)
        {
            if(theTileMap.DigTile(x , y, BombPower))
            {
                return SpawnExplosion(x, y, Damage);
            }
            return null;
        }

        protected GameObject DigSpawnTile(Vector2 tile, int BombPower, int Damage = 1)
        {
            if (theTileMap.DigTile((int)tile.x, (int)tile.y, BombPower))
            {
                return SpawnExplosion((int)tile.x, (int)tile.y, Damage);
            }
            return null;
        }

        virtual protected void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Explosion"))
            {
                if (!this.Exploded)
                    Explode();
            }
        }
    }
}