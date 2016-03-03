using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roland
{
    public class SmallBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 0;
            AddItemToList();
        }

        public override GameObject PlayerSpawnBomb(Vector3 location)
        {
            return ObjectSpawner.SpawnObject("SmallBomb", location);
        }

        public override void UpdateInGameUI(Text theText, Image theImage)
        {
            theText.text = "∞";
            theImage.sprite = UISprite;
        }
    }
}
