using UnityEngine;
using System.Collections;

namespace Roland
{
    public class TileMapInterfacer : Singleton<TileMapInterfacer>
    {
        private TileMap theTileMap;
        protected TileMapInterfacer() { }

        public TileMap TileMap
        {
            get { return theTileMap; }
            set
            {
                if (value is TileMap)
                {
                    theTileMap = value;
                }
            }
        }
    }
}