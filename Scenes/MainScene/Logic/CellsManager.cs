using System.Collections.Generic;
using Godot;
using Minesweeper.Scenes.MainScene.Logic.Enums;
using Minesweeper.Scenes.MainScene.Nodes;

namespace Minesweeper.Scenes.MainScene.Logic;

public class CellsManager
{
    private readonly GridCell[] _cells;
    private readonly int _numberOfColumns;
    
    public CellsManager(GridCell[] cells, int numberOfColumns)
    {
        _cells = cells;
        _numberOfColumns = numberOfColumns;
        foreach (var cell in _cells)
        {
            cell.EmptyCellClicked += HandleEmptyCellClick;
            cell.NumberCellClicked += HandleNumberCellClick;
        }
    }

    private void HandleEmptyCellClick(int id)
    {
        var grid = FormGrid(_cells);
        var linkedEmptyNeighbours = new HashSet<GridCell>();
        GetLinkedEmptyNeighbors(grid, id, linkedEmptyNeighbours);
        foreach (var cell in linkedEmptyNeighbours)
        {
            cell.Press();
        }
    }

    private void HandleNumberCellClick(int id)
    {
        var needToOpen = new HashSet<GridCell>();
        var numberOfFlaggedBombs = GetNumberOfBombNeighbors(id, needToOpen);
        if (numberOfFlaggedBombs == _cells[id].CellValue)
        {
            foreach (var cell in needToOpen)
            {
                cell.Press();
            }
        }
    }

    private GridCell[,] FormGrid(GridCell[] cells)
    {
        int rows = cells.Length / _numberOfColumns;
        var grid = new GridCell[rows, _numberOfColumns];
        for (int i = 0; i < cells.Length; i++)
            grid[i / _numberOfColumns, i % _numberOfColumns] = cells[i];
        return grid;
    }

    private void GetLinkedEmptyNeighbors(GridCell[,] cells, int id, HashSet<GridCell> visited)
    {
        int row = id / _numberOfColumns;
        int col = id % _numberOfColumns;
        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;
            int r = row + dr, c = col + dc;
            if (r < 0 || r >= _numberOfColumns || c < 0 || c >= _numberOfColumns) continue;
            var cell = cells[r, c];
            if (cell.CellValueType != CellValuesTypes.Empty || cell.IsToggled) continue;
            if(!visited.Add(cells[r,c])) continue;
            
            GetLinkedEmptyNeighbors(cells, r * _numberOfColumns + _numberOfColumns, visited);
        }
    }

    private int GetNumberOfBombNeighbors(int id, HashSet<GridCell> needToOpen)
    {
        var grid = FormGrid(_cells);
        var count = 0;
        int row = id / _numberOfColumns;
        int col = id % _numberOfColumns;
        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;
            int r = row + dr, c = col + dc;
            if (r < 0 || r >= _numberOfColumns || c < 0 || c >= _numberOfColumns) continue;
            var cell = grid[r, c];
            if (cell.CellValueType == CellValuesTypes.Bomb && cell.IsFlagged) count++;
            if (cell.CellValueType != CellValuesTypes.Bomb && !cell.IsToggled)
            {
                needToOpen.Add(grid[r, c]);
            }
        }
        
        return count;
    }
}