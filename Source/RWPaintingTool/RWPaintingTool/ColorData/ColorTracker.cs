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

    private bool shouldUseTemp = false;
    private SixColorSet? tempColorSet;

    public SixColorSet? TempColorSet
    {
        get => tempColorSet;
        set
        {
            tempColorSet = value;
            if (value == null)
            {
                shouldUseTemp = false;
            }
        }
    }
    public bool ShouldUseTemp
    {
        get
        {
            return shouldUseTemp && tempColorSet != null;
        }
        set
        {
            if (value && tempColorSet == null)
            {
                Log.Warning("tempColorSet is null and forced to use temp values");
            }
            shouldUseTemp = value;
        }
    }

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

    public Color ColorOne => ShouldUseTemp ? TempColorSet.Value.colorOne : _one ?? _thing.DrawColor;
    public Color ColorTwo => ShouldUseTemp ? TempColorSet.Value.colorTwo : _two ?? _thing.DrawColorTwo;
    public Color ColorThree => ShouldUseTemp ? TempColorSet.Value.colorThree : _three;

    public Color ColorFour => ShouldUseTemp ? TempColorSet.Value.colorFour : _four;
    public Color ColorFive => ShouldUseTemp ? TempColorSet.Value.colorFive : _five;
    public Color ColorSix => ShouldUseTemp ? TempColorSet.Value.colorSix : _six;

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

    public SixColorSet ColorSet
    {
        get
        {
            return new SixColorSet
            {
                colorOne = ColorOne,
                colorTwo = ColorTwo,
                colorThree = ColorThree,
                colorFour = ColorFour,
                colorFive = ColorFive,
                colorSix = ColorSix
            };
        }
        set
        {
            _one = value.colorOne;
            _two = value.colorTwo;
            _three = value.colorThree;
            _four = value.colorFour;
            _five = value.colorFive;
            _six = value.colorSix;
        }
    }
    public void Reset()
    {
        TempColorSet = null;
    }
    public void Commit()
    {
        if (TempColorSet == null)
        {
            return;
        }
        this.SetColors(0, TempColorSet.Value.colorOne);
        this.SetColors(1, TempColorSet.Value.colorTwo);
        this.SetColors(2, TempColorSet.Value.colorThree);
        this.SetColors(3, TempColorSet.Value.colorFour);
        this.SetColors(4, TempColorSet.Value.colorFive);
        this.SetColors(5, TempColorSet.Value.colorSix);
        TempColorSet = null;
        _thing.Notify_ColorChanged();
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

    public string CurrentMaskPath => MaskManager.GetMaskPath(_thing, _maskIndex);
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
    public Texture2D GetMask(bool silent = false)
    {
        return GetMask(Rot4.Invalid, true) ?? GetMask(Rot4.North, silent);
    }
    public Texture2D GetMask(Rot4 rotation, bool silent = false)
    {
        var id = new TextureID
        {
            Def = _thing.def,
            BodyType = BodyType,
            MaskID = CurMaskID,
            Rotation = rotation
        };

        return MaskManager.GetMask(id, silent);
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