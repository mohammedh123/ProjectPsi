using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPsi.Core
{
    /// <summary>
    /// An interface for a class that represents the type of tiles used in a map.
    /// </summary>
    public interface ITileInfo
    {
        /// <summary>
        /// The radius of the tile.
        /// </summary>
        float Radius { get; set; }

        /// <summary>
        /// The width of the tile. Should be a function of the Radius.
        /// </summary>
        float Width { get; }

        /// <summary>
        /// The height of the tile. Should be a function of the Radius.
        /// </summary>
        float Height { get; }

        /// <summary>
        /// Returns the x-component of the tile located at [col, row].
        /// </summary>
        float GetTilePositionX(int x, int y);

        /// <summary>
        /// Returns the y-component of the tile located at [col, row].
        /// </summary>
        float GetTilePositionY(int x, int y);
    }
}
