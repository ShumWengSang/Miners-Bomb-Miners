using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Roland
{
    public class GoldSpawnerManager : MonoBehaviour
    {
        static Vector2[] GetRandomGoldTiles(int amount)
        {
            //The general algorithm is to split the gold into 5 different parts.
            //Each player gets amount / 5 gold at his sector, with the last odd one out
            //splitting up to everyone.

            //Vector2[] goldTiles = new Vector2[amount];
            List<Vector2> goldTiles = new List<Vector2>();
            TileMap theTileMap = TileMapInterfacer.Instance.TileMap;

            //Player 1 amount
            for(int i = 0; i < amount / 5; i++)
            {
                Vector2 temp;
                do
                {
                    temp = new Vector2(Random.Range(1, theTileMap.size_x / 2), Random.Range(1, theTileMap.size_z / 2));
                }
                while (goldTiles.Contains(temp) || (theTileMap.theMap.GetTileAt(temp) is Noblock));

                goldTiles.Add(temp);
            }
            for (int i = 0; i < amount / 5; i++)
            {
                Vector2 temp;
                do
                {
                    temp = new Vector2(Random.Range(1, theTileMap.size_x / 2), Random.Range(theTileMap.size_z / 2, theTileMap.size_z - 2));
                }
                while (goldTiles.Contains(temp) || (theTileMap.theMap.GetTileAt(temp) is Noblock));

                goldTiles.Add(temp);
            }
            for (int i = 0; i < amount / 5; i++)
            {
                Vector2 temp;
                do
                {
                    temp = new Vector2(Random.Range(theTileMap.size_x / 2, theTileMap.size_x - 2), Random.Range(1, theTileMap.size_z / 2));
                }
                while (goldTiles.Contains(temp) || (theTileMap.theMap.GetTileAt(temp) is Noblock));

                goldTiles.Add(temp);
            }
            for (int i = 0; i < amount / 5; i++)
            {
                Vector2 temp;
                do
                {
                    temp = new Vector2(Random.Range(theTileMap.size_x / 2, theTileMap.size_x - 2), Random.Range(theTileMap.size_z / 2, theTileMap.size_z - 2));
                }
                while (goldTiles.Contains(temp) || (theTileMap.theMap.GetTileAt(temp) is Noblock));

                goldTiles.Add(temp);
            }
            for (int i = 0; i < amount / 5; i++)
            {
                Vector2 temp;
                do
                {
                    temp = new Vector2(Random.Range(1, theTileMap.size_x - 1), Random.Range(1, theTileMap.size_z - 2));
                }
                while (goldTiles.Contains(temp) || (theTileMap.theMap.GetTileAt(temp) is Noblock));

                goldTiles.Add(temp);
            }

            return goldTiles.ToArray();
        }

        public static void SpawnRandomGoldTiles(int amount, GameObject goldPrefab, Transform goldParent)
        {
            TileMap tm = TileMapInterfacer.Instance.TileMap;
            Vector2[] goldTiles = GetRandomGoldTiles(amount);
            for(int i = 0; i < goldTiles.Length; i++)
            {
                tm.UpdateTexture(goldTiles[i], new Noblock());
                Lean.LeanPool.Spawn(goldPrefab, tm.ConvertTileToWorld(goldTiles[i]), Quaternion.identity, goldParent);
            }
        }
    }
}
