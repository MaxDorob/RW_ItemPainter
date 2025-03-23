using RimWorld;
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
        private void DrawRightRect(Rect inRect)
        {
            inRect.SplitHorizontally(ButSize.y, out var applyButtonRect, out inRect);
            if (Widgets.ButtonText(applyButtonRect, "Reset".Translate()))
            {

            }

            inRect.SplitHorizontally(ButSize.y, out var masksRect, out var colorToolRect);


            DrawMaskEditableColors(masksRect, _thing as Apparel);
            DrawColorTool(colorToolRect);

        }
        private void DrawColorTool(Rect rect)
        {

            rect = rect.Rounded();
            var colorSize = ColorPicker.DefaultSize;
            var subRect = rect.LeftPartPixels(colorSize.x);
            var colorPickerRect = subRect.RightPartPixels(colorSize.x);

            //Widgets.DrawHighlight(subRect);
            //Widgets.DrawHighlight(colorPickerRect);
            //Widgets.DrawHighlight(colorSelectRect);

            var res = _colorPicker.Draw(colorPickerRect.position);
            var paletteRect = new Rect(res.xMax, res.y, rect.width - res.width, res.height);

            GUI.color = Color.red;
            Widgets.DrawHighlight(res);
            GUI.color = Color.blue;
            Widgets.DrawHighlight(paletteRect);
            GUI.color = Color.white;
            return;
            //TODO: Improve hashcode perf
            //var subDiv = new RectDivider(colorSelectRect.ContractedBy(0, 2.5f), colorSelectRect.GetHashCode());

            ////TODO: Get actual available colors (detect or read from extension)
            //for (int i = 0; i < 6; i++)
            //{
            //    var colorDiv = subDiv.NewRow(colorSelWidth - 5, VerticalJustification.Top, 5);
            //    var colorRect = colorDiv.Rect.Rounded();
            //    var color = _colorsSet[i];
            //    var isSelected = i == _curColorIndex;
            //    var mouseOver = colorRect.Contains(Event.current.mousePosition);
            //    var isHighlighted = isSelected || mouseOver;
            //    Widgets.DrawBoxSolid(colorRect, color);
            //    if (mouseOver)
            //    {
            //        _highLightedIndex = i;
            //    }
            //    if (isHighlighted)
            //    {
            //        Widgets.DrawBox(colorRect, 1);
            //    }
            //    if (Widgets.ButtonInvisible(colorRect))
            //    {
            //        _curColorIndex = i;
            //        _colorPicker.SetColors(color);
            //    }
            //}
        }

        private /*static*/ void DrawMaskEditableColors(Rect colorSelectRect, Apparel apparel)
        {
            var colorsSet = ColorTrackerDB.GetTracker(apparel)?.ColorSet ?? new SixColorSet() { ColorOne = apparelColors[apparel] };

            var colorSize = ColorPicker.DefaultSize;
            var colorSelWidth = colorSize.y / 6;
            var subDiv = new RectDivider(colorSelectRect.ContractedBy(0, 2.5f), colorSelectRect.GetHashCode());
            for (int i = 0; i < 6; i++)
            {
                var color = colorsSet[i];
                if (color == default(Color))
                {
                    continue;
                }
                var colorDiv = subDiv.NewCol(colorSelWidth - 5, HorizontalJustification.Left, 5);
                var colorRect = colorDiv.Rect.Rounded();
                var isSelected = i == _curColorIndex && _thing == apparel;
                var mouseOver = colorRect.Contains(Event.current.mousePosition);
                var isHighlighted = isSelected || mouseOver;
                Widgets.DrawBoxSolid(colorRect, color);
                Widgets.Label(colorRect.RightHalf(), (i + 1).ToString());
                if (mouseOver)
                {
                    _highLightedIndex = i;
                }
                if (isHighlighted)
                {
                    Widgets.DrawBox(colorRect, 1);
                }
                if (Widgets.ButtonInvisible(colorRect))
                {
                    if (_thing != apparel)
                    {
                        SetFor(apparel);
                    }
                    _curColorIndex = i;
                    _colorPicker.SetColors(color);
                }
            }
        }
    }
}
