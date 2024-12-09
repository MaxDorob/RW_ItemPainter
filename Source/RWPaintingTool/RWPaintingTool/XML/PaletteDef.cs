using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public class PaletteGroupDef : Def
{
    public List<PaletteDef> palettes;
}

public class PaletteDef : Def
{
    public Palette palette;
}

public struct Palette
{
    public Color colorOne;
    public Color colorTwo;
    public Color colorThree;
    public Color colorFour;
    public Color colorFive;
    public Color colorSix;

    public static implicit operator SixColorSet(Palette palette)
    {
        return new SixColorSet(palette.colorOne, palette.colorTwo, palette.colorThree, palette.colorFour, palette.colorFive, palette.colorSix);
    }
}
