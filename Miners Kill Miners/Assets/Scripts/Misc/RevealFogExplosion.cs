using UnityEngine;
using System.Collections;
namespace Roland
{
    public class RevealFogExplosion : MonoBehaviour
    {
        string explosion = "Explosion";
        string EndExplosion = "EndExplosion";

        public static bool Trigger = false;
        public float Wait;
        public LayerMask playerMask;
        MeshRenderer mr;
        WaitForSeconds wait;
        GameObject thePlayer;
        public CircleCollider2D PlayerCollider;
        void Start()
        {
            wait = new WaitForSeconds(Wait);
            mr = GetComponent<MeshRenderer>();
        }
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (Trigger)
            {
                if (collider.CompareTag(explosion) || collider.CompareTag(EndExplosion))
                {
                    Explosion exp = collider.GetComponentInParent<Explosion>();
                    if (exp != null)
                    {
                        if (exp.ID == DarkRift.DarkRiftAPI.id)
                            StartCoroutine(RevealFog());
                    }
                    else
                    {
                        mr.enabled = false;
                    }
                }
            }
        }

        IEnumerator RevealFog()
        {
            thePlayer = CurrentPlayer.Instance.ThePlayer.gameObject;
            float currentTime = Time.time;
            float MaxTime = currentTime + Wait;
            while (currentTime < MaxTime)
            {
                currentTime += Time.deltaTime;
                mr.enabled = false;
                yield return null;
            }
            while (Physics2D.OverlapCircle(transform.position, PlayerCollider.radius, playerMask))
            {
                yield return null;
            }
            mr.enabled = true;
        }


    }
}
