using System;

namespace ProjectPsi.Core
{
    public class Map
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int[,] Tiles { get; set; }

        public ITileInfo TileInfo { get; set; }

        public int LeftEdge { get; set; }
        public int TopEdge { get; set; }
        public int RightEdge { get; set; }
        public int BottomEdge { get; set; }

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
        }

        public void SetTile(int x, int y, int spriteIndex)
        {
            Tiles[y, x] = spriteIndex;
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