using UnityEngine;
using System.Collections;
namespace Roland
{
    public class Gold : MonoBehaviour
    {
        //18 types of gold LOL
        //We increase by 30 each time.
        public Sprite[] GoldSprites;
        public int MoneyGiven;
        public static int MoneyEscalator = 25;

        void Start()
        {
            RandomizeSprites();
            DarkRift.DarkRiftAPI.onData += ReceiveData;
        }

        public void OnDespawn()
        {
            DarkRift.DarkRiftAPI.onData -= ReceiveData;
        }

        void OnDestroy()
        {
            DarkRift.DarkRiftAPI.onData -= ReceiveData;
        }

        void RandomizeSprites()
        {
           // int random = Random.Range(0, GoldSprites.Length - 1);
            //max 18
            //50% chance of getting 2 and below
            //1 % of getting 8 and above.
            //3 to 7 is  %49
            int random = Random.Range(0, 100);
            if((0 < random) && random <= 50)
            {
                random = Random.Range(0, 2);
            }
            else if ((51 <= random) && random <= 99)
            {
                random = Random.Range(3, 7);
            }
            else if(random == 100)
            {
                random = Random.Range(8, GoldSprites.Length - 1);
            }
            MoneyGiven = random * MoneyEscalator;

            GetComponent<SpriteRenderer>().sprite = GoldSprites[random];
        }

        protected  void ReceiveData(byte tag, ushort subject, object data)
        {
            if (tag == NetworkingTags.Misc)
            {
                if (subject == NetworkingTags.MiscSubjects.GoldPickedUp)
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