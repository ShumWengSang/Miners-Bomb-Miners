using UnityEngine;
using System.Collections;

namespace Roland
{
    public class InvisibleWallBlock : Block
    {
        public InvisibleWallBlock()
        {
            //20
            texture_number = 20;
            DigsToGoThrough = 9999;
        }
    }
}