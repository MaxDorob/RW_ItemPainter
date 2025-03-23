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
        public void DrawMiddlePart(Rect inRect)
        {

            inRect.SplitHorizontally(ButSize.y, out var apparelButtonRect, out inRect);
            var apparelLabel = "Some apparel";
            if (Widgets.ButtonText(apparelButtonRect, apparelLabel))
            {

            }
            inRect.SplitHorizontallyWithMargin(out var pawnRect, out var bottomButtonsRect, out _, bottomHeight: ButSize.y * 3 + Margin * 2);
            DrawPawn(pawnRect);


            Rect rect2 = bottomButtonsRect;
            var y = rect2.y;
            Widgets.CheckboxLabeled(new Rect(rect2.x, y, rect2.width, ButSize.y), "ShowHeadgear".Translate(), ref this.showHeadgear, false, null, null, false, false);
            y += ButSize.y;
            Widgets.CheckboxLabeled(new Rect(rect2.x, y, rect2.width, ButSize.y), "ShowApparel".Translate(), ref this.showClothes, false, null, null, false, false);
            y += ButSize.y + Margin;
            Widgets.ButtonText(new Rect(rect2.x, y, rect2.width, ButSize.y), "Accept".Translate());


        }

        Rot4 rotation = Rot4.South;
        private void DrawPawn(Rect rect)
        {
            //Widgets.BeginGroup(rect);

            Rect position = rect.ContractedBy(Margin);
            RenderTexture image = PortraitsCache.Get(this.pawn, new Vector2(position.width, position.height), rotation, Dialog_StylingStation.PortraitOffset, 1.1f, true, true, this.showHeadgear, this.showClothes, this.apparelColors, new Color?(this.desiredHairColor), true, null);
            GUI.DrawTexture(position, image);
            
            var leftArrowRect = new Rect(rect.xMin, rect.center.y, Margin, Margin);
            if (Widgets.ButtonText(leftArrowRect, "<", false))
            {
                rotation = new Rot4(rotation.AsInt - 1);
            }

            var rightArrowRect = new Rect(rect.xMax - Margin, rect.center.y, Margin, Margin);
            if (Widgets.ButtonText(rightArrowRect, ">", false))
            {
                rotation = new Rot4(rotation.AsInt + 1);
            }
            

            //Widgets.EndGroup();
        }
    }
}
