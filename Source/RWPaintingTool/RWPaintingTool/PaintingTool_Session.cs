using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private Pawn? _carryingPawn;
    private Thing _thing;

    //
    private SixColorSet _colorsSet;
    private int _curColorIndex = 0;
    private int _highLightedIndex = -1;
    
    private ColorTracker _tracker;
    private MaskTracker _maskTracker;
    
    // Sides
    
    public bool IsPawn => _carryingPawn != null;
    
    public void SetFor(Thing thing)
    {
        //Resolve graphic
        var graphic = thing.Graphic;

        //
        forcePause = true;
        
        _thing = thing;
        _colorPicker = new ColorPicker();
        _colorPicker.ColorChanged += color => Notify_ColorChanged(color, _curColorIndex);
        
        //TODO Get correct colors and then set it too
        _tracker = ColorTrackerDB.GetTracker(_thing);
        //_maskTracker = ColorTrackerDB.GetMaskTracker(_thing);
        //_selectedMaskIndex = _maskTracker.CurMaskID;
        
        //TLog.Debug("Tracker: " + (_tracker != null));
        //TLog.Debug("MaskTracker: " + (_maskTracker != null));
        
        _colorsSet = _tracker?.ColorSet ?? new SixColorSet() { ColorOne = _thing.DrawColor};
        
        _colorPicker.SetColors(_colorsSet[0]);
    }
    
    private void SetPalette(Palette palette)
    {
        _colorsSet = palette;
    }
    
    private void Notify_ColorChanged(Color color, int index)
    {
        _colorsSet[index] = color;
        apparelColors[_thing as Apparel] = color;
        if (_tracker == null)
        {
            return;
        }
        _tracker.SetColors(0, _colorsSet.ColorOne);
        _tracker.SetColors(1, _colorsSet.ColorTwo);
        _tracker.SetColors(2, _colorsSet.ColorThree);
        _tracker.SetColors(3, _colorsSet.ColorFour);
        _tracker.SetColors(4, _colorsSet.ColorFive);
        _tracker.SetColors(5, _colorsSet.ColorSix);
        _tracker.Notify_ColorsChanged();
    }

    public override void Close(bool doCloseSound = true)
    {
        base.Close(doCloseSound);
        _tracker.SetColors(0, _colorsSet.ColorOne);
        _tracker.SetColors(1, _colorsSet.ColorTwo);
        _tracker.SetColors(2, _colorsSet.ColorThree);
        _tracker.SetColors(3, _colorsSet.ColorFour);
        _tracker.SetColors(4, _colorsSet.ColorFive);
        _tracker.SetColors(5, _colorsSet.ColorSix);
        _tracker.Notify_ColorsChanged();
        
    }

    private void Notify_MaskChanged()
    {
        //_maskTracker.SetMaskID(_selectedMaskIndex);
        _tracker.Notify_ColorsChanged();
    }
}