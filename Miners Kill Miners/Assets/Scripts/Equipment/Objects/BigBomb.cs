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
                    DigSpawnTile(i, j, BombPower);
                }
            }
            DigSpawnTile(x + 2, y, BombPower);
            DigSpawnTile(x - 2, y, BombPower);
            DigSpawnTile(x, y + 2, BombPower);
            DigSpawnTile(x, y - 2, BombPower);

            SpawnExplosion(x, y);
            theSrc.Play();
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}