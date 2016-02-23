using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Roland
{
    public class TNTBomb : BombsParent
    {
        public int Radius;
        List<Vector2> ListToExplode;

        protected override void Explode()
        {
            //ListToExplode = theTileMap.GetCircleTiles(Pos, Radius);

            //for (int i = 0; i < ListToExplode.Count; i++)
            //{
            //    DigSpawnTile(ListToExplode[i], BombPower);
            //}
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
            theSrc.Play();
            SpawnExplosion(x, y);
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}