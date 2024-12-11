using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public class ColorTracker : IExposable
{
    private Thing _thing;
    
    private Color? _one; 
    private Color? _two;
    private Color _three = Color.white;
    private Color _four = Color.white;
    private Color _five = Color.white;
    private Color _six = Color.white;

    public ColorTracker(Thing thing, PaintableExtension extension)
    {
        _thing = thing;
        var palette = extension.paletteDef != null ? extension.paletteDef.palette : extension.defaultPalette;
        if (palette is { } pal)
        {
            SetColors(0, pal.colorOne);
            SetColors(1, pal.colorTwo);
            SetColors(2, pal.colorThree);
            SetColors(3, pal.colorFour);
            SetColors(4, pal.colorFive);
            SetColors(5, pal.colorSix);
        }
        //_one = _two = _three = _four = _five = _six = Color.white;
    }
    public ColorTracker() { }
    internal Color? ColorOneNullable => _one;
    internal Color? ColorTwoNullable => _two;

    public Color ColorOne => _one ?? _thing.DrawColor;
    public Color ColorTwo => _two?? _thing.DrawColorTwo;
    public Color ColorThree => _three;
    public Color ColorFour => _four;
    public Color ColorFive => _five;
    public Color ColorSix => _six;
    
    public void SetColors(int index, Color color)
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

    internal void SetColorssOn(Graphic graphic)
    {
        if (graphic is Graphic_Multi multi)
        {
            SetColorssOn(multi.MatEast);
            SetColorssOn(multi.MatWest);
            SetColorssOn(multi.MatNorth);
            SetColorssOn(multi.MatSouth);    
            return;
        }
        
        //
        SetColorssOn(graphic.MatSingle);
    }
    
    public void SetColorssOn(Material material)
    {
        material.SetColor("_Color", ColorOne);
        material.SetColor("_ColorTwo", ColorTwo);
        material.SetColor("_ColorThree", ColorThree);
        material.SetColor("_ColorFour", ColorFour);
        material.SetColor("_ColorFive", ColorFive);
        material.SetColor("_ColorSix", ColorSix);
    }
}

public class MaskTracker : IExposable
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

    public MaskTracker() { }
    internal void SetMaskID(int selectedMaskIndex)
    {
        _maskIndex = selectedMaskIndex;
    }
    
    public void SetMaskOn(Graphic graphic)
    {
        if (graphic is Graphic_Multi multi)
        {
            if (multi.eastFlipped)
                SetMaskOn(multi.MatWest, Rot4.West);
            
            if (multi.westFlipped)
                SetMaskOn(multi.MatEast, Rot4.East);
            
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

    public void ExposeData()
    {
        Scribe_References.Look(ref _thing, "thing");
        Scribe_Values.Look(ref _maskIndex, "maskIndex");
        Scribe_Values.Look(ref _maskCount, "maskCount");
    }
}