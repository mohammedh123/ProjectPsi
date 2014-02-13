namespace ProjectPsi.Core
{
    public class Map
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int[,] Tiles { get; set; }

        public ITileInfo TileInfo { get; set; }


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
        }

        public void SetTile(int x, int y, int spriteIndex)
        {
            Tiles[y, x] = spriteIndex;
        }
    }
}
