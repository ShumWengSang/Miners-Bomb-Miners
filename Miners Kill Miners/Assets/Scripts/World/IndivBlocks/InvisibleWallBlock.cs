using UnityEngine;
using System.Collections;

namespace Roland
{
    public class InvisibleWallBlock : Block
    {
        public InvisibleWallBlock()
        {
            texture_number = 16;
            DigsToGoThrough = 9999;
        }
    }
}