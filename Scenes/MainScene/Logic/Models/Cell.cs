using Minesweeper.Scenes.MainScene.Logic.Enums;

namespace Minesweeper.Scenes.MainScene.Logic.Models;

public class Cell
{
    public bool IsPressed { get; set; }

    public bool IsFlagged { get; set; }

    public CellValuesTypes CellValueType { get; set; }
    
    public int? NumberValue { get; set;}

    private readonly string _cellValue;
    
    public string CellValue
    {
        get => NumberValue.HasValue ? NumberValue.Value.ToString() : _cellValue;

        init => _cellValue = value;
    }

}