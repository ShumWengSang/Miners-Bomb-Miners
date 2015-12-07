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
            return GameObject.Instantiate(Resources.Load(ResourceName), position, Quaternion.identity) as GameObject;
        }
        public static GameObject SpawnObject(string ResourceName, Vector2 TilePos)
        {
            return GameObject.Instantiate(Resources.Load(ResourceName), TileMapInterfacer.Instance.TileMap.ConvertTileToWorld(TilePos), Quaternion.identity) as GameObject;
        }
    }
}