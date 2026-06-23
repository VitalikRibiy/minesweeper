using Godot;
using Minesweeper.Scenes.MainScene.Logic;
using Minesweeper.Scenes.MainScene.Nodes;

public partial class MainScene : Node2D
{
	[Export] private GridContainer _grid;
	[Export] private int _numberOfColumns;
	[Export] private int _numberOfMines;
	[Export] private Button _restartButton;
	[Export] private Label _gameOverLabel;

	private GridCell[,] _cells;
	private MinefieldManager _minefieldManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_restartButton.Pressed += StartGame;
		StartGame();
	}

	private void StartGame()
	{
		_gameOverLabel.Visible = false;
		if (_cells != null)
			foreach (var cell in _cells)
			{
				cell.QueueFree();
			}

		_cells = CreateBoard();
	}

	private GridCell[,] CreateBoard()
	{
		var cells = FieldSeeder.GetSeededCellsValues(_numberOfColumns, _numberOfMines);
		var gridCells = new GridCell[_numberOfColumns, _numberOfColumns];
		var cellSize = new Vector2(_grid.Size.X/_numberOfColumns, _grid.Size.Y/_numberOfColumns);
		
		for(var r=0; r<_numberOfColumns; r++)
		for (var c = 0; c < _numberOfColumns; c++)
		{
			var gridCell = new GridCell(r, c) {CustomMinimumSize = cellSize};
			gridCell.LeftClicked += HandleLeftClicked;
			gridCell.RightClicked += HandleRightClicked;
			gridCells[r, c] = gridCell;
			_grid.AddChild(gridCell);
		}
		
		_minefieldManager = new MinefieldManager(cells);

		return gridCells;
	}
	
	private void HandleLeftClicked(int r, int c)
	{
		var needToReveal = _minefieldManager.Reveal(r, c);
		if (needToReveal is null)
		{
			HandleGameOver();
			return;
		}
		
		foreach (var cellToReveal in needToReveal)
		{
			var cell = _cells[cellToReveal.Item1, cellToReveal.Item2];
			cell.ButtonPressed = true;
			cell.Render(cellToReveal.Item3);
		}
	}

	private void HandleRightClicked(int r, int c)
	{
		var cell = _cells[r, c];
		cell.IsFlagged = !cell.IsFlagged;
		_minefieldManager.Flag(r, c);
		cell.Render(cell.IsFlagged ? "P" : string.Empty);
	}
	
	private void HandleGameOver()
	{
		_gameOverLabel.Visible = true;
		var revealedCells = _minefieldManager.RevealAll();
		foreach (var cell in revealedCells)
		{
			_cells[cell.Item1, cell.Item2].ButtonPressed = true;
			_cells[cell.Item1, cell.Item2].Disabled = true;
			_cells[cell.Item1, cell.Item2].Render(cell.Item3);
		}
	}
}
