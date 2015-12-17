using UnityEngine;
using System.Collections;

namespace Roland
{
    public class TNTBomb : BombsParent
    {

        protected override void Explode()
        {
            for (int i = x - 2; i <= x + 2; i++)
            {
                for (int j = y - 2; j <= y + 2; j++)
                {
                    theTileMap.DigTile(i, j, BombPower, "Explosion");
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                theTileMap.DigTile(x + 3, y + i, BombPower, "Explosion");
                theTileMap.DigTile(x - 3, y + i, BombPower, "Explosion");
                theTileMap.DigTile(x + i, y + 3, BombPower, "Explosion");
                theTileMap.DigTile(x + i, y - 3, BombPower, "Explosion");
            }

            ObjectSpawner.SpawnObject("Explosion", new Vector2(x, y));
            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            DestroyObject(this.gameObject);
        }
    }
}