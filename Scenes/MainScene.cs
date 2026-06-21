using Godot;
using System;
using Minesweeper.Scenes.MainScene.Logic;
using Minesweeper.Scenes.MainScene.Nodes;

public partial class MainScene : Node2D
{
	[Export] private GridContainer _grid;
	[Export] private int _numberOfColumns;
	[Export] private int _numberOfMines;
	[Export] private Button _restartButton;
	[Export] private Label _gameOverLabel;

	private readonly FieldSeeder _seeder = new();

	private CellsManager _cellsManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_restartButton.Pressed += StartGame;
		StartGame();
	}

	private void StartGame()
	{
		_gameOverLabel.Visible = false;
		foreach (var cell in _grid.GetChildren())
		{
			_grid.RemoveChild(cell);
		}
		var gridCells = CreateBoard();
		_cellsManager = new CellsManager(gridCells, _numberOfColumns);
	}

	private GridCell[] CreateBoard()
	{
		var gridCells = new GridCell[_numberOfColumns*_numberOfColumns];
		var cellSize = new Vector2(_grid.Size.X/_numberOfColumns, _grid.Size.Y/_numberOfColumns);
		var seededCellsValues = _seeder.GetSeededCellsValues(_numberOfColumns, _numberOfMines);
		for (int i=0; i<seededCellsValues.Length; i++)
		{
			var cell = new GridCell(i, seededCellsValues[i].Item1, seededCellsValues[i].Item2)
				{ CustomMinimumSize = cellSize };
			cell.BombPressed += HandleGameOver;
			gridCells[i] = cell;
			_grid.AddChild(cell);
		}
		return gridCells;
	}

	private void HandleGameOver()
	{
		_gameOverLabel.Visible = true;
		foreach (GridCell cell in _grid.GetChildren())
		{
			cell.Press();
			cell.Disabled = true;
		}
	}
}
