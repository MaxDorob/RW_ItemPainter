using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private Pawn pawn;
    private Thing _thing;
    private Dictionary<Apparel, Color> apparelColors = new Dictionary<Apparel, Color>();
    private Vector2 ButSize = Dialog_StylingStation.ButSize;
    private Rot4 rotation = Rot4.South;
    private bool showSelectedOnly = false;
    private bool showClothes = true, showHeadgear = true;
    private int _curColorIndex = 0;    
    private ColorTracker _tracker;
    
    
    public void SetFor(Thing thing)
    {
        forcePause = true;
        _thing = thing;
        _tracker = null;
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
}