
using System.Diagnostics;
using GameOfLifeLibrary;

public class Program
{
    public static void Main()
    {
        //SpaceProgressingConsoleOutputTest(); 
        ParallelizationPerformanceTest();

        static void OutputToConsole(GameOfLife gameOfLife, bool clearScreen = true)
        {
            bool[,] grid = gameOfLife.GetGrid();

            string outputString = "";

            outputString += "\n";
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y]) outputString += "X";
                    else outputString += "O";
                }
                outputString += "\n";
            }
            outputString += "\n\n";

            if (clearScreen)
                Console.Clear();

            Console.Write(outputString);
        }

        static void ParallelizationPerformanceTest()
        {
            int cycle = 0;
            int targetCycles = 10000;

            Stopwatch stopwatch = new();

            GameOfLife gameOfLife = new(100, 100);

            gameOfLife.PopulateCell(2, 2);
            gameOfLife.PopulateCell(3, 2);
            gameOfLife.PopulateCell(1, 2);
            gameOfLife.PopulateCell(3, 1);
            gameOfLife.PopulateCell(2, 0);

            GameOfLife gameOfLifeParallel = new(gameOfLife);

            stopwatch.Start();
            while (cycle < targetCycles)
            {
                gameOfLife.SimulateGameTick();
                cycle++;
            }
            stopwatch.Stop();
            bool[,] singleThreadState = gameOfLife.GetGrid();
            long singleThreadTime = stopwatch.ElapsedMilliseconds;

            cycle = 0;
            stopwatch.Restart();
            while (cycle < targetCycles)
            {
                gameOfLifeParallel.SimulateGameTickParallel();
                cycle++;
            }
            stopwatch.Stop();
            bool[,] multiThreadState = gameOfLifeParallel.GetGrid();
            long multiThreadTime = stopwatch.ElapsedMilliseconds;

            bool multiThreadEquality =
                    singleThreadState.Rank == multiThreadState.Rank &&
                    Enumerable.Range(0, singleThreadState.Rank).All(dimension => singleThreadState.GetLength(dimension) == multiThreadState.GetLength(dimension)) &&
                    singleThreadState.Cast<bool>().SequenceEqual(multiThreadState.Cast<bool>());

            Console.Write(
                $"Running {targetCycles} cycles each:\n" +
                $"\n" +
                $"Single-threaded: {singleThreadTime}ms\n" +
                $"Multi-threaded: {multiThreadTime}ms\n" +
                $"\n" +
                $"State Equality: {multiThreadEquality}\n"
                );

            OutputToConsole(gameOfLife, false);

            OutputToConsole(gameOfLifeParallel, false);
        }

        static void SpaceProgressingConsoleOutputTest()
        {
            GameOfLife gameOfLife = new(8, 8);

            gameOfLife.PopulateCell(2, 2);
            gameOfLife.PopulateCell(3, 2);
            gameOfLife.PopulateCell(1, 2);
            gameOfLife.PopulateCell(3, 1);
            gameOfLife.PopulateCell(2, 0);

            while (true)
            {
                OutputToConsole(gameOfLife, false);
                Console.ReadKey();
                gameOfLife.SimulateGameTick();
            }
        }
    }


}
