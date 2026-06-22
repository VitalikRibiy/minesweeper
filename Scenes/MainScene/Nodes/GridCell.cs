using Godot;

namespace Minesweeper.Scenes.MainScene.Nodes;

public partial class GridCell: Button
{
    [Signal] public delegate void LeftClickedEventHandler(int r, int c);
    [Signal] public delegate void RightClickedEventHandler(int r, int c);
    
    private readonly int _rowId;
    private readonly int _columnId;
    public bool IsFlagged {get; set;}

    public GridCell(int row, int column)
    {
        ToggleMode = true;
        _rowId = row;
        _columnId = column;
        GuiInput += HandleMouthClick;
    }
    
    public void Render(string text)
    {
        Text = text;
    }
    
    private void HandleMouthClick(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right })
        {
            EmitSignal(SignalName.RightClicked, _rowId, _columnId);
        }

        if (inputEvent is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
        {
            ButtonPressed = true;
            EmitSignal(SignalName.LeftClicked, _rowId, _columnId);
        }
    }
}