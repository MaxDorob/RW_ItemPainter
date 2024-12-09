using Verse;

namespace RWPaintingTool;

public class PaintableExtension : DefModExtension
{
    public int maskCount;
    public int beginIndex = 0;
    public int colors = 3;
    public string maskPath;

    public Palette? defaultPalette;
    public PaletteDef paletteDef;
}