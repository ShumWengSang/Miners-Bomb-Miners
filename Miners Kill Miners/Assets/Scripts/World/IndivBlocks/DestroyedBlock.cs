using UnityEngine;
using System.Collections;
namespace Roland
{
    public class DestroyedBlock : Block
    {
        public DestroyedBlock()
        {
            //20
            texture_number = 7;
            DigsToGoThrough = 9999;
        }
    }
}
