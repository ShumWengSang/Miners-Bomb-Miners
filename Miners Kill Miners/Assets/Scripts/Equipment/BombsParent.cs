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
        // Use this for initialization
        void Start()
        {
            WaitTillExplode = new WaitForSeconds(TimeToExplode);
            theTileMap = TileMapInterfacer.Instance.TileMap;
            Vector2 tilePos = theTileMap.ConvertWorldToTile(transform.position);
            SetTilePos((int)tilePos.x, (int)tilePos.y);
            transform.position = theTileMap.ConvertTileToWorld(tilePos);
            StartCoroutine(CountDown());

            //  InvisibleWallBlock
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
    }
}