using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectPsi.Core;

namespace ProjectPsi.Tests.Core
{
    [TestClass]
    internal class TMap
    {
        protected Map DummyMap { get; set; }

        [TestClass]
        public class RecalculateEdges : TMap
        {
            [TestInitialize]
            public void Setup()
            {
                DummyMap = new Map(5, 5, new HexagonalTileInfo(10));
            }

            [TestMethod]
            public void ShouldProperlyCalculateAtMultipleDepths()
            {
                //left edge - depth 0
                DummyMap.SetTile(0, 1, 0);
                //top edge - depth 1
                DummyMap.SetTile(1, 1, 0);
                //right edge - depth 2
                DummyMap.SetTile(2, 1, 0);
                //bottom edge is implicit

                // ending grid is:
                // - - - - -
                // x x x - -
                // - - - - -
                // - - - - -
                // - - - - - 

                DummyMap.RecalculateEdges();

                Assert.AreEqual(0, DummyMap.LeftEdge);
                Assert.AreEqual(1, DummyMap.TopEdge);
                Assert.AreEqual(2, DummyMap.RightEdge);
                Assert.AreEqual(1, DummyMap.BottomEdge);
            }

            [TestMethod]
            public void ShouldProperlyCalculateAtSameDepths()
            {
                DummyMap.SetTile(1, 1, 0);
                DummyMap.SetTile(3, 1, 0);
                DummyMap.SetTile(1, 3, 0);
                DummyMap.SetTile(3, 3, 0);

                // ending grid is:
                // - - - - -
                // - x - x -
                // - - - - -
                // - x - x -
                // - - - - - 

                DummyMap.RecalculateEdges();

                Assert.AreEqual(1, DummyMap.LeftEdge);
                Assert.AreEqual(1, DummyMap.TopEdge);
                Assert.AreEqual(3, DummyMap.RightEdge);
                Assert.AreEqual(3, DummyMap.BottomEdge);
            }

            [TestMethod]
            public void ShouldProperlyCalculateWithRectangularDimensions()
            {
                DummyMap = new Map(10, 5, new HexagonalTileInfo(10));
                DummyMap.SetTile(1, 3, 0);
                DummyMap.SetTile(2, 2, 0);
                DummyMap.SetTile(5, 1, 0);
                DummyMap.SetTile(8, 3, 0);

                // ending grid is:
                // - - - - - - - - - -
                // - - - - - x - - - -
                // - - x - - - - - - -
                // - x - - - - - - x -
                // - - - - - - - - - -

                DummyMap.RecalculateEdges();

                Assert.AreEqual(1, DummyMap.LeftEdge);
                Assert.AreEqual(1, DummyMap.TopEdge);
                Assert.AreEqual(8, DummyMap.RightEdge);
                Assert.AreEqual(3, DummyMap.BottomEdge);
            }
            
            [TestMethod]
            public void ShouldHandleEmptySquareMap()
            {
                DummyMap.RecalculateEdges();

                Assert.AreEqual(-1, DummyMap.LeftEdge);
                Assert.AreEqual(-1, DummyMap.TopEdge);
                Assert.AreEqual(-1, DummyMap.RightEdge);
                Assert.AreEqual(-1, DummyMap.BottomEdge);
            }

            [TestMethod]
            public void ShouldHandleEmptyRectangularMap()
            {
                DummyMap = new Map(10, 5, new HexagonalTileInfo(10));
                DummyMap.RecalculateEdges();

                Assert.AreEqual(-1, DummyMap.LeftEdge);
                Assert.AreEqual(-1, DummyMap.TopEdge);
                Assert.AreEqual(-1, DummyMap.RightEdge);
                Assert.AreEqual(-1, DummyMap.BottomEdge);
            }
        }
    }
}
