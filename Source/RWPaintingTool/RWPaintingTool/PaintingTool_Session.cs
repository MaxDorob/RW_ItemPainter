using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private Pawn? _carryingPawn;
    private Thing _thing;


    private int _curColorIndex = 0;
    private int _highLightedIndex = -1;
    
    private ColorTracker _tracker;
    private MaskTracker _maskTracker;
    
    // Sides
    
    public bool IsPawn => _carryingPawn != null;
    
    public void SetFor(Thing thing)
    {
        //Resolve graphic

        //
        forcePause = true;
        
        _thing = thing;
        _tracker = null;

        
        //TODO Get correct colors and then set it too
        //_maskTracker = ColorTrackerDB.GetMaskTracker(_thing);
        //_selectedMaskIndex = _maskTracker.CurMaskID;
        
        //TLog.Debug("Tracker: " + (_tracker != null));
        //TLog.Debug("MaskTracker: " + (_maskTracker != null));
        
        _curColorIndex = 0;

        _colorPicker.SetColors(ColorTracker.ColorSet[_curColorIndex]);
    }
    private ColorTracker ColorTracker => _tracker ??= ColorTrackerDB.GetTracker(_thing);
    private void SetPalette(SixColorSet palette)
    {
        if (ColorTracker != null)
        {
            ColorTracker.TempColorSet = palette;
        }
        apparelColors[_thing as Apparel] = palette.colorOne;
    }
    
    private void Notify_ColorChanged(Color color)
    {
        apparelColors[_thing as Apparel] = color;
        if (ColorTracker == null)
        {
            return;
        }
        var set = ColorTracker.TempColorSet ?? ColorTracker.ColorSet;
        set[_curColorIndex] = color;
        ColorTracker.TempColorSet = set;
    }

    public override void Close(bool doCloseSound = true)
    {
        base.Close(doCloseSound);
        foreach (var apparel in pawn.apparel.WornApparel)
        {
            var colorTracker = ColorTrackerDB.GetTracker(apparel);
            colorTracker?.Reset();
        }
        
    }

    private void Notify_MaskChanged()
    {
        //_maskTracker.SetMaskID(_selectedMaskIndex);
        _tracker.Notify_ColorsChanged();
    }
}