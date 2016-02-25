﻿using UnityEngine;
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
            {
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Misc, NetworkingTags.MiscSubjects.It, collider.transform.position);
                PickItems();
            }
        }

        protected void ReceiveData(byte tag, ushort subject, object data)
        {
            
            if (tag == NetworkingTags.Misc)
            {
                if (subject == NetworkingTags.MiscSubjects.ItemPickUp)
                {
                    Vector3 theDataPos = (Vector3)data;
                    Vector3 pos = transform.position;
                    if (theDataPos == pos)
                    {
                        Lean.LeanPool.Despawn(this.gameObject);
                    }
                }
            }
        }
    }
}
