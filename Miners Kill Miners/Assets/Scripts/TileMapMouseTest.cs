using UnityEngine;
using System.Collections;

namespace Roland
{
    public class TileMapMouseTest : MonoBehaviour
    {
        TileMap _tileMap;
        Collider theCollider;
        MeshRenderer theMeshRenderer;

        // Use this for initialization
        void Start()
        {
            theMeshRenderer = GetComponent<MeshRenderer>();
            _tileMap = GetComponent<TileMap>();
            theCollider = GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if (theCollider.Raycast(ray, out hitInfo, Mathf.Infinity))
                {

                    int x = (int)((hitInfo.point.x / theMeshRenderer.bounds.size.x) * _tileMap.size_x); 
                    int y = (int)((hitInfo.point.y / theMeshRenderer.bounds.size.y) * _tileMap.size_z);
                    //Debug.Log("Tile: " + x + ", " + z);
                   // Debug.Log("Hit info  x : " + Mathf.RoundToInt(hitInfo.point.x / _tileMap.tileSize) + " y: " + Mathf.RoundToInt(hitInfo.point.y / _tileMap.tileSize));

                    _tileMap.UpdateTexture(x, y, new DirtBlock());
                }
            }
        }
    }
}