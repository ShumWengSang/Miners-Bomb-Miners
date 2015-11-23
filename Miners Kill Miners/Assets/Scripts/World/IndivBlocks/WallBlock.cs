using UnityEngine;
using System.Collections;

namespace Roland
{
    public class WallBlock : Block
    {

        public WallBlock()
        {
            texture_number = 1;
            DigsToGoThrough = 9999;
        }

    }
}