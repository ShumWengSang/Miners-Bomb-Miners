using UnityEngine;
using System.Collections;

namespace Roland
{
    public class SmallBomb : BombsParent
    {

        protected override void Explode()
        {
            DigSpawnTile(x + 1, y, BombPower);
            DigSpawnTile(x - 1, y, BombPower);
            DigSpawnTile(x, y + 1, BombPower);
            DigSpawnTile(x, y - 1, BombPower);
            theSrc.Play(theClipToPlayWhenExplode);
            SpawnExplosion(x, y, BombDamage);
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
