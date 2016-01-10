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
            BombName = "SmallBomb";
        }

        public override void PlayerSpawnBomb(Vector3 location)
        {
            ObjectSpawner.SpawnObject(BombName, location);
        }

        public override void UpdateInGameUI(Text theText, Image theImage)
        {
            theText.text = "∞";
            theImage.sprite = UISprite;
        }
    }
}
