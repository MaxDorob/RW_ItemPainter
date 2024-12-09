using RimWorld;
using TeleCore.Loader;
using TeleCore.UI;
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
    private Material _north;
    private Material _south;
    private Material _east;
    private Material _west;
    
    public bool IsPawn => _carryingPawn != null;
    
    public PaintingTool(Thing thing)
    {
        //Resolve graphic
        var graphic = thing.Graphic;
        if (thing is Apparel apparel)
        {
            ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, BodyTypeDefOf.Male, out var apparelGraphicRecord);
            graphic = apparelGraphicRecord.graphic;
        }
        
        _north = graphic.MatNorth;
        _south = graphic.MatSouth;
        _east = graphic.MatEast;
        _west = graphic.MatWest;

        //
        forcePause = true;
        
        _thing = thing;
        _colorPicker = new ColorPicker();
        _colorPicker.ColorChanged += color => Notify_ColorChanged(color, _curColorIndex);
        
        //TODO Get correct colors and then set it too
        _tracker = ColorTrackerDB.GetTracker(_thing);
        _maskTracker = ColorTrackerDB.GetMaskTracker(_thing);
        _selectedMaskIndex = _maskTracker.CurMaskID;
        
        TLog.Debug("Tracker: " + (_tracker != null));
        TLog.Debug("MaskTracker: " + (_maskTracker != null));
        
        _colorsSet = new SixColorSet
        {
            ColorOne = thing.DrawColor, // _tracker.ColorOne,
            ColorTwo = thing.DrawColorTwo, // _tracker.ColorTwo,
            ColorThree = _tracker.ColorThree,
            ColorFour = _tracker.ColorFour,
            ColorFive = _tracker.ColorFive,
            ColorSix = _tracker.ColorSix
        };
        
        _colorPicker.SetColor(_colorsSet[0]);
    }
    
    private void SetPalette(Palette palette)
    {
        _colorsSet = palette;
    }
    
    private void Notify_ColorChanged(Color color, int index)
    {
        _colorsSet[index] = color;
    }

    public override void Close(bool doCloseSound = true)
    {
        base.Close(doCloseSound);
        _tracker.SetColor(0, _colorsSet.ColorOne);
        _tracker.SetColor(1, _colorsSet.ColorTwo);
        _tracker.SetColor(2, _colorsSet.ColorThree);
        _tracker.SetColor(3, _colorsSet.ColorFour);
        _tracker.SetColor(4, _colorsSet.ColorFive);
        _tracker.SetColor(5, _colorsSet.ColorSix);
        _tracker.Notify_ColorsChanged();
        
    }

    private void Notify_MaskChanged()
    {
        _maskTracker.SetMaskID(_selectedMaskIndex);
        _tracker.Notify_ColorsChanged();
    }
}