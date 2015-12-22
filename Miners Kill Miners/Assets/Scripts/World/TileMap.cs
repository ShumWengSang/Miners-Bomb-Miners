using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Roland
{

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]

    public class TileMap : MonoBehaviour
    {
        MeshFilter theMeshFilter;
        MeshRenderer theMeshRenderer;
        MeshCollider theMeshCollider;

        public int size_x = 70;
        public int size_z = 60;
        public float tileSize = 1.0f;

        public Texture2D terrainTiles;
        public int tileResolution;
        public Material standardMaterial;

        float HalfTileX = 0;
        float HalfTileY = 0;

        Map map;

        bool b_UpdateTexture = false;

        public Map theMap
        {
            get { return map; }
        }
        
        public Vector2 ConvertWorldToTile(Vector2 point)
        {
            return new Vector2(Mathf.Floor(point.x / theMeshRenderer.bounds.size.x * size_x), Mathf.Floor(point.y / theMeshRenderer.bounds.size.y * size_z));
        }

        public Vector2 ConvertTileToWorld(Vector2 tile)
        {
            return new Vector2((tile.x * theMeshRenderer.bounds.size.x / size_x) + HalfTileX, (tile.y * theMeshRenderer.bounds.size.y / size_z) + HalfTileY);
        }
        void Awake()
        {
            TileMapInterfacer.Instance.TileMap = this;
        }

        void Start()
        {
            map = new Map(size_x, size_z);
            theMeshFilter = GetComponent<MeshFilter>();
            theMeshRenderer = GetComponent<MeshRenderer>();
            theMeshCollider = GetComponent<MeshCollider>();
            BuildMesh();
        }

        public void TileMapReset()
        {
            theMap.GenerateMap(size_x, size_z);
            BuildMesh();
        }

        public void UpdateTexture(int x, int y, Block newBlock)
        {

            map.SetTileAt(x, y, newBlock);
            BuildTexture();
        }

        Color[][] ChopUpTiles()
        {
            int numTilesPerRow = terrainTiles.width / tileResolution;
            int numRows = terrainTiles.height / tileResolution;

            Color[][] tiles = new Color[numTilesPerRow * numRows][];

            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < numTilesPerRow; x++)
                {
                    tiles[y * numTilesPerRow + x] = terrainTiles.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
                }
            }

            return tiles;
        }

        void BuildTexture()
        {

            int texWidth = size_x * tileResolution;
            int texHeight = size_z * tileResolution;
            Texture2D texture = new Texture2D(texWidth, texHeight);

            Color[][] tiles = ChopUpTiles();

            for (int y = 0; y < size_z; y++)
            {
                for (int x = 0; x < size_x; x++)
                {
                    Color[] p = tiles[map.GetTileAt(x, y).texture_number];
                    texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, p);
                }
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();

            if (theMeshRenderer.sharedMaterial == null)
            {
                theMeshRenderer.sharedMaterial = standardMaterial;
            }
            theMeshRenderer.sharedMaterial.mainTexture = texture;
        }

        public void BuildMesh()
        {
            //Generate Mesh data


            //Find the number of triangles.
            int numTiles = size_x * size_z;
            int numTris = numTiles * 2;

            //Find the number of vertices.
            int vsize_x = size_x + 1;
            int vsize_z = size_z + 1;
            int numVerts = vsize_x * vsize_z;

            Vector3[] vertices = new Vector3[numVerts];
            Vector3[] normals = new Vector3[numVerts];
            Vector2[] uv = new Vector2[numVerts];
            int[] triangles = new int[numTris * 3];

            int x, z;
            for (z = 0; z < vsize_z; z++)
            {
                for (x = 0; x < vsize_x; x++)
                {
                    vertices[z * vsize_x + x] = new Vector3(x * tileSize, -z * tileSize, 0);
                    normals[z * vsize_x + x] = Vector3.back;
                    uv[z * vsize_x + x] = new Vector2((float)x / size_x, 1f - (float)z / size_z);
                }
            }
            Debug.Log("Done Verts!");

            for (z = 0; z < size_z; z++)
            {
                for (x = 0; x < size_x; x++)
                {
                    int squareIndex = z * size_x + x;
                    int triOffset = squareIndex * 6;
                    triangles[triOffset + 0] = z * vsize_x + x + 0;
                    triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 0;
                    triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 1;

                    triangles[triOffset + 3] = z * vsize_x + x + 0;
                    triangles[triOffset + 5] = z * vsize_x + x + vsize_x + 1;
                    triangles[triOffset + 4] = z * vsize_x + x + 1;
                }
            }

            Debug.Log("Done Triangles!");

            // Create a new Mesh and populate with the data
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;

            theMeshFilter.mesh = mesh;

            BuildTexture();

            theMeshCollider.sharedMesh = mesh;

            transform.localPosition = new Vector3(0, theMeshCollider.bounds.size.y + transform.localPosition.y, 0);
            Camera.main.transform.localPosition = new Vector3(theMeshRenderer.bounds.center.x, theMeshRenderer.bounds.center.y, -10);

            HalfTileX = theMeshRenderer.bounds.size.x / size_x / 2;
            HalfTileY = theMeshRenderer.bounds.size.y / size_z / 2;

        }

        public void DigTile(Vector2 tile, int power, string ResourceName = null)
        {
            if(!map.GetTileAt(tile).Dig(power))
            {
                //Dug through the tile
                if(ResourceName != null && ResourceName != string.Empty)
                {
                    ObjectSpawner.SpawnObject(ResourceName, tile);
                }
                map.SetTileAt(tile, new Noblock());
                b_UpdateTexture = true;
            } 
        }

        public void DigTile(int x, int y, int power, string ResourceName = null)
        {
            DigTile(new Vector2(x, y), power, ResourceName);
        }

        void LateUpdate()
        {
            if(b_UpdateTexture)
            {
                BuildTexture();
                b_UpdateTexture = false;
            }
        }
    }
}