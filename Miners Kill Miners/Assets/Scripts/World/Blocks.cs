using UnityEngine;
using System.Collections;

namespace Roland
{
    public class Block
    {
        public int texture_number;
        public int DigsToGoThrough;
        public bool Dig(int power)
        {
            if (DigsToGoThrough == 9999)
            {
                return true;
            }
            DigsToGoThrough -= power;
            if(DigsToGoThrough > 0)
            {
                //not broken
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}