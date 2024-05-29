using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private void DrawPaletteSelection(Rect inRect)
    {
        //Settings bar
        //TODO: Palette group selection dropdown: All/Specific Def
        var topBar = inRect.TopPartPixels(24).Rounded();
        var selRect = inRect.BottomPartPixels(inRect.height - 24).Rounded();
        
    }
}