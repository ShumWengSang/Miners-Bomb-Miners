using UnityEngine;
using System.Collections;
namespace Roland
{
    public class DropItems : MonoBehaviour
    {
        public float waitSeconds = 15;
        public GameObject spawnTile;
        public GameObject ObjectToDrop;
        WaitForSeconds wait;
        Vector3 location;

        GameObject currentObj;
        public GameSceneController controller;
        public PickItem weapon;


        // Use this for initialization
        void Start()
        {
            int x = TileMapInterfacer.Instance.TileMap.size_x;
            int y = TileMapInterfacer.Instance.TileMap.size_z;

            x /= 2;
            y /= 2;
            location = TileMapInterfacer.Instance.TileMap.ConvertTileToWorld(new Vector2(x, y));
            ObjectSpawner.SpawnObject(spawnTile, location);
            wait = new WaitForSeconds(waitSeconds);
            StartCoroutine(DropItemsTimer());
        }
        
        IEnumerator DropItemsTimer()
        {
            while (true)
            {
                while (controller.GameHasStarted)
                {
                    if (currentObj != null)
                    {
                        while (currentObj.activeInHierarchy)
                        {
                            yield return null;
                        }
                    }
                    yield return wait;
                    currentObj = ObjectSpawner.SpawnObject(ObjectToDrop, location);
                }
                yield return null;
            }
        }
    }
}
