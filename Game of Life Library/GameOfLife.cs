using System.Threading.Tasks;

namespace GameOfLifeLibrary;

public class GameOfLife
{
    private bool edgeLooping;
    private bool[,] currentGrid, nextGrid;
    private static readonly Coord[] positionsToCheck = new Coord[8]
    {
            new(-1, -1), new(-1, 0), new(-1, 1),
            new(0, -1),              new(0, 1),
            new(1, -1),  new(1, 0),  new(1, 1)
    };

    public struct Coord
    {
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static Coord operator +(Coord a) => a;
        public static Coord operator -(Coord a) => new Coord(-a.X, -a.Y);

        public static Coord operator +(Coord a, Coord b)
            => new Coord(a.X + b.X, a.Y + b.Y);
        public static Coord operator -(Coord a, Coord b)
            => new Coord(a.X - b.X, a.Y - b.Y);
    }

    public GameOfLife(int x, int y, bool edgeLooping = true)
    {
        
        currentGrid = new bool[x, y];
        nextGrid = new bool[x, y];
        this.edgeLooping = edgeLooping;
    }

    public GameOfLife(GameOfLife gameOfLife)
    {
        currentGrid = gameOfLife.currentGrid;
        nextGrid = gameOfLife.nextGrid;
        edgeLooping = gameOfLife.edgeLooping;
    }

    public void PopulateCell(int x, int y)
    {
        currentGrid[x, y] = true;
    }

    public void DepopulateCell(int x, int y)
    {
        currentGrid[x, y] = false;
    }

    public void ToggleCell(int x, int y)
    {
        currentGrid[x, y] = !currentGrid[x, y];
    }

    public void SimulateGameTick()
    {
        for (int x = 0; x < currentGrid.GetLength(0); x++)
        {
            for (int y = 0; y < currentGrid.GetLength(1); y++)
            {
                nextGrid[x, y] = ApplySurvivalRules(currentGrid[x, y], GetLivingNeighbors(new Coord(x, y)));
            }
        }

        currentGrid = nextGrid.Clone() as bool[,];
    }

    public void SimulateGameTickParallel()
    {
        Parallel.For(0, currentGrid.GetLength(0), x =>
        {
            for (int y = 0; y < currentGrid.GetLength(1); y++)
            {
                nextGrid[x, y] = ApplySurvivalRules(currentGrid[x, y], GetLivingNeighbors(new Coord(x, y)));
            }
        });

        currentGrid = nextGrid.Clone() as bool[,];
    }

    private int GetLivingNeighbors(Coord coord)
    {
        int activeNeighbors = 0;

        foreach (Coord adjPos in positionsToCheck)
        {
            Coord activeCell = coord + adjPos;

            if (IsPosInBounds(activeCell))
            {
                if (currentGrid[activeCell.X, activeCell.Y])
                    activeNeighbors++;
            }
            else if (edgeLooping)
            {
                activeCell = ApplyEdgeLoopingToActiveCell(activeCell);
                if (currentGrid[activeCell.X, activeCell.Y])
                    activeNeighbors++;
            }
        }

        return activeNeighbors;

    }

    private Coord ApplyEdgeLoopingToActiveCell(Coord activeCell)
    {
        if (activeCell.X < 0)
        {
            activeCell.X = currentGrid.GetLength(0) - 1;
        }
        else if (activeCell.X >= currentGrid.GetLength(0))
        {
            activeCell.X = 0;
        }
        if (activeCell.Y < 0)
        {
            activeCell.Y = currentGrid.GetLength(1) - 1;
        }
        else if (activeCell.Y >= currentGrid.GetLength(1))
        {
            activeCell.Y = 0;
        }
        return activeCell;
    }



    private static bool ApplySurvivalRules(bool currentState, int neighbors)
    {
        if (currentState)
        {
            if (neighbors == 2 || neighbors == 3) 
                return true;
            else 
                return false;
        }
        else
        {
            if (neighbors == 3) 
                return true;
            else 
                return false;
        }
    }

    private bool IsPosInBounds(Coord coord)
    {
        if (coord.X < 0 || coord.Y < 0) 
            return false;

        if (coord.X >= currentGrid.GetLength(0) || coord.Y >= currentGrid.GetLength(1)) 
            return false;

        return true;
    }

    public bool[,] GetGrid()
    {
        return currentGrid;
    }

    public void SetEdgeLooping(bool shouldEdgeLoop)
    {
        edgeLooping = shouldEdgeLoop;
    }
}
