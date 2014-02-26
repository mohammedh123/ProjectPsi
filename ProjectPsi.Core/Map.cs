using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProjectPsi.Core
{
    public class Map
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public int[,] Tiles { get; private set; }

        /// <summary>
        /// A list of unique spawn points in the map.
        /// </summary>
        public HashSet<Vector<int>> SpawnPoints { get; private set; }

        /// <summary>
        /// A list of unique end points in the map.
        /// </summary>
        public HashSet<Vector<int>> EndPoints { get; private set; } 

        public ITileInfo TileInfo { get; private set; }

        #region Edge Properties

        public int LeftEdge { get; private set; }
        public int TopEdge { get; private set; }
        public int RightEdge { get; private set; }
        public int BottomEdge { get; private set; }

        public float LeftEdgeInPixels
        {
            get { return Math.Min(TileInfo.GetTilePositionX(LeftEdge, 0), TileInfo.GetTilePositionX(LeftEdge, 1)) - TileInfo.Width/2; }
        }

        public float TopEdgeInPixels
        {
            get { return Math.Min(TileInfo.GetTilePositionY(0, TopEdge), TileInfo.GetTilePositionY(1, TopEdge)) - TileInfo.Height/2; }
        }

        public float RightEdgeInPixels
        {
            get { return Math.Max(TileInfo.GetTilePositionX(RightEdge, 0), TileInfo.GetTilePositionX(RightEdge, 1)) + TileInfo.Width/2; }
        }

        public float BottomEdgeInPixels
        {
            get { return Math.Max(TileInfo.GetTilePositionY(0, BottomEdge), TileInfo.GetTilePositionY(1, BottomEdge)) + TileInfo.Height/2; }
        }

        public float WidthInPixels
        {
            get { return TileInfo.GetTilePositionX(Width - 1, 0) + TileInfo.Width/2; }
        }

        public float HeightInPixels
        {
            get { return TileInfo.GetTilePositionY(0, Height - 1) + TileInfo.Height/2; }
        }

        #endregion

        public Map(int width, int height, ITileInfo tileInfo)
        {
            Width = width;
            Height = height;
            TileInfo = tileInfo;

            Tiles = new int[height, width];

            for (var i = 0; i < height; ++i) {
                for (var j = 0; j < width; ++j) {
                    Tiles[i, j] = -1;
                }
            }

            LeftEdge = -1;
            TopEdge = -1;
            RightEdge = -1;
            BottomEdge = -1;

            SpawnPoints = new HashSet<Vector<int>>();
            EndPoints = new HashSet<Vector<int>>();
        }

        public void SetTile(int x, int y, int spriteIndex)
        {
            Tiles[y, x] = spriteIndex;
        }

        /// <summary>
        /// Attempts to add a spawn point to the map. If the tile at the input coordinates is not empty, no spawn point will be added.
        /// </summary>
        /// <returns><b>true</b> if the spawn point is now in the list, <b>false</b> otherwise.</returns>
        public bool AddSpawnPoint(int x, int y)
        {
            if (x < Width && y < Height)
            {
                SpawnPoints.Add(new Vector<int>(x, y));
            }
            else
            {
                return false;
            }

            return true; //todo: the rest of the logic required
        }

        /// <summary>
        /// Removes any spawn point that is located at the input coordinates. Does nothing if no spawn point exists there.
        /// </summary>
        public void RemoveSpawnPoint(int x, int y)
        {
            SpawnPoints.Remove(new Vector<int>(x, y));
        }

        /// <summary>
        /// Attempts to add an end point to the map. If the tile at the input coordinates is not empty, no end point will be added.
        /// </summary>
        /// <returns><b>true</b> if the end point is now in the list, <b>false</b> otherwise.</returns>
        public bool AddEndPoint(int x, int y)
        {
            if (x < Width && y < Height)
            {
                EndPoints.Add(new Vector<int>(x, y));
            }
            else
            {
                return false;
            }

            return true; //todo: the rest of the logic required
        }

        /// <summary>
        /// Removes any end point that is located at the input coordinates. Does nothing if no end point exists there.
        /// </summary>
        public void RemoveEndPoint(int x, int y)
        {
            EndPoints.Remove(new Vector<int>(x, y));
        }

        /// <summary>
        /// Relatively expensive operation. Must be called after modifying/populating the tiles. *Edge properties will be set to the first x/y value of the first non-empty tile in that row/column.
        /// </summary>
        public void RecalculateEdges()
        {
            LeftEdge = TopEdge = RightEdge = BottomEdge = -1;
            int depth = 0;

            var foundEdges = 0;

            while (foundEdges < 4 && (depth < Width || depth < Height)) {
                for (var col = 0; col < Width && depth < Height && (TopEdge == -1 || BottomEdge == -1); ++col) {
                    if (TopEdge == -1 && Tiles[depth, col] != -1) {
                        TopEdge = depth;
                        foundEdges++;
                    }
                    if (BottomEdge == -1 && Tiles[Height - 1 - depth, col] != -1)
                    {
                        BottomEdge = Height - 1 - depth;
                        foundEdges++;
                    }
                }

                for (var row = 0; row < Height && depth < Width && (LeftEdge == -1 || RightEdge == -1); ++row) {
                    if (LeftEdge == -1 && Tiles[row, depth] != -1)
                    {
                        LeftEdge = depth;
                        foundEdges++;
                    }
                    if (RightEdge == -1 && Tiles[row, Width - 1 - depth] != -1)
                    {
                        RightEdge = Width - 1 - depth;
                        foundEdges++;
                    }
                }

                depth++;
            }
        }
    }
}