using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;
using System.Diagnostics;
using GameOfLifeLibrary;
using Game_of_Life_Raylib.Properties;

static class Program
{
	static int windowWidth = 1200, windowHeight = 800;

	static GameOfLife gameOfLife = new(40, 34);
	static int targetTPS = 8;
	static Stopwatch tickTimer = new();
	static Int64 msBetweenTicks = 1000;
	static bool isPaused = false;

	static Color backgroundColor = Color.RAYWHITE;
	static Color inactiveCellColor = Color.LIGHTGRAY;
	static Color activeCellColor = Color.DARKGRAY;
	static Color cellBorderHighlight = Color.BLUE;

	static Camera2D camera = new(new Vector2(windowWidth / 2, windowHeight / 2), Vector2.Zero, 0f, 1f);
	static float camSpeed = 20f;
	static float zoomSpeed = 0.3f;
	static float minZoom = 0.1f;
	static Vector2 prevMousePos = GetMousePosition();

	static int cellWidth = 20;
	static int cellBorderWidth = 1;

	public static void Main(string[] args)
	{

		InitWindow(windowWidth, windowHeight, "Conway's Game of Life");
		// Image myImage = Resources.Conway_Toy;
		SetTargetFPS(120);
		tickTimer.Start();

		// INITIALIZE GAME STATE
		gameOfLife.PopulateCell(2, 2);
		gameOfLife.PopulateCell(3, 2);
		gameOfLife.PopulateCell(1, 2);
		gameOfLife.PopulateCell(3, 1);
		gameOfLife.PopulateCell(2, 0);

		while (!WindowShouldClose())
		{
			// SETUP

			int totalCellWidth = (cellWidth + cellBorderWidth * 2);
			bool[,] gridToRender = gameOfLife.GetGrid();
			int startX = -gridToRender.GetLength(0) * totalCellWidth / 2;
			int startY = -gridToRender.GetLength(1) * totalCellWidth / 2;

			(int, int)? cellUnderCursor = GetCellUnderCursor();


			// CLEAR SCREEN

			BeginDrawing();

			ClearBackground(backgroundColor);


			// CAMERA

			float mouseWheelDelta = GetMouseWheelMove();
			float newZoom = Math.Max(camera.zoom + mouseWheelDelta * zoomSpeed * camera.zoom, minZoom);
			camera.zoom = newZoom;

			Vector2 mousePos = GetMousePosition();
			Vector2 mousePosDelta = prevMousePos - mousePos;
			prevMousePos = mousePos;

			if (IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON))
				camera.target = GetScreenToWorld2D(mousePosDelta + camera.offset, camera);


			// MODEL

			if (tickTimer.ElapsedMilliseconds > (1000 / targetTPS) & !isPaused)
			{
				msBetweenTicks = tickTimer.ElapsedMilliseconds;
				tickTimer.Restart();
				gameOfLife.SimulateGameTick();
			}

			if (IsKeyPressed(KeyboardKey.KEY_A))
				targetTPS++;

			if (IsKeyPressed(KeyboardKey.KEY_S))
				targetTPS = Math.Max(1, targetTPS - 1);

			if (IsKeyPressed(KeyboardKey.KEY_SPACE))
				isPaused = !isPaused;

			if (cellUnderCursor != null)
            {
				if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON))
					gameOfLife.PopulateCell(cellUnderCursor.Value.Item1, cellUnderCursor.Value.Item2);

				else if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON))
					gameOfLife.DepopulateCell(cellUnderCursor.Value.Item1, cellUnderCursor.Value.Item2);
			}


			// DRAWING 2D

			BeginMode2D(camera);
			{
				DrawCircle(0, 0, 6f, Color.RED);
				RenderGame();
			}
			EndMode2D();


			// DRAWING UI

			DrawText(
#if DEBUG
				$"Camera Target: ({camera.target.X.ToString("n2")}, {camera.target.Y.ToString("n2")})\n" +
				$"Camera Offset: ({camera.offset.X.ToString("n2")}, {camera.offset.Y.ToString("n2")})\n" +
				$"Mouse Position: ({GetMousePosition().X}, {GetMousePosition().Y})\n" +
				$"Zoom Level: {camera.zoom.ToString("n2")}\n" +
#endif
				$"FPS: {(1f/GetFrameTime()).ToString("n2")}\n" +
				$"TPS: {(isPaused ? "PAUSED" : ((1f/msBetweenTicks)*1000).ToString("n2"))}",
					14, 14, 20, Color.BLACK);

			EndDrawing();


			void RenderGame()
			{
				(int, int)? cellUnderCursor = GetCellUnderCursor();
				if (cellUnderCursor != null)
				{
					int highlightStartX = startX + (totalCellWidth * cellUnderCursor.Value.Item1);
					int highlightStartY = startY + (totalCellWidth * cellUnderCursor.Value.Item2);
					DrawRectangle(highlightStartX, highlightStartY, totalCellWidth, totalCellWidth, cellBorderHighlight);
				}

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
			}


			(int, int)? GetCellUnderCursor()
			{
				if (!IsCursorOnScreen())
					return null;

				Vector2 mouseWorldPos = GetScreenToWorld2D(GetMousePosition(), camera);
				Vector2 mouseRelativeToGridStart = mouseWorldPos - new Vector2(startX, startY);

				if (mouseRelativeToGridStart.X <= 0 || mouseRelativeToGridStart.Y <= 0)
					return null;

				Vector2 positionInGridByGridSize = mouseRelativeToGridStart / totalCellWidth;

				if (positionInGridByGridSize.X > gridToRender.GetLength(0) || positionInGridByGridSize.Y > gridToRender.GetLength(1))
					return null;

				(int, int) cellWithMouse = ((int)positionInGridByGridSize.X, (int)positionInGridByGridSize.Y);

				return cellWithMouse;
			}
		}
		CloseWindow();
	}
}