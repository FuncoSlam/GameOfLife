using System.Threading.Tasks;

namespace GameOfLifeLibrary;

public class GameOfLife
{
    private bool edgeLooping;
    private bool[,] currentGrid, nextGrid;
    private static readonly (int, int)[] positionsToCheck = new (int, int)[8]
    {
            (-1, -1), (-1, 0), (-1, 1),
            (0, -1),           (0, 1),
            (1, -1),  (1, 0),  (1, 1)
    };

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
                nextGrid[x, y] = ApplySurvivalRules(currentGrid[x, y], GetLivingNeighbors(x, y));
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
                nextGrid[x, y] = ApplySurvivalRules(currentGrid[x, y], GetLivingNeighbors(x, y));
            }
        });

        currentGrid = nextGrid.Clone() as bool[,];
    }

    private int GetLivingNeighbors(int x, int y)
    {
        int activeNeighbors = 0;

        foreach ((int,int) position in positionsToCheck)
        {
            (int, int) activeCell = (x + position.Item1, y + position.Item2);

            if (IsPosInBounds(activeCell.Item1, activeCell.Item2))
            {
                if (currentGrid[activeCell.Item1, activeCell.Item2])
                    activeNeighbors++;
            }
            else if (edgeLooping)
            {
                activeCell = ApplyEdgeLoopingToActiveCell(activeCell);
                if (currentGrid[activeCell.Item1, activeCell.Item2])
                    activeNeighbors++;
            }
        }

        return activeNeighbors;

    }

    private (int, int) ApplyEdgeLoopingToActiveCell((int,int) activeCell)
    {
        if (activeCell.Item1 < 0)
        {
            activeCell.Item1 = currentGrid.GetLength(0) - 1;
        }
        else if (activeCell.Item1 >= currentGrid.GetLength(0))
        {
            activeCell.Item1 = 0;
        }
        if (activeCell.Item2 < 0)
        {
            activeCell.Item2 = currentGrid.GetLength(1) - 1;
        }
        else if (activeCell.Item2 >= currentGrid.GetLength(1))
        {
            activeCell.Item2 = 0;
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

    private bool IsPosInBounds(int x, int y)
    {
        if (x < 0 || y < 0) 
            return false;

        if (x >= currentGrid.GetLength(0) || y >= currentGrid.GetLength(1)) 
            return false;

        return true;
    }

    public bool[,] GetGrid()
    {
        return currentGrid;
    }
}
