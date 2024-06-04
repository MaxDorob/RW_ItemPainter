using TeleCore.Shared;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private void DrawPaletteSelection(Rect inRect)
    {
        inRect = inRect.ContractedBy(1,0);
        Widgets.BeginGroup(inRect);
        
        inRect = inRect.AtZero();
        Widgets.DrawBoxSolid(inRect, TColor.BGDarker);
        
        //Settings bar
        //TODO: Palette group selection dropdown: All/Specific Def
        var topBar = inRect.TopPartPixels(24).Rounded();
        var selRect = inRect.BottomPartPixels(inRect.height - 24).Rounded();
        
        
        
        Widgets.EndGroup();
        
    }
}