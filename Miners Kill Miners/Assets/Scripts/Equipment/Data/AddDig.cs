using UnityEngine;
using System.Collections;
namespace Roland
{
    public class AddDig : EquipmentBase
    {
        public int addDigAmount = 1;
        public override void BuyItem()
        {
            if (CurrentPlayer.Instance.BuyThings(cost))
            {
                CurrentPlayer.Instance.AddedDig += addDigAmount;
                numberOfItemText.text = CurrentPlayer.Instance.AddedDig.ToString();
            }
        }
    }
}
