using UnityEngine;
using System.Collections;

namespace Roland
{
    public class BombsParent : MonoBehaviour
    {
        public float damage = 50f;
        public int BombPower;
        public int TimeToExplode = 2;
        public Player ParentPlayer = null;
        WaitForSeconds WaitTillExplode;
        protected TileMap theTileMap;
        protected int x, y;
        public AudioClip theClipToPlayWhenExplode = null;
        // Use this for initialization
        void Start()
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
            Debug.Log("Setting tile at x: " + x + " y: " + y);
            theTileMap.theMap.SetTileAt(new Vector2(x, y), new InvisibleWallBlock());
        }

        IEnumerator CountDown()
        {
            yield return WaitTillExplode;
            Explode();
        }

        public void SetTilePos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        protected virtual void Explode() { }

        public void SpawnExplosion(int x, int y)
        {
            ObjectSpawner.SpawnObject("Explosion", new Vector2(x, y));
        }
        public void SpawnExplosion(Vector2 tile)
        {
            ObjectSpawner.SpawnObject("Explosion", tile);
        }
        
        public void Update()
        {
            if (!(theTileMap.theMap.GetTileAt(new Vector2(x,y)) is InvisibleWallBlock))
                theTileMap.theMap.SetTileAt(new Vector2(x, y), new InvisibleWallBlock());
        }
    }
}