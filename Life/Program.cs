using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;
using SkiaSharp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Collections;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;

        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }

        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }

    public class BoardSettingsHelper
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }

    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public int numOfAliveCells { get { return Cells.Cast<Cell>().Where(x => x.IsAlive).Count(); } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();

        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }

        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }


    public class BoardAnalyzer
    {
        private readonly Board _board;

        public BoardAnalyzer(Board board)
        {
            _board = board;
        }

        public int[,] ExtractBoardData()
        {
            int[,] data = new int[_board.Rows, _board.Columns];

            for (int row = 0; row < _board.Rows; row++)
                for (int col = 0; col < _board.Columns; col++)
                    data[row, col] = _board.Cells[col, row].IsAlive ? 1 : 0;

            return data;
        }

        public int CountIslands()
        {
            int[,] data = ExtractBoardData();
            return CountIslandsInMatrix(data);
        }

        public static int CountIslandsInMatrix(int[,] cells)
        {
            int rows = cells.GetLength(0);
            int cols = cells.GetLength(1);
            int count = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (cells[i, j] == 1)
                    {
                        count++;
                        DFS(cells, i, j);
                    }

            return count;
        }

        public static void DFS(int[,] cells, int row, int col)
        {
            int rows = cells.GetLength(0);
            int cols = cells.GetLength(1);

            if (row < 0 || row >= rows || col < 0 || col >= cols || cells[row, col] != 1)
                return;

            cells[row, col] = 0;

            DFS(cells, row - 1, col);
            DFS(cells, row + 1, col);
            DFS(cells, row, col - 1);
            DFS(cells, row, col + 1);
            DFS(cells, row - 1, col - 1);
            DFS(cells, row - 1, col + 1);
            DFS(cells, row + 1, col - 1);
            DFS(cells, row + 1, col + 1);
        }
    }

    public class FigureFinder
    {
        public static readonly Dictionary<string, int[,]> FamousFigures = new Dictionary<string, int[,]>
        {
            { "улей", new int[,]
                {
                    { 0, 0, 0, 0, 0 },
                    { 0, 0, 1, 0, 0 },
                    { 0, 1, 0, 1, 0 },
                    { 0, 1, 0, 1, 0 },
                    { 0, 0, 1, 0, 0 },
                    { 0, 0, 0, 0, 0 }
                }
            },
            { "блок", new int[,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 0 }
                }
            },
            { "лодка", new int[,]
                {
                    { 0, 0, 0, 0, 0 },
                    { 0, 1, 1, 0, 0 },
                    { 0, 1, 0, 1, 0 },
                    { 0, 0, 1, 0, 0 },
                    { 0, 0, 0, 0, 0 }
                }
            },
            { "маячок", new int[,]
                {
                    { 0, 0, 0, 0, 0 },
                    { 0, 0, 1, 0, 0 },
                    { 0, 0, 0, 1, 0 },
                    { 0, 1, 1, 1, 0 },
                    { 0, 0, 0, 0, 0 }
                }
            },
            { "ящик", new int[,]
                {
                    { 0, 0, 0, 0, 0 },
                    { 0, 0, 1, 0, 0 },
                    { 0, 1, 0, 1, 0 },
                    { 0, 0, 1, 0, 0 },
                    { 0, 0, 0, 0, 0 }
                }
            },
            { "планер", new int[,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 0 }
                }
            }
        };

        private readonly BoardAnalyzer _analyzer;

        public FigureFinder(BoardAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public void FindAndPrintFamousFigures()
        {
            int[,] data = _analyzer.ExtractBoardData();
            foreach (var figure in FamousFigures)
            {
                int count = CountFigureOccurrences(data, figure.Value);
                Console.WriteLine($"Фигура '{figure.Key}' встречается на доске {count} раз(а).");
            }
        }

        public static int CountFigureOccurrences(int[,] boardData, int[,] figure)
        {
            int boardRows = boardData.GetLength(0);
            int boardCols = boardData.GetLength(1);
            int figureRows = figure.GetLength(0);
            int figureCols = figure.GetLength(1);
            int count = 0;

            for (int i = 0; i <= boardRows - figureRows; i++)
                for (int j = 0; j <= boardCols - figureCols; j++)
                    if (IsFigureAtPosition(boardData, figure, i, j))
                        count++;

            return count;
        }

        public static bool IsFigureAtPosition(int[,] boardData, int[,] figure, int startRow, int startCol)
        {
            int figureRows = figure.GetLength(0);
            int figureCols = figure.GetLength(1);

            for (int i = 0; i < figureRows; i++)
                for (int j = 0; j < figureCols; j++)
                    if (boardData[startRow + i, startCol + j] != figure[i, j])
                        return false;

            return true;
        }
    }
    public class BoardSerializer
    {
        private readonly Board _board;
        private readonly BoardAnalyzer _analyzer;
        private readonly string _dataDir;

        public BoardSerializer(Board board, BoardAnalyzer analyzer, string dataDir)
        {
            _board = board;
            _analyzer = analyzer;
            _dataDir = dataDir;
        }

        public void SaveCurrentState(int generation)
        {
            string fileName = "gen-" + generation;
            var data = _analyzer.ExtractBoardData();
            SaveToText(data, fileName);
            SaveToImage(data, fileName);
        }

        public void SaveToText(int[,] data, string fileName)
        {
            string path = Path.Combine(_dataDir, fileName + ".txt");
            using StreamWriter writer = new StreamWriter(path);

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    writer.Write(data[row, col]);
                    writer.Write(',');
                }
                writer.WriteLine();
            }
        }

        public void SaveToImage(int[,] data, string fileName)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            using SKBitmap bmp = new(cols * 10, rows * 10);
            using SKCanvas canvas = new(bmp);

            canvas.Clear(SKColors.Black);

            SKPaint paintAlive = new() { Color = SKColors.White, StrokeWidth = 10, IsAntialias = true };
            SKPaint paintDead = new() { Color = SKColors.Black, StrokeWidth = 10, IsAntialias = true };

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    SKPoint pt = new(col * 10, row * 10);
                    canvas.DrawPoint(pt, data[row, col] == 1 ? paintAlive : paintDead);
                }

            string path = Path.Combine(_dataDir, fileName + ".jpg");
            using SKFileWStream fs = new(path);
            bmp.Encode(fs, SKEncodedImageFormat.Jpeg, 85);
        }

        public void LoadDataFromFile(string fileName)
        {
            string path = Path.Combine(_dataDir, fileName + ".txt");

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл НЕ найден: " + path);
                return;
            }

            using StreamReader reader = new StreamReader(path);
            int row = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (values.Length != _board.Columns)
                {
                    Console.WriteLine($"Неверный формат данных. Ожидалось {_board.Columns} значений, получено {values.Length}.");
                    return;
                }

                for (int col = 0; col < values.Length; col++)
                    if (int.TryParse(values[col], out int cellValue))
                        _board.Cells[col, row].IsAlive = cellValue == 1;

                row++;
            }
        }
    }

    public class StatisticsCollector
    {
        private readonly string _dataDir;

        public StatisticsCollector(string dataDir)
        {
            _dataDir = dataDir;
        }
        public static int GenerationsToStabilize(Board b)
        {
            int generation = 0;
            int prev = b.numOfAliveCells;
            int streak = 0;

            while (true)
            {
                generation++;
                b.Advance();

                if (prev == b.numOfAliveCells)
                {
                    streak++;
                    if (streak >= 5) return generation;
                }
                else
                    streak = 0;

                if (generation > 100) return generation;
                prev = b.numOfAliveCells;
            }
        }

        public void CollectAndPlot()
        {
            const int runs = 30;
            const int windowSize = 5;

            var plt = new ScottPlot.Plot();
            List<double> xdata = new();
            List<double> ydata = new();

            for (double density = 0; density < 1; density += 0.01)
            {
                double sum = 0;
                for (int j = 0; j < runs; j++)
                {
                    var b = new Board(50, 20, 1, density);
                    sum += GenerationsToStabilize(b);
                }
                xdata.Add(density);
                ydata.Add(sum / runs);
            }

            SaveStatisticsToCSV(xdata, ydata);

            List<double> smoothed = new();
            for (int i = 0; i < ydata.Count; i++)
            {
                int from = Math.Max(0, i - windowSize / 2);
                int to = Math.Min(ydata.Count - 1, i + windowSize / 2);
                smoothed.Add(ydata.Skip(from).Take(to - from + 1).Average());
            }

            plt.Add.Scatter(xdata, smoothed);

            string imgPath = Path.Combine(_dataDir, "statistics.png");
            plt.SavePng(imgPath, 800, 600);

            OpenImage(imgPath);
        }

        private void SaveStatisticsToCSV(List<double> xdata, List<double> ydata)
        {
            string path = Path.Combine(_dataDir, "statistics.csv");
            using StreamWriter writer = new StreamWriter(path);

            writer.WriteLine("density,avg_generations");
            for (int i = 0; i < xdata.Count; i++)
                writer.WriteLine($"{xdata[i]:F2},{ydata[i]:F4}");

            Console.WriteLine($"Данные сохранены: {path}");
        }

        private static void OpenImage(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось открыть изображение: {e.Message}");
                Console.WriteLine($"Откройте вручную: {path}");
            }
        }
    }

    
    public class TimingCollector
    {
        public TimeSpan MeasureTimeToStabilize(double liveDensity)
        {
            var b = new Board(50, 20, 1, liveDensity);
            var sw = Stopwatch.StartNew();
            StatisticsCollector.GenerationsToStabilize(b);
            sw.Stop();
            return sw.Elapsed;
        }

        public string AverageTimeOfStabilization()
        {
            TimeSpan sumTime = new TimeSpan();

            for (double density = 0; density < 1; density += 0.1)
            {
                TimeSpan time = new TimeSpan();
                for (int j = 0; j < 100; j++)
                    time += MeasureTimeToStabilize(density);

                sumTime += TimeSpan.FromTicks(time.Ticks / 100);
            }

            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                sumTime.Hours, sumTime.Minutes, sumTime.Seconds, sumTime.Milliseconds / 10);
        }
    }

    class Program
    {
        static Board board;
        static string rootDir = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;
        static string dataDir = Path.Combine(rootDir, "Data");

        static private void Reset()
        {
            board = new Board(width: 50, height: 20, cellSize: 1, liveDensity: 0.5);
        }

        static private void ResetViaFile()
        {
            
            //string filePath = Path.Combine(AppContext.BaseDirectory, "settings.json");
            string filePath = Path.Combine(dataDir, "settings" + ".json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл НЕ найден: " + filePath);
                Reset();
                return;
            }

            string jsonString = File.ReadAllText(filePath);
            try
            {
                BoardSettingsHelper boardSettings = JsonSerializer.Deserialize<BoardSettingsHelper>(jsonString);
                board = new Board(
                    width: boardSettings.Width,
                    height: boardSettings.Height,
                    cellSize: boardSettings.CellSize,
                    liveDensity: boardSettings.LiveDensity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Reset();
            }
        }

        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    Console.Write(cell.IsAlive ? '*' : ' ');
                }
                Console.Write('\n');
            }
        }

        static void Main(string[] args)
        {
            ResetViaFile();
            int generation = 0;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                    // Создаём вспомогательные объекты здесь, так как board может пересоздаваться
                    var analyzer = new BoardAnalyzer(board);
                    var serializer = new BoardSerializer(board, analyzer, dataDir);
                    var figures = new FigureFinder(analyzer);
                    var stats = new StatisticsCollector(dataDir);
                    var timing = new TimingCollector();

                    if (consoleKeyInfo.KeyChar == 'q')
                    {

                        break;
                    }

                    else if (consoleKeyInfo.KeyChar == 's')
                    {
                        serializer.SaveCurrentState(generation);
                    }
                    else if (consoleKeyInfo.KeyChar == 'l')
                    {
                        Console.WriteLine("Введите имя файла для загрузки (без расширения):");
                        string fileName = Console.ReadLine();
                        serializer.LoadDataFromFile(fileName);
                    }
                    else if (consoleKeyInfo.KeyChar == 'c')
                    {
                        serializer.SaveCurrentState(generation);
                        int islandCount = analyzer.CountIslands();
                        Console.WriteLine($"Количество островов на доске: {islandCount}");
                        Thread.Sleep(3000);
                    }
                    else if (consoleKeyInfo.KeyChar == 'f')
                    {
                        figures.FindAndPrintFamousFigures();
                        Thread.Sleep(3000);
                    }
                    //по заданию непонятно какую статистику нужно собрать в задании исследовать среднее время
                    //кол-во поколений до стабилизации или физическое время, можно посмотреть среднее физическое время до стабилизации,
                    //но график строится по поколениям 
                    else if (consoleKeyInfo.KeyChar == 'p')
                    {
                        Console.WriteLine("\nИдет сбор статистики...");
                        stats.CollectAndPlot();
                    }
                    else if (consoleKeyInfo.KeyChar == 't')
                    {
                        Console.WriteLine("\nЗапускаю имитацию...");
                        string avgTime = timing.AverageTimeOfStabilization();
                        Console.WriteLine("Среднее время до стабилизации: " + avgTime);
                        Thread.Sleep(3000);
                    }
                }

                generation++;
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(10);
            }
        }
    }
}