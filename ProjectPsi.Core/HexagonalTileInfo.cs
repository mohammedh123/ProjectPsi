using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPsi.Core
{
    /// <summary>
    /// An implementation of ITileInfo for a map using hexagonal tiles (even-r layout).
    /// </summary>
    public struct HexagonalTileInfo : ITileInfo
    {
        private static readonly float SinPiOver3 = (float) Math.Sin(Math.PI/3);

        public float Radius { get; set; }

        public float Width
        {
            get { return Height * SinPiOver3; }
        }

        public float Height
        {
            get { return Radius * 2; }
        }

        public float GetTilePositionX(int x, int y)
        {
            var xPos = x * Width;

            if (y % 2 != 0)
            {
                xPos -= Width * 0.5f;
            }

            return xPos;
        }

        public float GetTilePositionY(int x, int y)
        {
            return y * Height * 0.75f;
        }

        public HexagonalTileInfo(float radius) : this()
        {
            Radius = radius;
        }
    }
}
