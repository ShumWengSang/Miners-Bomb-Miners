using UnityEngine;
using System.Collections;

namespace Roland
{
    public class SmallBomb : BombsParent
    {

        protected override void Explode()
        {
            theTileMap.DigTile(x + 1, y, BombPower, "Explosion");
            theTileMap.DigTile(x - 1, y, BombPower, "Explosion");
            theTileMap.DigTile(x, y + 1, BombPower, "Explosion");
            theTileMap.DigTile(x, y - 1, BombPower, "Explosion");
            ObjectSpawner.SpawnObject("Explosion", new Vector2(x, y));
            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            DestroyObject(this.gameObject);
        }
    }
}
