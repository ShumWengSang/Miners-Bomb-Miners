using UnityEngine;
using System.Collections;
namespace Roland
{
    public class HideFog : MonoBehaviour
    {
        string Fog = "Fog";
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(Fog))
            {
                collider.GetComponent<MeshRenderer>().enabled = false;
                //Lean.LeanPool.Despawn(collider.gameObject);
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if(collider.CompareTag(Fog))
            {
                collider.GetComponent<MeshRenderer>().enabled = true;
                //Lean.LeanPool.Despawn(collider.gameObject);
            }
        }
    }
}
