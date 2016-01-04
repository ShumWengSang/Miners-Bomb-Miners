using UnityEngine;
using System.Collections;

namespace Roland
{
    public class StoneBlocks : Block
    {
        //Our stone block maximum is 10.
        const int MaximumStrength = 10;

        //We change the stone texture at 5, 7, and 9

        const int LittleLeft = 22;
        const int MediumLeft = 23;
        int HighNumberBlocks;
        public StoneBlocks()
        {
            DigsToGoThrough = 10;
            UpdateTexture(DigsToGoThrough);
            HighNumberBlocks = Random.Range(10, 13);
            
        }

        public void ChangeDigsToGoThrough(int newDigsToGoThrough)
        {
            DigsToGoThrough = newDigsToGoThrough;
            UpdateTexture(DigsToGoThrough);
        }


        public override bool Dig(int power)
        {
            DigsToGoThrough -= power;
            UpdateTexture(DigsToGoThrough);
            if (DigsToGoThrough > 0)
            {
                //not broken
                return true;
            }
            else
            {
                return false;
            }
        }

        void UpdateTexture(int DigsLeft)
        {
            if(DigsLeft >= 5)
            {
                this.texture_number = HighNumberBlocks;
                TileMapInterfacer.Instance.TileMap.UpdateTexture();
            }
            else if(DigsLeft >= 3)
            {
                this.texture_number = MediumLeft;
                TileMapInterfacer.Instance.TileMap.UpdateTexture();
            }
            else if(DigsLeft >= 0)
            {
                this.texture_number = LittleLeft;
                TileMapInterfacer.Instance.TileMap.UpdateTexture();
            }
        }
    }
}
