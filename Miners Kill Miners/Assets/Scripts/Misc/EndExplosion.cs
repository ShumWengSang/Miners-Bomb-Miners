using UnityEngine;
using System.Collections;
namespace Roland
{
    public class EndExplosion : MonoBehaviour
    {
        Animator ChildAnimator;
        public float time;
        public AudioClip explosion;
        WaitForSeconds wait;
        void Init()
        {
            wait = new WaitForSeconds(time);
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = explosion;
            src.loop = false;
            src.Play();
            TileMapInterfacer.Instance.TileMap.UpdateTexture(TileMapInterfacer.Instance.TileMap.ConvertWorldToTile(transform.position), new DestroyedBlock());
        }

        protected virtual void OnSpawn()
        {
            Init();
        }

        string gold = "Gold";
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(gold))
            {
                Lean.LeanPool.Despawn(collider.gameObject);
            }
        }
    }
}
