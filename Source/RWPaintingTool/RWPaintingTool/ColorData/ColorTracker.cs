using RimWorld;
using TeleCore.Loader;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

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

    internal void SetColorsOn(Graphic graphic)
    {
        if (graphic is Graphic_Multi multi)
        {
            SetColorsOn(multi.MatEast);
            SetColorsOn(multi.MatWest);
            SetColorsOn(multi.MatNorth);
            SetColorsOn(multi.MatSouth);    
            return;
        }
        
        //
        SetColorsOn(graphic.MatSingle);
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
    private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");

    public int CurMaskID => _maskIndex;

    public BodyTypeDef? BodyType
    {
        get
        {
            if (_thing is Apparel apparel)
            {
                if (apparel.Wearer != null)
                {
                    return apparel.Wearer.story.bodyType;
                }
                return BodyTypeDefOf.Male;
            }

            return null;
        }
    }
    
    public MaskTracker(Thing thing)
    {
        _thing = thing;
    }

    internal void SetMaskID(int selectedMaskIndex)
    {
        _maskIndex = selectedMaskIndex;
    }
    
    public void SetMaskOn(Graphic graphic)
    {
        if (graphic is Graphic_Multi multi)
        {
            SetMaskOn(multi.MatEast, Rot4.East);
            SetMaskOn(multi.MatWest, Rot4.West);
            SetMaskOn(multi.MatNorth, Rot4.North);
            SetMaskOn(multi.MatSouth, Rot4.South);
            return;
        }

        //
        SetMaskOn(graphic.MatSingle, Rot4.North);
    }

    private void SetMaskOn(Material material, Rot4 rotation)
    {
        var id = new TextureID
        {
            Def = _thing.def,
            BodyType = BodyType,
            MaskID = CurMaskID,
            Rotation = rotation
        };
            
        var mask = MaskManager.GetMask(id);
        material.SetTexture(MaskTex, mask);
    }
}