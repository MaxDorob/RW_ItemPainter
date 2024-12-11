using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private IReadOnlyList<PaletteDef> Palettes => DefDatabase<PaletteDef>.AllDefsListForReading;
    
    private void DrawPaletteSelection(Rect inRect)
    {
        inRect = inRect.ContractedBy(1,0);
        Widgets.BeginGroup(inRect);
        
        inRect = inRect.AtZero();
        Widgets.DrawBoxSolid(inRect, Colors.BGDarker);
        
        //Settings bar
        //TODO: Palette group selection dropdown: All/Specific Def
        var topBar = inRect.TopPartPixels(24).Rounded();
        var selRect = inRect.BottomPartPixels(inRect.height - 24).Rounded();

        if (Palettes.Count == 0) return;

        Vector2 xy = inRect.position;
        var widthPer = inRect.width / 4;
        var heightPer = 40;
        for (var i = 0; i < Palettes.Count; i++)
        {
            var palette = Palettes[i];
            var rect = new Rect(xy.x, xy.y, widthPer, heightPer).Rounded();
            DrawPaletteOption(rect, palette);

            var oldset = _colorsSet;
            if (Mouse.IsOver(rect))
            {
                _colorsSet = palette.palette;
            }
            _colorsSet = oldset;

            if (Widgets.ButtonInvisible(rect))
            {
                SetPalette(palette.palette);
            }
            
            //Increment x and wrap once reaches width
            xy.x += widthPer;
            if (i != 0 && i % 4 == 0)
            {
                xy = new Vector2(inRect.x, xy.y + heightPer);
            }
        }

        Widgets.EndGroup();
    }

    private void DrawPaletteOption(Rect rect, PaletteDef palette)
    {
        DrawHover(rect);
        rect = rect.ContractedBy(2).Rounded();
        
        var label = palette.LabelCap;
        var labelSize = Text.CalcSize(label);
        var leftOver = rect.height - labelSize.y;
        var labelRect = rect.TopPartPixels(labelSize.y).Rounded();
        var colorRect = rect.BottomPartPixels(leftOver).Rounded();
        
        Widgets.Label(labelRect, label);

        var parts = colorRect.width / 6;
        var col1R = new Rect(colorRect.x, colorRect.y, parts, colorRect.height);
        var col2R = new Rect(colorRect.x + parts, colorRect.y, parts, colorRect.height);
        var col3R = new Rect(colorRect.x + parts * 2, colorRect.y, parts, colorRect.height);
        var col4R = new Rect(colorRect.x + parts * 3, colorRect.y, parts, colorRect.height);
        var col5R = new Rect(colorRect.x + parts * 4, colorRect.y, parts, colorRect.height);
        var col6R = new Rect(colorRect.x + parts * 5, colorRect.y, parts, colorRect.height);
        
        Widgets.DrawBoxSolid(col1R, palette.palette.colorOne);
        Widgets.DrawBoxSolid(col2R, palette.palette.colorTwo);
        Widgets.DrawBoxSolid(col3R, palette.palette.colorThree);
        Widgets.DrawBoxSolid(col4R, palette.palette.colorFour);
        Widgets.DrawBoxSolid(col5R, palette.palette.colorFive);
        Widgets.DrawBoxSolid(col6R, palette.palette.colorSix);
    }

    public static void DrawHover(Rect rect)
    {
        var selBorder = Mouse.IsOver(rect)? borderWhite05 : borderWhite025;
        Widgets.DrawBox(rect, 2, selBorder);
    }
    
    public static void DrawHoverSelection(Rect rect, bool isSelected)
    {
        var selBorder = isSelected? BaseContent.WhiteTex : borderWhite025;
        selBorder = Mouse.IsOver(rect)? borderWhite05 : selBorder;
        Widgets.DrawBox(rect, 2, selBorder);
    }
}