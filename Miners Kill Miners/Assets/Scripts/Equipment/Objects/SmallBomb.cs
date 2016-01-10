﻿using UnityEngine;
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

            SpawnExplosion(x, y);
            theTileMap.theMap.SetTileAt(new Vector2(x, y), new Noblock());
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
