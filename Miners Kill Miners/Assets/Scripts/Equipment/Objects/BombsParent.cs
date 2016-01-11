using UnityEngine;
using System.Collections;

namespace Roland
{
    public class BombsParent : MonoBehaviour
    {
        public int BombPower;
        public int TimeToExplode = 2;
        public PlayerBase ParentPlayer = null;
        WaitForSeconds WaitTillExplode;
        protected TileMap theTileMap;
        protected int x, y;
        protected Vector2 Pos;
        public AudioClip theClipToPlayWhenExplode = null;
        // Use this for initialization
        protected virtual void Init()
        {
            WaitTillExplode = new WaitForSeconds(TimeToExplode);
            theTileMap = TileMapInterfacer.Instance.TileMap;
            Vector2 tilePos = theTileMap.ConvertWorldToTile(transform.position);
            SetTilePos((int)tilePos.x, (int)tilePos.y);
            transform.position = theTileMap.ConvertTileToWorld(tilePos);
            StartCoroutine(CountDown());

            AudioSource theSrc = gameObject.AddComponent<AudioSource>();
            theSrc.clip = theClipToPlayWhenExplode;
            theSrc.playOnAwake = false;
            //  InvisibleWallBlock
            theTileMap.theMap.SetTileAt(Pos, new InvisibleWallBlock());
        }
        protected virtual void OnSpawn()
        {
            Init();
        }
        protected virtual void OnDespawn()
        {
            theTileMap.theMap.SetTileAt(Pos, new Noblock());
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

        public GameObject SpawnExplosion(int x, int y)
        {
            return ObjectSpawner.SpawnObject("Explosion", new Vector2(x, y));
        }
        public GameObject SpawnExplosion(Vector2 tile)
        {
            return ObjectSpawner.SpawnObject("Explosion", tile);
        }
        
        public virtual void Update()
        {
            if (!(theTileMap.theMap.GetTileAt(new Vector2(x,y)) is InvisibleWallBlock))
                theTileMap.theMap.SetTileAt(new Vector2(x, y), new InvisibleWallBlock());
        }

        protected GameObject DigSpawnTile(int x, int y, int BombPower)
        {
            if(theTileMap.DigTile(x , y, BombPower))
            {
                return SpawnExplosion(x, y);
            }
            return null;
        }

        protected GameObject DigSpawnTile(Vector2 tile, int BombPower)
        {
            if (theTileMap.DigTile((int)tile.x, (int)tile.y, BombPower))
            {
                return SpawnExplosion((int)tile.x, (int)tile.y);
            }
            return null;
        }
    }
}