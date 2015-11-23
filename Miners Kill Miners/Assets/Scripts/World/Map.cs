using UnityEngine;
using System.Collections;

namespace Roland
{
    public class Map 
    {
        Block[,] blocks;
        int sizex;
        int sizez;

        public Map(int x, int z)
        {
            blocks = new Block[x, z];
            sizex = x;
            sizez = z;

            //Generate map here;
            for (int i = 0; i < sizex; i++)
            {
                for(int j = 0; j < sizez; j++)
                {
                    blocks[i,j] = new DirtBlock();
                }
            }

            blocks[1, 1] = new Noblock();
            blocks[1, 2] = new Noblock();
            blocks[1, 3] = new Noblock();
            blocks[1, sizez - 2] = new Noblock();
            blocks[sizex - 2, sizez - 2] = new Noblock();
            blocks[sizex - 2, 1] = new Noblock();

            for (int i = 0; i < sizez; i++)
            {
                blocks[0, i] = new WallBlock();
                blocks[sizex - 1, i] = new WallBlock();
            }
            for (int i = 0; i < sizex; i++)
            {
                blocks[i, sizez - 1] = new WallBlock();
                blocks[i, 0] = new WallBlock();
            }
        }

        public Block GetTileAt(int x, int y)
        {
            return blocks[x, y];
        }
        public Block GetTileAt(Vector2 tile)
        {
            return blocks[(int)tile.x, (int)tile.y];
        }

        public void SetTileAt(int x , int y, Block newBlock)
        {
            blocks[x, y] = newBlock;
        }
        public void SetTileAt(Vector2 tile, Block newBlock)
        {
            blocks[(int)tile.x, (int)tile.y] = newBlock;
        }


        

    }
}