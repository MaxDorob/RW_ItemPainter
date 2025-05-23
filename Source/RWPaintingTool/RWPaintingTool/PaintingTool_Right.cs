﻿using RimWorld;
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
            if (Widgets.ButtonText(applyButtonRect.LeftPart(2f / 3f), "Reset".Translate()))
            {
                ColorTracker.TempColorSet = ColorTracker.ColorSet;
                _colorPicker.SetColors(ColorTracker.TempColorSet.Value[_curColorIndex]);
            }
            if (Widgets.ButtonText(applyButtonRect.RightPart(1f / 3f), "RWPT_ResetCurrent".Translate()))
            {
                _colorPicker.SetColors(ColorTracker.ColorSet[_curColorIndex]);
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


            var res = _colorPicker.Draw(colorPickerRect.position);
            var paletteRect = new Rect(res.xMax, res.y, rect.width - res.width, res.height);

            GUI.color = Color.red;
            Widgets.DrawHighlight(res);
            GUI.color = Color.blue;
            Widgets.DrawHighlight(paletteRect);
            GUI.color = Color.white;
            return;
        }

        private /*static*/ void DrawMaskEditableColors(Rect colorSelectRect, Apparel apparel)
        {
            var tracker = ColorTrackerDB.GetTracker(apparel);
            var colorsSet = tracker?.TempColorSet ?? tracker?.ColorSet ?? new SixColorSet() { colorOne = apparelColors[apparel] };

            var colorSize = ColorPicker.DefaultSize;
            var colorSelWidth = colorSize.y / 6;
            var subDiv = new RectDivider(colorSelectRect.ContractedBy(0, 2.5f), colorSelectRect.GetHashCode());
            GraphicsPatches.CurThing = apparel;
            ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, pawn?.story?.bodyType, out var graphicRecord);
            GraphicsPatches.CurThing = apparel;
            for (int i = 0; i < 6; i++)
            {
                var color = colorsSet[i];
                if (color == default(Color))
                {
                    continue;
                }
                var colors = new Color[6];
                colors[i] = color;
                GraphicsPatches.Colors = colors;
                var graphic = graphicRecord.graphic.GetColoredVersion(ShaderDB.CutoutMultiMask, default, default);
                var mat = graphic.MatAt(rotation);
                var colorDiv = subDiv.NewCol(colorSelWidth - 5, HorizontalJustification.Left, 5);
                var colorRect = colorDiv.Rect.Rounded();
                var isSelected = i == _curColorIndex && _thing == apparel;
                var mouseOver = colorRect.Contains(Event.current.mousePosition);
                var isHighlighted = isSelected || mouseOver;

                mat = RWPT_MaterialPool.MatFrom(new RWPT_MaterialRequest(mat.mainTexture, mat.shader, mat.GetMaskTexture(), colors));

                GenUI.DrawTextureWithMaterial(colorRect, mat.mainTexture, mat);
                GraphicsPatches.Colors = null;
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
            GraphicsPatches.CurThing = null;
        }
    }
}
