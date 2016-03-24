using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Roland
{
    public class CrossBomb : BombsParent
    {
        List<Vector2> BombPlaces;
        public int Distance = 10;
        TileMap tile;
        void Start()
        {
            tile = TileMapInterfacer.Instance.TileMap;
        }
        protected override void Explode()
        {
            for (int i = 0; i < tile.size_x; i++ )
            {
                DigSpawnTile(i, this.y, BombPower);
            }
            for (int i = 0; i < tile.size_z; i++ )
            {
                DigSpawnTile(this.x, i, BombPower);
            }

            SpawnExplosion(x, y, BombDamage);
            theSrc.Play(theClipToPlayWhenExplode);
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
