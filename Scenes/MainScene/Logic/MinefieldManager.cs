using System;
using System.Collections.Generic;
using Minesweeper.Scenes.MainScene.Logic.Enums;
using Minesweeper.Scenes.MainScene.Logic.Excepions;
using Minesweeper.Scenes.MainScene.Logic.Models;

namespace Minesweeper.Scenes.MainScene.Logic;

public class MinefieldManager
{
    private readonly Cell[,] _cells;
    
    public MinefieldManager(Cell[,] cells)
    {
        _cells = cells;
    }

    public IReadOnlyCollection<(int, int, string)> Reveal(int r, int c)
    {
        var needToReveal = new HashSet<(int, int)> { (r, c) };
        switch (_cells[r,c].CellValueType)
        {
            case CellValuesTypes.Empty:
                needToReveal = HandleEmptyCellClick(r,c);
                break;
            case CellValuesTypes.Number:
                needToReveal = HandleNumberCellClick(r,c);
                break;
            case CellValuesTypes.Bomb:
                throw new GameException();
        }

        var valuesToReveal = new HashSet<(int, int, string)>();
        foreach (var cellToReveal in needToReveal)
        {
            var cell = _cells[cellToReveal.Item1, cellToReveal.Item2];
            cell.IsPressed = true;
            valuesToReveal.Add((cellToReveal.Item1, cellToReveal.Item2, cell.CellValue));
        }
        
        return valuesToReveal;
    }
    
    private HashSet<(int,int)> HandleEmptyCellClick(int row, int col)
    {
        var needToReveal = new HashSet<(int,int)>();
        GetLinkedEmptyNeighbors(row, col, needToReveal);
        return needToReveal;
    }

    private HashSet<(int,int)> HandleNumberCellClick(int row, int col)
    {
        var needToReveal = new HashSet<(int,int)>();
        var numberOfFlaggedBombs = GetNumberOfBombNeighbors(row, col, needToReveal);
        return numberOfFlaggedBombs.ToString() == _cells[row, col].CellValue ? needToReveal : new HashSet<(int,int)>();
    }

    private void GetLinkedEmptyNeighbors(int row, int col, HashSet<(int, int)> needToReveal)
    {
        int rows = _cells.GetLength(0), cols = _cells.GetLength(1);
        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;
            int r = row + dr, c = col + dc;
            if (r < 0 || r >= rows || c < 0 || c >= cols) continue;

            var cell = _cells[r, c];
            if (cell.IsPressed || cell.IsFlagged || cell.CellValueType == CellValuesTypes.Bomb) continue;
            if (!needToReveal.Add((r,c))) continue;                 // reveal it (empty OR number)

            if (cell.CellValueType == CellValuesTypes.Empty)  // recurse ONLY from empties
                GetLinkedEmptyNeighbors(r, c, needToReveal);
        }
    }

    private int GetNumberOfBombNeighbors(int row, int col, HashSet<(int, int)> needToReveal)
    {
        var count = 0;
        int rows = _cells.GetLength(0), cols = _cells.GetLength(1);
        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;
            int r = row + dr, c = col + dc;
            if (r < 0 || r >= rows || c < 0 || c >= cols) continue;
            var cell = _cells[r, c];
            if (cell.CellValueType == CellValuesTypes.Bomb && cell.IsFlagged) count++;
            if (cell.CellValueType != CellValuesTypes.Bomb && !cell.IsPressed)
            {
                needToReveal.Add((r,c));
            }
        }
        
        return count;
    }

    public void Flag(int r, int c)
    {
        _cells[r, c].IsFlagged = !_cells[r,c].IsFlagged;
    }
}