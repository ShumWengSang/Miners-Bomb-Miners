using UnityEngine;
using System.Collections;

namespace Roland
{
    public class SmallBomb : MonoBehaviour
    {
        public int TimeToExplode = 2;
        int currentTime;
        WaitForSeconds WaitTillExplode;
        TileMap theTileMap;
        int x, y;
        // Update is called once per frame
        void Start()
        {
            WaitTillExplode = new WaitForSeconds(TimeToExplode);
            theTileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
            x = 0; y = 0;
            Vector2 tilePos = theTileMap.ConvertWorldToTile(transform.position);
            SetTilePos((int)tilePos.x, (int)tilePos.y);
            transform.position = theTileMap.ConvertTileToWorld(tilePos);
            StartCoroutine(CountDown());
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
        void Explode()
        {
            Debug.Log("Explode");
            theTileMap.DigTile(x + 1, y, 100);
            theTileMap.DigTile(x - 1, y, 100);
            theTileMap.DigTile(x, y + 1, 100);
            theTileMap.DigTile(x, y - 1, 100);
            DestroyObject(this.gameObject);
        }
    }
}
