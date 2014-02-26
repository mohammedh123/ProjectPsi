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

        [TestClass]
        public class AddSpawnPoint : TMap
        {
            [TestInitialize]
            public void Setup()
            {
                DummyMap = new Map(5, 5, new HexagonalTileInfo(10));
            }

            [TestMethod]
            public void ShouldProperlyAddNewSpawnPointSingle()
            {
                DummyMap.AddSpawnPoint(0, 0);

                Assert.IsTrue(DummyMap.SpawnPoints.Contains(new Vector<int>(0, 0)));
            }

            [TestMethod]
            public void ShouldProperlyAddNewSpawnPointMultiple()
            {
                DummyMap.AddSpawnPoint(0, 0);
                DummyMap.AddSpawnPoint(0, 0);
                DummyMap.AddSpawnPoint(0, 0);
                DummyMap.AddSpawnPoint(0, 0);
                DummyMap.AddSpawnPoint(0, 0);
                DummyMap.AddSpawnPoint(0, 0);
                DummyMap.AddSpawnPoint(0, 0);

                Assert.IsTrue(DummyMap.SpawnPoints.Contains(new Vector<int>(0, 0)));
                Assert.IsTrue(DummyMap.SpawnPoints.Count == 1);
            }

            [TestMethod]
            public void ShouldFailForOutOfBoundsSpawnPoint()
            {
                var retVal = DummyMap.AddSpawnPoint(999, 999);

                Assert.IsFalse(retVal);
                Assert.AreEqual(0, DummyMap.SpawnPoints.Count);
            }
        }

        [TestClass]
        public class RemoveSpawnPoint : TMap
        {
            [TestInitialize]
            public void Setup()
            {
                DummyMap = new Map(5, 5, new HexagonalTileInfo(10));
                DummyMap.AddSpawnPoint(0, 0);
            }

            [TestMethod]
            public void ShouldProperlyRemoveSpawnPointSingle()
            {
                DummyMap.RemoveSpawnPoint(0,0);

                Assert.AreEqual(0, DummyMap.SpawnPoints.Count);
            }

            [TestMethod]
            public void ShouldProperlyRemoveSpawnPointMultiple()
            {
                DummyMap.RemoveSpawnPoint(0, 0);
                DummyMap.RemoveSpawnPoint(0, 0);
                DummyMap.RemoveSpawnPoint(0, 0);
                DummyMap.RemoveSpawnPoint(0, 0);
                DummyMap.RemoveSpawnPoint(0, 0);
                DummyMap.RemoveSpawnPoint(0, 0);

                Assert.AreEqual(0, DummyMap.SpawnPoints.Count);
            }

            [TestMethod]
            public void ShouldNotRemoveSpawnPointThatDoesNotExist()
            {
                DummyMap.RemoveSpawnPoint(5, 5);
                DummyMap.RemoveSpawnPoint(555, 555);

                Assert.AreEqual(1, DummyMap.SpawnPoints.Count);
            }
        }

        [TestClass]
        public class AddEndPoint : TMap
        {
            [TestInitialize]
            public void Setup()
            {
                DummyMap = new Map(5, 5, new HexagonalTileInfo(10));
            }

            [TestMethod]
            public void ShouldProperlyAddNewEndPointSingle()
            {
                DummyMap.AddEndPoint(0, 0);

                Assert.IsTrue(DummyMap.EndPoints.Contains(new Vector<int>(0, 0)));
            }

            [TestMethod]
            public void ShouldProperlyAddNewEndPointMultiple()
            {
                DummyMap.AddEndPoint(0, 0);
                DummyMap.AddEndPoint(0, 0);
                DummyMap.AddEndPoint(0, 0);
                DummyMap.AddEndPoint(0, 0);
                DummyMap.AddEndPoint(0, 0);
                DummyMap.AddEndPoint(0, 0);
                DummyMap.AddEndPoint(0, 0);

                Assert.IsTrue(DummyMap.EndPoints.Contains(new Vector<int>(0, 0)));
                Assert.IsTrue(DummyMap.EndPoints.Count == 1);
            }

            [TestMethod]
            public void ShouldFailForOutOfBoundsEndPoint()
            {
                var retVal = DummyMap.AddEndPoint(999, 999);

                Assert.IsFalse(retVal);
                Assert.AreEqual(0, DummyMap.EndPoints.Count);
            }
        }

        [TestClass]
        public class RemoveEndPoint : TMap
        {
            [TestInitialize]
            public void Setup()
            {
                DummyMap = new Map(5, 5, new HexagonalTileInfo(10));
                DummyMap.AddEndPoint(0, 0);
            }

            [TestMethod]
            public void ShouldProperlyRemoveEndPointSingle()
            {
                DummyMap.RemoveEndPoint(0, 0);

                Assert.AreEqual(0, DummyMap.EndPoints.Count);
            }

            [TestMethod]
            public void ShouldProperlyRemoveEndPointMultiple()
            {
                DummyMap.RemoveEndPoint(0, 0);
                DummyMap.RemoveEndPoint(0, 0);
                DummyMap.RemoveEndPoint(0, 0);
                DummyMap.RemoveEndPoint(0, 0);
                DummyMap.RemoveEndPoint(0, 0);
                DummyMap.RemoveEndPoint(0, 0);

                Assert.AreEqual(0, DummyMap.EndPoints.Count);
            }

            [TestMethod]
            public void ShouldNotRemoveEndPointThatDoesNotExist()
            {
                DummyMap.RemoveEndPoint(5, 5);
                DummyMap.RemoveEndPoint(555, 555);

                Assert.AreEqual(1, DummyMap.EndPoints.Count);
            }
        }
    }
}
