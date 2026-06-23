using System;
using Minesweeper.Scenes.MainScene.Logic.Enums;
using Minesweeper.Scenes.MainScene.Logic.Models;

namespace Minesweeper.Scenes.MainScene.Logic;

public static class FieldSeeder
{
    public static Cell[,] GetSeededCellsValues(int numberOfColumns, int numberOfMines)
    {
        var cellsValues = new Cell[numberOfColumns, numberOfColumns];
        for (int r = 0; r < numberOfColumns; r++)
        for (int c = 0; c < numberOfColumns; c++)
            cellsValues[r, c] = new Cell();
        var bombIndexes = new (int, int)[numberOfMines];
        var random = new Random();
        for (uint i = 0; i < numberOfMines; i++)
        {
            var bombPlaced = false;
            while (!bombPlaced)
            {
                var r = random.Next(0, numberOfColumns);
                var c = random.Next(0, numberOfColumns);
                if (cellsValues[r,c] == null || cellsValues[r,c].CellValueType == CellValuesTypes.Empty)
                {
                    cellsValues[r,c]= new Cell() {CellValueType = CellValuesTypes.Bomb, CellValue = "B"};
                    bombPlaced = true;
                    bombIndexes[i] = (r,c);
                }
            }
        }
        
        int cols = numberOfColumns, rows = numberOfColumns;
        foreach (var bomb in bombIndexes)
        {
            int br = bomb.Item1, bc = bomb.Item2;
            for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int r = br + dr, c = bc + dc;
                if (r < 0 || r >= rows || c < 0 || c >= cols) continue;
                if(cellsValues[r, c].CellValueType == CellValuesTypes.Bomb) continue;
                cellsValues[r, c].CellValueType = CellValuesTypes.Number;
                cellsValues[r, c].NumberValue = ((cellsValues[r, c].NumberValue ?? 0) + 1);
            }
        }
        
        return cellsValues;
    }
}