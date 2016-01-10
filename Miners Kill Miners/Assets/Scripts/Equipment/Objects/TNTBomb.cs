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
                    DigSpawnTile(i, j, BombPower);
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                DigSpawnTile(x + 3, y + i, BombPower);
                DigSpawnTile(x - 3, y + i, BombPower);
                DigSpawnTile(x + i, y + 3, BombPower);
                DigSpawnTile(x + i, y - 3, BombPower);
            }

            SpawnExplosion(x, y);
            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}