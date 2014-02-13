using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPsi.Core
{
    /// <summary>
    /// An implementation of ITileInfo for a map using hexagonal tiles (even-q layout).
    /// </summary>
    public struct HexagonalTileInfo : ITileInfo
    {
        private static readonly float SinPiOver3 = (float) Math.Sin(Math.PI/3);

        public float Radius { get; set; }

        public float Width
        {
            get { return Radius*2; }
        }

        public float Height
        {
            get { return Width*SinPiOver3; }
        }

        public float GetTilePositionX(int x, int y)
        {
            return x * Width * 0.75f;
        }

        public float GetTilePositionY(int x, int y)
        {
            var yPos = y*Height;

            if (x%2 != 0) {
                yPos -= Height*0.5f;
            }

            return yPos;
        }

        public HexagonalTileInfo(float radius) : this()
        {
            Radius = radius;
        }
    }
}
