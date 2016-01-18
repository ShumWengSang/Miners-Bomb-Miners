using UnityEngine;
using System.Collections;
namespace Roland
{
    public class FogOfWar : MonoBehaviour
    {
        public GameObject FogPrefab;
        TileMap tm;
        GameObject[,] FogObjects;
        public GameObject FogParent;
        public void Init()
        {
            tm = TileMapInterfacer.Instance.TileMap;
            FogObjects = new GameObject[tm.size_x, tm.size_z];
        }

        public void SetFogOff(Vector2 tile)
        {
            SetFogOff((int)tile.x, (int)tile.y);
        }

        public void SetFogOff(int x, int y)
        {
            FogObjects[x, y].SetActive(false);
        }

       public void CreateFogOfWar()
        {
            if (tm == null)
                tm = TileMapInterfacer.Instance.TileMap;
            for(int i = 1; i < tm.size_x - 1; i ++)
            {
                for(int j = 1; j < tm.size_z - 1; j++)
                {
                    CreateFogOverTile(i, j);
                }
            }
        }
        void CreateFogOverTile(int x, int y)
        {
            CreateFogOverTile(new Vector2(x, y));
        }

        void CreateFogOverTile(Vector2 tile)
        {
            if(!(tm.theMap.GetTileAt(tile) is WallBlock))
            {
                Vector2 tileC = tm.ConvertTileToWorld(tile);
                FogObjects[(int)tile.x, (int)tile.y] = Lean.LeanPool.Spawn(FogPrefab, new Vector3(tileC.x, tileC.y, -2), Quaternion.identity, FogParent.transform);
            }
        }
    }
}
