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
    public Color colorOne;
    public Color colorTwo;
    public Color colorThree;
    
    public Palette Palette => new Palette
    {
        colorOne = colorOne,
        colorTwo = colorTwo,
        colorThree = colorThree
    };
}

public struct Palette
{
    public Color colorOne;
    public Color colorTwo;
    public Color colorThree;
}
