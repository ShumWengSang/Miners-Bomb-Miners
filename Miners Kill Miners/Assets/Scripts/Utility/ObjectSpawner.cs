using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roland
{

    public static class ObjectSpawner
    {
        // GameObject.Instantiate(Resources.Load("SmallBomb"), transform.position, Quaternion.identity);
        public static GameObject SpawnObject(string ResourceName, Vector3 position)
        {
            return Lean.LeanPool.Spawn(Resources.Load(ResourceName) as GameObject, position, Quaternion.identity) as GameObject;
        }
        public static GameObject SpawnObject(GameObject prefab, Vector3 position)
        {
            return Lean.LeanPool.Spawn(prefab as GameObject, position, Quaternion.identity) as GameObject;
        }
        public static GameObject SpawnObject(GameObject prefab, Vector2 TilePos)
        {
            return Lean.LeanPool.Spawn(prefab as GameObject, TileMapInterfacer.Instance.TileMap.ConvertTileToWorld(TilePos), Quaternion.identity) as GameObject;
        }
        public static GameObject SpawnObject(string ResourceName, Vector2 TilePos)
        {
            return Lean.LeanPool.Spawn(Resources.Load(ResourceName) as GameObject, TileMapInterfacer.Instance.TileMap.ConvertTileToWorld(TilePos), Quaternion.identity) as GameObject;
        }
    }
}