using System.Collections.Generic;
using TeleCore.Events;
using TeleCore.Events.Args;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public class GameComponent_ColorTracking : GameComponent
{
    public GameComponent_ColorTracking(Game game)
    {
    }

    public override void ExposeData()
    {
        base.ExposeData();
        ColorTrackerDB.ExposeData();
    }
}

public class ColorTracker : IExposable
{
    private Thing _thing;
    
    private Color? _one;
    private Color? _two;
    private Color? _three;
    private Color _four;
    private Color _five;
    private Color _six;

    public ColorTracker(Thing thing)
    {
        _thing = thing;
        _three = Color.white;
    }

    public Color ColorOne => _one ?? _thing.DrawColor;
    public Color ColorTwo => _two ?? _thing.DrawColorTwo;
    public Color ColorThree => _three ?? Color.white;
    public Color ColorFour => _four;
    public Color ColorFive => _five;
    public Color ColorSix => _six;
    
    public void SetColor(int index, Color color)
    {
        switch (index)
        {
            case 0:
                _one = color;
                break;
            case 1:
                _two = color;
                break;
            case 2:
                _three = color;
                break;
            case 3:
                _four = color;
                break;
            case 4:
                _five = color;
                break;
            case 5:
                _six = color;
                break;
        }
    }

    public void ExposeData()
    {
        Scribe_References.Look(ref _thing, "thing");
        Scribe_Values.Look(ref _one, "colorOne");
        Scribe_Values.Look(ref _two, "colorTwo");
        Scribe_Values.Look(ref _three, "colorThree");
        Scribe_Values.Look(ref _four, "colorFour");
        Scribe_Values.Look(ref _five, "colorFive");
        Scribe_Values.Look(ref _six, "colorSix");
    }

    public void Notify_ColorsChanged()
    {
        _thing.Notify_ColorChanged();
    }

    public void SetColorsOn(Material material)
    {
        material.SetColor("_Color", ColorOne);
        material.SetColor("_ColorTwo", ColorTwo);
        material.SetColor("_ColorThree", ColorThree);
        material.SetColor("_ColorFour", ColorFour);
        material.SetColor("_ColorFive", ColorFive);
        material.SetColor("_ColorSix", ColorSix);
    }
}

public class MaskTracker
{
    private Thing _thing;
    private int _maskIndex;
    private int _maskCount;
    
    public int CurMaskID => _maskIndex;
    
    public MaskTracker(Thing thing)
    {
        _thing = thing;
    }
}