using Godot;
using Minesweeper.Scenes.MainScene.Logic.Enums;

namespace Minesweeper.Scenes.MainScene.Logic;

public class FieldSeeder
{
    public (CellValuesTypes, uint?)[] GetSeededCellsValues(int numberOfColumns, int numberOfMines)
    {
        var numberOfCells = numberOfColumns * numberOfColumns;
        var cellsValues = new (CellValuesTypes,uint?)[numberOfCells];
        var bombIndexes = new int[numberOfMines];
        for (uint i = 0; i < numberOfMines; i++)
        {
            var bombPlaced = false;
            while (!bombPlaced)
            {
                var index = GD.RandRange(0, (int)numberOfCells-1);
                if (cellsValues[index].Item1 == CellValuesTypes.Empty)
                {
                    cellsValues[index].Item1 = CellValuesTypes.Bomb;
                    bombPlaced = true;
                    bombIndexes[i] = index;
                }
            }
        }
        
        int cols = numberOfColumns, rows = numberOfColumns;
        foreach (int bomb in bombIndexes)
        {
            int br = bomb / cols, bc = bomb % cols;
            for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int r = br + dr, c = bc + dc;
                if (r < 0 || r >= rows || c < 0 || c >= cols) continue;
                var idx = r * cols + c;
                if(cellsValues[idx].Item1 == CellValuesTypes.Bomb) continue;
                cellsValues[idx] = (CellValuesTypes.Number, (cellsValues[idx].Item2 ?? 0) + 1);
            }
        }
        
        return cellsValues;
    }
}