using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Roland
{
    public class BigRemoteBomb : RemoteBomb
    {
        public int Radius;
        List<Vector2> ListToExplode;
        protected override void RemoteExplode(int id)
        {
            if (id == this.id)
            {
                for (int i = x - 2; i <= x + 1; i++)
                {
                    for (int j = y - 2; j <= y + 1; j++)
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
                theSrc.Play();
                Lean.LeanPool.Despawn(this.gameObject);
            }
        }
    }
}
