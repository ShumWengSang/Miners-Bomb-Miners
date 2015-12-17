using UnityEngine;
using System.Collections;

namespace Roland
{
    public class BigBomb : BombsParent
    {

        protected override void Explode()
        {
            for(int i = x - 1; i <= x + 1; i ++)
            {
                for(int j = y - 1; j <= y + 1; j++)
                {
                    theTileMap.DigTile(i , j, BombPower, "Explosion");
                }
            }
            theTileMap.DigTile(x + 2, y, BombPower, "Explosion");
            theTileMap.DigTile(x - 2, y, BombPower, "Explosion");
            theTileMap.DigTile(x, y + 2, BombPower, "Explosion");
            theTileMap.DigTile(x, y - 2, BombPower, "Explosion");

            ObjectSpawner.SpawnObject("Explosion", new Vector2(x, y));
            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            DestroyObject(this.gameObject);
        }
    }
}