﻿using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RWPaintingTool;

public partial class PaintingTool : Dialog_StylingStation
{
    [TweakValue("_RWIP", 0f, 700f)]
    private static float colorsWidthPx = 448f;

    private ColorPicker _colorPicker;


    public PaintingTool(Pawn pawn, Thing stylingStation) : base(pawn, stylingStation)
    {
        SetFor(pawn.apparel.WornApparel.Find(a => a.def.HasModExtension<PaintableExtension>()));
    }
    public override Vector2 InitialSize => new(1400f, 750f);

    public List<Color> AllHairColors { get; private set; }

    public override void DoWindowContents(Rect originalInRect)
    {
        var leftRect = originalInRect.LeftPart(1f / 3f).ContractedBy(Margin);
        var middleRect = originalInRect.RightPart(2f / 3f).LeftHalf().ContractedBy(Margin);
        var rightRect = originalInRect.RightPart(1f / 3f).ContractedBy(Margin);

        DrawLeftRect(leftRect);
        DrawMiddlePart(middleRect);
        DrawRightRect(rightRect);

        return;
        var inRect = originalInRect.LeftPart(0.7f);
        Text.Font = GameFont.Medium;
        Rect rect = new Rect(inRect)
        {
            height = Text.LineHeight * 2f
        };
        Widgets.Label(rect, "StylePawn".Translate().CapitalizeFirst() + ": " + Find.ActiveLanguageWorker.WithDefiniteArticle(this.pawn.Name.ToStringShort, this.pawn.gender, false, true).ApplyTag(TagType.Name, null));
        Text.Font = GameFont.Small;
        inRect.yMin = rect.yMax + 4f;
        Rect rect2 = inRect;
        rect2.width *= 0.3f;
        rect2.yMax -= Dialog_StylingStation.ButSize.y + 4f;
        this.DrawPawn(rect2);
        Rect rect3 = inRect;
        rect3.xMin = rect2.xMax + 10f;
        rect3.yMax -= Dialog_StylingStation.ButSize.y + 4f;
        //this.DrawTabs(rect3);
        curTab = StylingTab.ApparelColor;
        this.DrawApparelColor(rect3);
        this.DrawBottomButtons(inRect);
        if (Prefs.DevMode)
        {
            Widgets.CheckboxLabeled(new Rect(inRect.xMax - 120f, 0f, 120f, 30f), "DEV: Show all", ref this.devEditMode, false, null, null, false, false);
        }

        inRect = originalInRect.RightPart(0.3f);

        Text.Font = GameFont.Small; //Reset font
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.Label(inRect, _thing.LabelCap);
        Text.Anchor = 0;

        var topBar = inRect.TopPartPixels(32);
        var body = inRect.BottomPartPixels(inRect.height - 32);

        var rightPart = body.RightPartPixels(body.width).ContractedBy(5, 0).Rounded();


        Widgets.DrawMenuSection(rightPart);
        DrawColorTool(rightPart.ContractedBy(5).TopPartPixels(200).LeftPartPixels(200));

        var maskRect = rightPart.BottomPartPixels(200).BottomPartPixels(100);
        var paletteRect = rightPart.BottomPartPixels(300).TopPartPixels(200);
        DrawMaskSelection(maskRect);
        Widgets.DrawLineHorizontal(maskRect.x, maskRect.y, maskRect.width, Widgets.WindowBGBorderColor);
        DrawPaletteSelection(paletteRect);
        Widgets.DrawLineHorizontal(paletteRect.x, paletteRect.y, paletteRect.width, Widgets.WindowBGBorderColor);
    }

