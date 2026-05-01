using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class CellTests
    {

        [TestMethod]
        public void CellLivesWith2NeighborsStaysAlive()
        {
            var cell = new Cell { IsAlive = true };
            for (int i = 0; i < 2; i++)
                cell.neighbors.Add(new Cell { IsAlive = true });
            for (int i = 0; i < 6; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsTrue(cell.IsAlive);
        }

        [TestMethod]
        public void CellLivesWith3NeighborsStaysAlive()
        {
            var cell = new Cell { IsAlive = true };
            for (int i = 0; i < 3; i++)
                cell.neighbors.Add(new Cell { IsAlive = true });
            for (int i = 0; i < 5; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsTrue(cell.IsAlive);
        }


        [TestMethod]
        public void CellLivesWith1NeighborDies()
        {
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            for (int i = 0; i < 7; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void CellLivesWith4NeighborsDies()
        {
            var cell = new Cell { IsAlive = true };
            for (int i = 0; i < 4; i++)
                cell.neighbors.Add(new Cell { IsAlive = true });
            for (int i = 0; i < 4; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void CellDeadWith3NeighborsBecomesAlive()
        {
            var cell = new Cell { IsAlive = false };
            for (int i = 0; i < 3; i++)
                cell.neighbors.Add(new Cell { IsAlive = true });
            for (int i = 0; i < 5; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsTrue(cell.IsAlive);
        }


        [TestMethod]
        public void CellDeadWith2NeighborsStaysDead()
        {
            var cell = new Cell { IsAlive = false };
            for (int i = 0; i < 2; i++)
                cell.neighbors.Add(new Cell { IsAlive = true });
            for (int i = 0; i < 6; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void CellLivesWithNoNeighborsDies()
        {
            var cell = new Cell { IsAlive = true };
            for (int i = 0; i < 8; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void CellDeadWithNoNeighborsStaysDead()
        {
            var cell = new Cell { IsAlive = false };
            for (int i = 0; i < 8; i++)
                cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }
    }

    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void BoardCreatedCorrectColumns()
        {
            var board = new Board(50, 20, 1, 0);
            Assert.AreEqual(50, board.Columns);
        }
        [TestMethod]
        public void BoardCreatedCorrectRows()
        {
            var board = new Board(50, 20, 1, 0);
            Assert.AreEqual(20, board.Rows);
        }

        [TestMethod]
        public void BoardZeroDensityAllCellsDead()
        {
            var board = new Board(50, 20, 1, 0);
            Assert.AreEqual(0, board.numOfAliveCells);
        }

        [TestMethod]
        public void BoardMaxDensityAllCellsAlive()
        {
            var board = new Board(50, 20, 1, 1);
            Assert.AreEqual(50 * 20, board.numOfAliveCells);
        }

        [TestMethod]
        public void BoardAliveCellsNeverExceedsTotalCells()
        {
            var board = new Board(50, 20, 1, 0.5);
            Assert.IsTrue(board.numOfAliveCells <= board.Columns * board.Rows);
        }
        [TestMethod]
        public void BoardCellSize2HalvesColumnsAndRows()
        {
            var board = new Board(50, 20, 2, 0);
            Assert.AreEqual(25, board.Columns);
            Assert.AreEqual(10, board.Rows);
        }

        [TestMethod]
        public void BoardFullBoardAfterAdvanceCellsDie()
        {
            var board = new Board(10, 10, 1, 1);
            board.Advance();
            Assert.AreEqual(0, board.numOfAliveCells);
        }

        [TestMethod]
        public void BoardEachCellHas8Neighbors()
        {
            var board = new Board(10, 10, 1, 0);
            foreach (var cell in board.Cells)
                Assert.AreEqual(8, cell.neighbors.Count);
        }
    }

    [TestClass]
    public class BoardAnalyzerTests
    {
        [TestMethod]
        public void BoardAnalyzerExtractDataAliveCellIsOne()
        {
            var board = new Board(4, 4, 1, 0);
            board.Cells[0, 0].IsAlive = true;

            var analyzer = new BoardAnalyzer(board);
            var data = analyzer.ExtractBoardData();

            Assert.AreEqual(1, data[0, 0]);
        }

        [TestMethod]
        public void BoardAnalyzer_ExtractData_DeadCellIsZero()
        {
            var board = new Board(4, 4, 1, 0);
            var analyzer = new BoardAnalyzer(board);
            var data = analyzer.ExtractBoardData();

            Assert.AreEqual(0, data[0, 0]);
        }
        [TestMethod]
        public void BoardAnalyzerEmptyBoardZeroIslands()
        {
            int[,] matrix = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            Assert.AreEqual(0, BoardAnalyzer.CountIslandsInMatrix(matrix));
        }

        [TestMethod]
        public void BoardAnalyzerThreeIsolatedCellsThreeIslands()
        {
            int[,] matrix =
            {
                { 1, 0, 1 },
                { 0, 0, 0 },
                { 1, 0, 0 }
            };
            Assert.AreEqual(3, BoardAnalyzer.CountIslandsInMatrix(matrix));
        }
        [TestMethod]
        public void BoardAnalyzerDiagonalCellsOneIsland()
        {
            int[,] matrix =
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            };
            Assert.AreEqual(1, BoardAnalyzer.CountIslandsInMatrix(matrix));
        }
        [TestMethod]
        public void BoardAnalyzerFullMatrixOneIsland()
        {
            int[,] matrix =
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };
            Assert.AreEqual(1, BoardAnalyzer.CountIslandsInMatrix(matrix));
        }
    }

    [TestClass]
    public class FigureFinderTests
    {
        [TestMethod]
        public void FigureFinderBlockFoundOnce()
        {
            int[,] board =
            {
                { 0, 0, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 }
            };
            var block = FigureFinder.FamousFigures["блок"];
            Assert.AreEqual(1, FigureFinder.CountFigureOccurrences(board, block));
        }

        [TestMethod]
        public void FigureFinderBlockNotFoundOnEmptyBoard()
        {
            int[,] board =
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            var block = FigureFinder.FamousFigures["блок"];
            Assert.AreEqual(0, FigureFinder.CountFigureOccurrences(board, block));
        }

        [TestMethod]
        public void FigureFinderTwoBlocksFoundTwice()
        {
            // два блока рядом, но не перекрываются
            int[,] board =
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 0, 0, 1, 1, 0, 0 },
                { 0, 1, 1, 0, 0, 1, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            var block = FigureFinder.FamousFigures["блок"];
            Assert.AreEqual(2, FigureFinder.CountFigureOccurrences(board, block));
        }

        [TestMethod]
        public void FigureFinderIsFigureAtPositionMatchReturnsTrue()
        {
            int[,] board =
            {
                { 0, 0, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 }
            };
            var block = FigureFinder.FamousFigures["блок"];
            Assert.IsTrue(FigureFinder.IsFigureAtPosition(board, block, 0, 0));
        }

        [TestMethod]
        public void FigureFinderIsFigureAtPositionMismatchReturnsFalse()
        {
            int[,] board =
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            var block = FigureFinder.FamousFigures["блок"];
            Assert.IsFalse(FigureFinder.IsFigureAtPosition(board, block, 0, 0));
        }

        [TestMethod]
        public void FigureFinderFamousFiguresContainsAllExpectedKeys()
        {
            var expected = new[] { "улей", "блок", "лодка", "маячок", "ящик", "планер" };
            foreach (var key in expected)
                Assert.IsTrue(FigureFinder.FamousFigures.ContainsKey(key), $"Фигура '{key}' отсутствует в словаре");
        }
    }

    [TestClass]
    public class StatisticsCollectorTests
    {
        [TestMethod]
        public void StatisticsCollector_EmptyBoard_StabilizesQuickly()
        {
            var board = new Board(50, 20, 1, 0);
            int generations = StatisticsCollector.GenerationsToStabilize(board);
            Assert.IsTrue(generations <= 10);
        }
        [TestMethod]
        public void StatisticsCollectorAnyBoardReturnsPositiveGenerations()
        {
            var board = new Board(50, 20, 1, 0.5);
            int generations = StatisticsCollector.GenerationsToStabilize(board);
            Assert.IsTrue(generations > 0);
        }
    }
}