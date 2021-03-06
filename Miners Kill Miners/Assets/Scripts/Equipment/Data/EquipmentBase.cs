﻿using UnityEngine;
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
        protected Text InGameUINumberOfItem;

        public Sprite UISprite;

        public string BombName;

        public GameObject ObjectToSpawn;

        public string ItemDescription;
        public Transform SetDescTo;

        public int OrderID
        {
            get { return id; }
            set { id = value;  }
        }

        protected int id;

        public int Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                if (numberOfItemText == null)
                {
                    numberOfItemText = transform.FindChild("Item Number").GetComponent<Text>();
                }
                numberOfItemText.text = amount.ToString();
            }
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
            numberOfItemText.text = amount.ToString();
        }
        public virtual void BuyItem()
        {
            if (CurrentPlayer.Instance.BuyThings(cost))
            {
                if (!Upgrade)
                {
                    amount++;
                    //numberOfItemText.text = amount.ToString();
                    UIShowDescr.instance.UpdateText();
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
            if (amount < 0)
                amount = 0;
            return returner;
        }
        public virtual GameObject PlayerSpawnBomb(Vector3 location)
        {
            if (MinusBomb())
                return ObjectSpawner.SpawnObject(ObjectToSpawn, location);
            return null;
                
        }

        public virtual GameObject DummySpawnBomb(Vector3 location)
        {
            return ObjectSpawner.SpawnObject(ObjectToSpawn, location);
        }

        public virtual void UpdateInGameUI(Text theText, Image theImage)
        {
            theText.text = amount.ToString();
            theImage.sprite = UISprite;
        }
    }
}