    private void DrawBottomButtons(Rect inRect)
    {
        if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - Dialog_StylingStation.ButSize.y, Dialog_StylingStation.ButSize.x, Dialog_StylingStation.ButSize.y), "Cancel".Translate(), true, true, true, null))
        {
            this.Reset(true);
            this.Close(true);
        }
        if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 2f - Dialog_StylingStation.ButSize.x / 2f, inRect.yMax - Dialog_StylingStation.ButSize.y, Dialog_StylingStation.ButSize.x, Dialog_StylingStation.ButSize.y), "Reset".Translate(), true, true, true, null))
        {
            this.Reset(true);
            SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
        }
        if (Widgets.ButtonText(new Rect(inRect.xMax - Dialog_StylingStation.ButSize.x, inRect.yMax - Dialog_StylingStation.ButSize.y, Dialog_StylingStation.ButSize.x, Dialog_StylingStation.ButSize.y), "Accept".Translate(), true, true, true, null))
        {
            if (this.pawn.story.hairDef != this.initialHairDef || this.pawn.style.beardDef != this.initialBeardDef || this.pawn.style.FaceTattoo != this.initialFaceTattoo || this.pawn.style.BodyTattoo != this.initialBodyTattoo || this.pawn.story.HairColor != this.desiredHairColor)
            {
                if (!this.DevMode)
                {
                    this.pawn.style.SetupNextLookChangeData(this.pawn.story.hairDef, this.pawn.style.beardDef, this.pawn.style.FaceTattoo, this.pawn.style.BodyTattoo, new Color?(this.desiredHairColor));
                    this.Reset(false);
                    this.pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.UseStylingStation, this.stylingStation), new JobTag?(JobTag.Misc), false);
                }
                else
                {
                    this.pawn.story.HairColor = this.desiredHairColor;
                    this.pawn.style.Notify_StyleItemChanged();
                }
            }
            this.ApplyApparelColors();
            foreach (var apparel in pawn.apparel.WornApparel)
            {
                ColorTrackerDB.GetTracker(apparel)?.Commit();
            }
            this.Close(true);
        }
    }

    
    
    private void DrawApparelColor(Rect rect)
    {
        Rect viewRect = new Rect(rect.x, rect.y, rect.width - 16f, this.viewRectHeight);
        Widgets.BeginScrollView(rect, ref this.apparelColorScrollPosition, viewRect, true);
        int num = 0;
        float num2 = rect.y;
        foreach (Apparel apparel in this.pawn.apparel.WornApparel)
        {
            Color color;
            if (this.apparelColors.TryGetValue(apparel, out color))
            {
                Rect rect2 = new Rect(rect.x, num2, viewRect.width, 92f);
                num2 += rect2.height + 10f;
                if (!this.pawn.apparel.IsLocked(apparel) || this.DevMode)
                {
                    float num3;
                    var colorChanged = Widgets.ColorSelector(rect2, ref color, this.AllColors, out num3, Widgets.GetIconFor(apparel.def, apparel.Stuff, apparel.StyleDef, null), 22, 2, new Action<Color, Rect>(this.ColorSelecterExtraOnGUI));
                    float num4 = rect2.x;
                    if (this.pawn.Ideo != null && !Find.IdeoManager.classicMode)
                    {
                        rect2 = new Rect(num4, num2, 200f, 24f);
                        if (Widgets.ButtonText(rect2, "SetIdeoColor".Translate(), true, true, true, null))
                        {
                            color = this.pawn.Ideo.ApparelColor;
                            SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                        }
                        num4 += 210f;
                    }
                    Pawn_StoryTracker story = this.pawn.story;
                    if (story != null && story.favoriteColor != null)
                    {
                        rect2 = new Rect(num4, num2, 200f, 24f);
                        if (Widgets.ButtonText(rect2, "SetFavoriteColor".Translate(), true, true, true, null))
                        {
                            color = this.pawn.story.favoriteColor.Value;
                            SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                        }
                        num4 += 210f;
                    }
                    rect2 = new Rect(num4, num2, viewRect.xMax - num4, 24f);
                    DrawMaskEditableColors(rect2, apparel);
                    if (!color.IndistinguishableFrom(apparel.DrawColor))
                    {
                        num++;
                    }
                    if (colorChanged)
                    {
                        if (_thing != apparel)
                        {
                            SetFor(apparel);
                        }

                        Notify_ColorChanged(color, _curColorIndex);
                    }
                }
                else
                {
                    Widgets.ColorSelectorIcon(new Rect(rect2.x, rect2.y, 88f, 88f), apparel.def.uiIcon, color, false);
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Rect rect3 = rect2;
                    rect3.x += 100f;
                    Widgets.Label(rect3, "ApparelLockedCannotRecolor".Translate(this.pawn.Named("PAWN"), apparel.Named("APPAREL")).Colorize(ColorLibrary.RedReadable));
                    Text.Anchor = TextAnchor.UpperLeft;
                }
                num2 += 34f;
            }
        }
        if (false && this.pawn.Spawned)
        {
            if (num > 0)
            {
                this.DrawDyeRequirement(rect, ref num2, num);
            }
            if (this.pawn.Map.resourceCounter.GetCount(ThingDefOf.Dye) < num)
            {
                Rect rect4 = new Rect(rect.x, num2, rect.width - 16f - 10f, 60f);
                Color color2 = GUI.color;
                GUI.color = ColorLibrary.RedReadable;
                Widgets.Label(rect4, "NotEnoughDye".Translate() + " " + "NotEnoughDyeWillRecolorApparel".Translate());
                GUI.color = color2;
                num2 += rect4.height;
            }
        }
        if (Event.current.type == EventType.Layout)
        {
            this.viewRectHeight = num2 - rect.y;
        }
        Widgets.EndScrollView();
    }


}