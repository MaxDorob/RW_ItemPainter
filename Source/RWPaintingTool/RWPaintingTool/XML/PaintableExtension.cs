using System.Collections.Generic;
using Verse;

namespace RWPaintingTool;

public class PaintableExtension : DefModExtension
{
    public int maskCount;
    public int beginIndex = 0;
    public int colors = 3;
    public string maskPath;

    public List<SixColorSet> defaultPalettes;
    public PaletteDef paletteDef;
}