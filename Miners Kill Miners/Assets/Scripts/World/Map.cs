using UnityEngine;
using System.Collections;

namespace Roland
{
    public class Map 
    {
        public static float Offset = 0f;
        public static float Threshold = 0.5f;
        Block[,] blocks;
        int sizex;
        int sizez;

        public static void MapChangeLevel()
        {
            Offset += 2.0f;
            Threshold -= 0.05f;
            if(Threshold > 0.3f)
            {
                Threshold = 0.3f;
            }
        }
        public Map(int x, int z)
        {
            GenerateMap(x, z);
        }

        public void GenerateMap(int x , int z)
        {
            blocks = new Block[x, z];
            sizex = x;
            sizez = z;

            //Generate map here;
            //anything below 0.5 is dirt.
            //
            for (int i = 0; i < sizex; i++)
            {
                for (int j = 0; j < sizez; j++)
                {
                    float number1 = (float)i / 10f + Offset;
                    float number2 = (float)j / 10f + Offset;
                    float number = Mathf.PerlinNoise(number1, number2);
                    if (number <= Threshold)
                    {
                        //dirt blocks
                        blocks[i, j] = new DirtBlock();
                    }
                    else
                    {
                        float percentage = (number - Threshold)/ (Threshold);
                        //This is a stone

                        float strength = percentage ;
                        StoneBlocks stone = new StoneBlocks();
                        stone.ChangeDigsToGoThrough((int)(strength * 10));
                        blocks[i, j] = stone;
                    }
                }
            }

            blocks[1, 1] = new Noblock();
            blocks[1, 2] = new Noblock();
            blocks[2, 1] = new Noblock();

            blocks[1, sizez - 2] = new Noblock();
            blocks[1, sizez - 3] = new Noblock();
            blocks[2, sizez - 2] = new Noblock();

            blocks[sizex - 2, 1] = new Noblock();
            blocks[sizex - 3, 1] = new Noblock();
            blocks[sizex - 2, 2] = new Noblock();

            blocks[sizex - 2, sizez - 2] = new Noblock();
            blocks[sizex - 3, sizez - 2] = new Noblock();
            blocks[sizex - 2, sizez - 3] = new Noblock();



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
            CheckBoundaries(ref x, ref y);
            return blocks[x, y];
        }
        public Block GetTileAt(Vector2 tile)
        {
            int x = (int)tile.x;
            int y = (int)tile.y;
            CheckBoundaries(ref x, ref y);
            return blocks[x, y];
        }

        void CheckBoundaries(ref int x, ref int y)
        {
            if (x >= sizex)
                x = sizex - 1;
            if (x < 0)
                x = 0;
            if (y >= sizez)
                y = sizez - 1;
            if (y < 0)
                y = 0;
        }

        public bool CheckBoundaries(int x, int y)
        {
            bool correct = true;
            if (x >= sizex)
                correct = false;
            if (x < 0)
                correct = false;
            if (y >= sizez)
                correct = false;
            if (y < 0)
                correct = false;

            return correct;
        }

        public Vector2 SetTileAt(int x , int y, Block newBlock)
        {
            if (x == -1 && y == -1)
                return Vector2.zero;
            CheckBoundaries(ref x, ref y);
            blocks[x, y] = newBlock;
            return new Vector2(x, y);
        }
        public Vector2 SetTileAt(Vector2 tile, Block newBlock)
        {
            int x = (int)tile.x;
            int y = (int)tile.y;
            if (x == -1 && y == -1)
                return Vector2.zero;
            CheckBoundaries(ref x, ref y);
            blocks[x, y] = newBlock;
            return new Vector2(x, y);
        }
    }
}