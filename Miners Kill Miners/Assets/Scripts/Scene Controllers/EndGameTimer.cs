using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Roland
{
    public class EndGameTimer : MonoBehaviour
    {
        Slider theTimer;
        TileMap tm;
        float timeCurrentTime;
        public float totaltime;
        public float waitForSeconds;
        WaitForSeconds wait;
        Queue<Vector2> TilesPos;
        // Use this for initialization
        void Start()
        {
            timeCurrentTime = 0;
            theTimer = GetComponent<Slider>();
            tm = TileMapInterfacer.Instance.TileMap;
            theTimer.maxValue = totaltime;
            TilesPos = new Queue<Vector2>();
            wait = new WaitForSeconds(waitForSeconds);
            StartCoroutine(countdown());

        }


        IEnumerator countdown()
        {
            while (timeCurrentTime < totaltime)
            {
                timeCurrentTime += Time.deltaTime;
                theTimer.value = theTimer.maxValue - timeCurrentTime;
                yield return null;
            }
            Run();
            StartCoroutine(ExplodeTiles());
        }

        IEnumerator ExplodeTiles()
        {
            do
            {
                Vector2 current = TilesPos.Dequeue();
                ObjectSpawner.SpawnObject("EndExplosion", current);
                yield return wait;
            } while (TilesPos.Count != 0);
        }

        int sizex;
        int sizez;
        void Run()
        {
            Vector2[,] VectorMatrix = new Vector2[tm.size_x, tm.size_z];
            for (int i = 0; i < VectorMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < VectorMatrix.GetLength(1); j++)
                {
                    VectorMatrix[i,j] = new Vector2(i,j);
                }
            }
            PrintTopRight(VectorMatrix, 0, 0, tm.size_z - 1, tm.size_x - 1); 
        }

        void PrintTopRight(Vector2[,] matrix, int x1, int y1, int x2, int y2)
        {
            int i = 0; int j = 0;

            for (i = x1; i <= x2; i++)
            {
                TilesPos.Enqueue(matrix[y1, i]);
            }

            // print values in the column.
            for (j = y1 + 1; j <= y2; j++)
            {
                TilesPos.Enqueue(matrix[j, x2]);
            }

            // see if more layers need to be printed.
            if (x2 - x1 > 0)
            {
                // if yes recursively call the function to 
                // print the bottom left of the sub matrix.
                PrintBottomLeft(matrix, x1, y1 + 1, x2 - 1, y2);
            }
        }

        void PrintBottomLeft(Vector2[,] matrix, int x1, int y1, int x2, int y2)
        {
            int i = 0, j = 0;

            // print the values in the row in reverse order.
            for (i = x2; i >= x1; i--)
            {
                TilesPos.Enqueue(matrix[y2, i]);
            }

            // print the values in the col in reverse order.
            for (j = y2 - 1; j >= y1; j--)
            {
                TilesPos.Enqueue(matrix[j, x1]);
            }

            // see if more layers need to be printed.
            if (x2 - x1 > 0)
            {
                // if yes recursively call the function to 
                // print the top right of the sub matrix.
                PrintTopRight(matrix, x1 + 1, y1, x2, y2 - 1);
            }
        }
    }
}
