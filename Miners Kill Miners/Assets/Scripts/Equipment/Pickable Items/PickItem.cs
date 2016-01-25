using UnityEngine;
using System.Collections;
namespace Roland
{
    public class PickItem : MonoBehaviour
    {
        protected virtual void PickItems()
        {

        }

        string player = "Player";
        protected void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(player))
                PickItems();
        }
    }
}
