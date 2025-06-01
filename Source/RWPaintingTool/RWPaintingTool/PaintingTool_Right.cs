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
            if (_thing == null || _thing.def == null) return;

            inRect.SplitHorizontally(ButSize.y, out var applyButtonRect, out inRect);

            // Draw reset buttons
            if (Widgets.ButtonText(applyButtonRect.LeftPart(2f / 3f), "Reset".Translate()))
            {
                ColorTracker.TempColorSet = ColorTracker.ColorSet;
                _colorPicker.SetColors(ColorTracker.TempColorSet.Value[_curColorIndex]);
            }
            if (Widgets.ButtonText(applyButtonRect.RightPart(1f / 3f), "RWPT_ResetCurrent".Translate()))
            {
                _colorPicker.SetColors(ColorTracker.ColorSet[_curColorIndex]);
            }

            inRect.SplitHorizontally(ButSize.y, out var masksRect, out inRect);
            DrawMaskEditableColors(masksRect, _thing as Apparel);

            inRect.SplitHorizontally(ColorPicker.DefaultSize.y, out var colorToolRect, out var maskSelectionRect);
            DrawColorTool(colorToolRect);

            var paintableExt = _thing.def.GetModExtension<PaintableExtension>();
            if (paintableExt?.maskCount > 0)
            {
                DrawMaskSelection(maskSelectionRect, paintableExt);
            }
        }

        private void DrawMaskSelection(Rect rect, PaintableExtension ext)
        {
            var maskTracker = ColorTrackerDB.GetMaskTracker(_thing);
            if (maskTracker == null || !(_thing is Apparel apparel)) return;
            if (!ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, pawn?.story?.bodyType, out var baseRecord)) return;

            float rowHeight = 60f;
            float spacing = 5f;
            float buttonWidth = 40f;
            float previewSize = rowHeight;

            Rect rowRect = new Rect(rect.x, rect.y, rect.width, rowHeight);
            int originalMaskID = maskTracker.CurMaskID;

            for (int i = 0; i < ext.maskCount; i++)
            {
                if (rowRect.yMax > rect.yMax) break;

                var buttonRect = new Rect(rowRect.x, rowRect.y, buttonWidth, rowHeight);
                var previewRect = new Rect(buttonRect.xMax + spacing, rowRect.y, previewSize, previewSize);

                bool isSelected = i == originalMaskID;
                if (isSelected)
                    Widgets.DrawHighlightSelected(rowRect);
                else if (Mouse.IsOver(rowRect))
                    Widgets.DrawHighlight(rowRect);

                if (Widgets.ButtonText(buttonRect, i.ToString()))
                {
                    maskTracker.SetMaskID(i);
                    UpdateCurrentMaskWithTracker();
                    originalMaskID = i;
                }

                int prevMaskID = maskTracker.CurMaskID;

                maskTracker.SetMaskID(i);
                if (ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, pawn?.story?.bodyType, out var graphicRecord))
                {
                    maskTracker.SetMaskOn(graphicRecord.graphic);
                    GraphicsPatches.CurThing = _thing;
                    var mat = graphicRecord.graphic.MatAt(rotation);
                    GenUI.DrawTextureWithMaterial(previewRect.Rounded(), mat.mainTexture, mat);
                    GraphicsPatches.CurThing = null;
                }

                maskTracker.SetMaskID(prevMaskID);
                UpdateCurrentMaskWithTracker();

                rowRect.y += rowHeight + spacing;
            }
        }

        private void UpdateCurrentMaskWithTracker()
        {
            if (_thing == null) return;

            var maskTracker = ColorTrackerDB.GetMaskTracker(_thing);
            if (maskTracker == null) return;

            if (_thing is Apparel apparel)
            {
                if (ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, apparel.Wearer?.story?.bodyType, out var graphicRecord))
                {
                    maskTracker.SetMaskOn(graphicRecord.graphic);
                    apparel.Notify_ColorChanged();
                }
            }
            else
            {
                Graphic graphic = _thing.Graphic;
                if (graphic != null)
                {
                    maskTracker.SetMaskOn(graphic);
                    _thing.DirtyMapMesh(_thing.Map);
                }
            }
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
