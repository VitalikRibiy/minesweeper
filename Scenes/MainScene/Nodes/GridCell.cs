using System;
using Godot;
using Minesweeper.Scenes.MainScene.Logic;
using Minesweeper.Scenes.MainScene.Logic.Enums;

namespace Minesweeper.Scenes.MainScene.Nodes;

public partial class GridCell: Button
{
    private readonly int _id;
    
    private bool _toggled = false;
    
    public bool IsToggled => _toggled;

    private bool _isFlagged = false;

    public bool IsFlagged => _isFlagged;
    
    private CellValuesTypes _cellValueType;
    
    public CellValuesTypes CellValueType => _cellValueType;

    private uint? _cellValue = null;
    
    public uint? CellValue { get => _cellValue; }
    
    [Signal] public delegate void NumberCellClickedEventHandler(int id);
    [Signal] public delegate void EmptyCellClickedEventHandler(int id);
    [Signal] public delegate void BombPressedEventHandler();

    public GridCell(int id, CellValuesTypes cellValueType, uint? value = null)
    {
        _id = id;
        ToggleMode = true;
        Pressed += HandlePressed; 
        _cellValueType = cellValueType;
        _cellValue = value;
        GuiInput += (InputEvent e) => HandleRightClick(e);
    }

    public void Press()
    {
        ButtonPressed = true;
        _toggled = true;
        switch (_cellValueType)
        {
            case CellValuesTypes.Empty:
                Text = string.Empty;
                break;
            case CellValuesTypes.Number:
                Text = _cellValue.ToString();
                break;
            case CellValuesTypes.Bomb:
                Text = "B";
                break;
        }
    }

    private void HandlePressed()
    {
        if (_toggled)
        {
            ButtonPressed = true;
        }
        _toggled = true;
        switch (_cellValueType)
        {
            case CellValuesTypes.Empty:
                Text = string.Empty;
                EmitSignal(SignalName.EmptyCellClicked, _id);
                break;
            case CellValuesTypes.Number:
                Text = _cellValue.ToString();
                EmitSignal(SignalName.NumberCellClicked,_id);
                break;
            case CellValuesTypes.Bomb:
                Text = "B";
                EmitSignal(SignalName.BombPressed);
                break;
        }
    }

    private void HandleRightClick(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventMouseButton mouseEvent || !mouseEvent.Pressed ||
            mouseEvent.ButtonIndex != MouseButton.Right || Disabled) return;

        if (!_toggled)
        {
            _isFlagged = !_isFlagged;
            Text = _isFlagged ? "P" : "";
        }
    }
}