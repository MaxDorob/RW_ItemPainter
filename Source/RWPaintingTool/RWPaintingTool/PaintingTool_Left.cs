using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWPaintingTool
{
    public partial class PaintingTool
    {
        public void DrawLeftRect(Rect inRect)
        {
            DrawPaletteSelection(inRect.TopPart(0.8f));
            DrawPalleteButtons(inRect.BottomPart(0.2f));
        }
        IList<IPalette> cachedPalettes;
        private IList<IPalette> Palettes
        {
            get
            {
                if (cachedPalettes == null)
                {
                    cachedPalettes = DefDatabase<PaletteDef>.AllDefsListForReading.Cast<IPalette>().Union(Current.Game.GetComponent<CustomPaletteStore>().customPalettes).ToList();
                }
                return cachedPalettes;
            }
        }

        private void DrawPaletteSelection(Rect inRect)
        {
            inRect = inRect.ContractedBy(1, 0);
            Widgets.BeginGroup(inRect);

            inRect = inRect.AtZero();
            Widgets.DrawBoxSolid(inRect, Colors.BGDarker);

            //Settings bar
            //TODO: Palette group selection dropdown: All/Specific Def
            var topBar = inRect.TopPartPixels(24).Rounded();
            var selRect = inRect.BottomPartPixels(inRect.height - 24).Rounded();

            if (Palettes.Count == 0) return;

            Vector2 xy = inRect.position;
            var widthPer = inRect.width;
            var heightPer = 48;
            for (var i = 0; i < Palettes.Count; i++)
            {
                var palette = Palettes[i];
                var rect = new Rect(xy.x, xy.y, widthPer, heightPer).Rounded();
                DrawPaletteOption(rect, palette);

                if (lastSelectedPalette == palette || Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }

                if (Widgets.ButtonInvisible(rect))
                {
                    if (lastSelectedPalette == palette)
                    {
                        SetPalette(palette.Palette);
                    }
                    lastSelectedPalette = palette;
                }

                xy = new Vector2(inRect.x, xy.y + heightPer + Margin / 2);

            }

            Widgets.EndGroup();
        }

        private void DrawPaletteOption(Rect rect, IPalette palette)
        {
            DrawHover(rect);
            rect = rect.ContractedBy(2).Rounded();

            var label = palette.Label;
            var labelSize = Text.CalcSize(label);
            var leftOver = rect.height - labelSize.y;
            var labelRect = rect.TopPartPixels(labelSize.y).Rounded();
            var colorRect = rect.BottomPartPixels(leftOver).Rounded();

            if (palette is CustomPalette customPalette)
            {
                customPalette.Label = Widgets.TextField(labelRect, label);
            }
            else
            {
                Widgets.Label(labelRect, label);
            }

            var parts = colorRect.width / 6;
            var col1R = new Rect(colorRect.x, colorRect.y, parts, colorRect.height);
            var col2R = new Rect(colorRect.x + parts, colorRect.y, parts, colorRect.height);
            var col3R = new Rect(colorRect.x + parts * 2, colorRect.y, parts, colorRect.height);
            var col4R = new Rect(colorRect.x + parts * 3, colorRect.y, parts, colorRect.height);
            var col5R = new Rect(colorRect.x + parts * 4, colorRect.y, parts, colorRect.height);
            var col6R = new Rect(colorRect.x + parts * 5, colorRect.y, parts, colorRect.height);

            Widgets.DrawBoxSolid(col1R, palette.Palette.colorOne);
            Widgets.DrawBoxSolid(col2R, palette.Palette.colorTwo);
            Widgets.DrawBoxSolid(col3R, palette.Palette.colorThree);
            Widgets.DrawBoxSolid(col4R, palette.Palette.colorFour);
            Widgets.DrawBoxSolid(col5R, palette.Palette.colorFive);
            Widgets.DrawBoxSolid(col6R, palette.Palette.colorSix);
        }

        public static void DrawHover(Rect rect)
        {
            var selBorder = Mouse.IsOver(rect) ? borderWhite05 : borderWhite025;
            Widgets.DrawBox(rect, 2, selBorder);
        }

        public static void DrawHoverSelection(Rect rect, bool isSelected)
        {
            var selBorder = isSelected ? BaseContent.WhiteTex : borderWhite025;
            selBorder = Mouse.IsOver(rect) ? borderWhite05 : selBorder;
            Widgets.DrawBox(rect, 2, selBorder);
        }
        private IPalette lastSelectedPalette;
        public void DrawPalleteButtons(Rect inRect)
        {
            inRect = inRect.ContractedBy(Margin);


            var addButtonRect = inRect.LeftPart(1f / 3f).ContractedBy(Margin);
            addButtonRect.height = ButSize.y;
            addButtonRect.center = new Vector2(addButtonRect.center.x, inRect.center.y);

            if (Widgets.ButtonText(addButtonRect, "RWPaintingTool_Add".Translate()))
            {
                Current.Game.GetComponent<CustomPaletteStore>().customPalettes.Add(new CustomPalette() { Palette = _tracker.ColorSet, Label = "New palette"});
                cachedPalettes = null;
            }


            var saveButtonRect = inRect.RightPart(2f / 3f).LeftHalf().ContractedBy(Margin);
            saveButtonRect.height = ButSize.y;
            saveButtonRect.center = new Vector2(saveButtonRect.center.x, inRect.center.y);
            if (Widgets.ButtonText(saveButtonRect, "RWPaintingTool_Save".Translate()))
            {

            }



            var removeButtonRect = inRect.RightPart(1f / 3f).ContractedBy(Margin);
            removeButtonRect.height = ButSize.y;
            removeButtonRect.center = new Vector2(removeButtonRect.center.x, inRect.center.y);
            if (Widgets.ButtonText(removeButtonRect, "RWPaintingTool_Remove".Translate()))
            {
                if (lastSelectedPalette is CustomPalette paletteToRemove)
                {
                    Current.Game.GetComponent<CustomPaletteStore>().customPalettes.Remove(paletteToRemove);
                    cachedPalettes = null;
                    lastSelectedPalette = null;
                }
            }
        }
    }
}
