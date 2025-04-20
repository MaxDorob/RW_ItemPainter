using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public interface IPalette
{
    public string Label { get; }
    public SixColorSet Palette { get; }
}

public class PaletteGroupDef : Def
{
    public List<PaletteDef> palettes;
}

public class PaletteDef : Def, IPalette
{
    public SixColorSet palette;

    public SixColorSet Palette => palette;
    public string Label => this.LabelCap;
}

