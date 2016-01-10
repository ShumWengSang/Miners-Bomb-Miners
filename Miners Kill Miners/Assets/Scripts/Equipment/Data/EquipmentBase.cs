using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roland
{
    public class EquipmentBase : MonoBehaviour
    {
        protected bool Upgrade = false;
        protected int amount = 0;
        public int cost = 0;

        protected Text CostOfItem;
        protected Text numberOfItemText;

        public Sprite UISprite;

        public string BombName;

        public int OrderID
        {
            get { return id; }
            set { id = value;  }
        }

        protected int id;

        public int Amount
        {
            get { return amount; }
        }

        protected void AddItemToList()
        {
            if (!Upgrade)
                CurrentPlayer.Instance.AmountOfEquipments.Add(this);
        }
        public virtual void Init()
        {
            GetComponent<Image>().sprite = UISprite;

            numberOfItemText = transform.FindChild("Item Number").GetComponent<Text>();
            CostOfItem = transform.FindChild("Price").GetComponent<Text>();

            CostOfItem.text = "$" + cost.ToString();
            numberOfItemText.text = 0.ToString();
        }
        public virtual void BuyItem()
        {
            if (CurrentPlayer.Instance.BuyThings(cost))
            {
                if (!Upgrade)
                {
                    amount++;
                    numberOfItemText.text = amount.ToString();
                }
            }
        }
        protected bool MinusBomb()
        {
            bool returner = false;
            if(amount > 0)
            {
                returner = true;
            }
            amount--;
            return returner;
        }
        public virtual void PlayerSpawnBomb(Vector3 location)
        {
            if (MinusBomb())
                ObjectSpawner.SpawnObject(BombName, location);
        }

        public virtual void DummySpawnBomb(Vector3 location)
        {
             ObjectSpawner.SpawnObject(BombName, location);
        }

        public virtual void UpdateInGameUI(Text theText, Image theImage)
        {
            theText.text = amount.ToString();
            theImage.sprite = UISprite;
        }
    }
}
