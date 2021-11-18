using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;
using System.Diagnostics;
using GameOfLifeLibrary;

static class Program
{
    static int windowHeight = 800, windowWidth = 400;
    static GameOfLife gameOfLife = new(100, 100);

    static Color backgroundColor = Color.RAYWHITE;
    static Color inactiveCellColor = Color.LIGHTGRAY;
    static Color activeCellColor = Color.DARKGRAY;
    static Color cellBorderHighlight = Color.BLUE;

    static Camera2D camera = new(new Vector2(windowHeight / 2, windowWidth / 2), Vector2.Zero, 0f, 1f);
    static float camSpeed = 200f;
    static float zoomSpeed = 0.3f;

    static int cellWidth = 20;
    static int cellBorderWidth = 1;

    public static void Main()
    {

        InitWindow(windowHeight, windowWidth, "Conway's Game of Life");
        SetTargetFPS(60);

        while (!WindowShouldClose())
        {
            // CLEAR SCREEN

            BeginDrawing();

            ClearBackground(backgroundColor);


            // CAMERA

            if (IsKeyDown(KeyboardKey.KEY_UP))
                camera.target.Y -= camSpeed * GetFrameTime() / camera.zoom;

            if (IsKeyDown(KeyboardKey.KEY_DOWN))
                camera.target.Y += camSpeed * GetFrameTime() / camera.zoom;

            if (IsKeyDown(KeyboardKey.KEY_LEFT))
                camera.target.X -= camSpeed * GetFrameTime() / camera.zoom;

            if (IsKeyDown(KeyboardKey.KEY_RIGHT))
                camera.target.X += camSpeed * GetFrameTime() / camera.zoom;

            if (IsKeyDown(KeyboardKey.KEY_Z))
                camera.zoom *= zoomSpeed * GetFrameTime();

            if (IsKeyDown(KeyboardKey.KEY_X))
                camera.zoom /= zoomSpeed * GetFrameTime();


            // DRAWING 2D

            BeginMode2D(camera);
            {
                DrawCircle(0, 0, 15f, Color.RED);
                RenderGame();
            }
            EndMode2D();


            // DRAWING UI

            DrawText($"Camera Target: ({camera.target.X}, {camera.target.Y})\n" +
                $"Mouse Position: ({GetMousePosition().X}, {GetMousePosition().Y})\n" +
                $"", 12, 12, 20, Color.BLACK);

            EndDrawing();
        }
        CloseWindow();


        void RenderGame()
        {
            int totalCellWidth = (cellWidth + cellBorderWidth * 2);
            bool[,] gridToRender = gameOfLife.GetGrid();
            int startX = -gridToRender.GetLength(0) * totalCellWidth / 2;
            int startY = -gridToRender.GetLength(1) * totalCellWidth / 2;

            RenderCellHighlight();

            for (int x = 0; x < gridToRender.GetLength(0); x++)
            {
                for (int y = 0; y < gridToRender.GetLength(1); y++)
                {
                    int cellStartX = startX + (totalCellWidth * x) + cellBorderWidth;
                    int cellStartY = startY + (totalCellWidth * y) + cellBorderWidth;

                    Color cellColor;
                    if (gridToRender[x, y])
                        cellColor = activeCellColor;
                    else
                        cellColor = inactiveCellColor;

                    DrawRectangle(cellStartX, cellStartY, cellWidth, cellWidth, cellColor);
                }
            }

            void RenderCellHighlight()
            {
                if (!IsCursorOnScreen())
                    return;
                // TODO
            }
        }
    }
}