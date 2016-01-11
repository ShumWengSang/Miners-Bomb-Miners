using UnityEngine;
using System.Collections;
namespace Roland
{
    public class GrenadeData : EquipmentBase
    {
        Direction direction;
        public void SetDirection(Direction theDir)
        {
            direction = theDir;
        }

        void Awake()
        {
            OrderID = 9;
            AddItemToList();
        }

        public override GameObject PlayerSpawnBomb(Vector3 location)
        {
            GameObject obj = null;
            if (MinusBomb())
            {
                obj = ObjectSpawner.SpawnObject(ObjectToSpawn, location);
                obj.GetComponent<GrenadeBomb>().Direction = direction;
            }
            return obj;

        }

        public override GameObject DummySpawnBomb(Vector3 location)
        {
            GameObject obj = null;
            obj = ObjectSpawner.SpawnObject(ObjectToSpawn, location);
            obj.GetComponent<GrenadeBomb>().Direction = direction;
            return obj;
        }

    }
}
