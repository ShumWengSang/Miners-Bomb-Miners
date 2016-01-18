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

        const int DLittleLeft = 6;
        const int DMediumLeft = 5;
        int HighNumberBlocks;

        bool firstTime = true;
        public StoneBlocks()
        {
            HighNumberBlocks = Random.Range(10, 13);
            
        }

        public void ChangeDigsToGoThrough(int newDigsToGoThrough)
        {
            DigsToGoThrough = newDigsToGoThrough;
            UpdateTexture(DigsToGoThrough);
            firstTime = false;
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
            else if(DigsLeft >= 4)
            {
                if (firstTime)
                    this.texture_number = MediumLeft;
                else
                    this.texture_number = DMediumLeft;
                TileMapInterfacer.Instance.TileMap.UpdateTexture();
            }
            else if(DigsLeft >= 0)
            {
                DigsLeft = 3;
                if (firstTime)
                    this.texture_number = LittleLeft;
                else
                    this.texture_number = DLittleLeft;
                TileMapInterfacer.Instance.TileMap.UpdateTexture();
            }
        }
    }
}
