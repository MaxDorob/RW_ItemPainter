using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RWPaintingTool;

[StaticConstructorOnStartup]
public partial class PaintingTool : Window
{
    [TweakValue("_RWIP", 0f, 700f)]
    private static float colorsWidthPx = 448f;

    private ColorPicker _colorPicker;
    private static Texture2D borderWhite05 = SolidColorMaterials.NewSolidColorTexture(Colors.White05);
    private static Texture2D borderWhite025 = SolidColorMaterials.NewSolidColorTexture(Colors.White025);

    public PaintingTool(Pawn pawn) : base()
    {
        this.pawn = pawn;
        _colorPicker = new ColorPicker();
        _colorPicker.ColorChanged += Notify_ColorChanged;
        foreach (var colorTracker in PaintableApparel.Select(ColorTrackerDB.GetTracker))
        {
            colorTracker.TempColorSet = colorTracker.ColorSet;
        }
        SetFor(pawn.apparel.WornApparel.Find(a => a.def.HasModExtension<PaintableExtension>()));

    }
    public override Vector2 InitialSize => new(1400f, 750f);

    public override void DoWindowContents(Rect originalInRect)
    {
        var leftRect = originalInRect.LeftPart(1f / 3f).ContractedBy(Margin);
        var middleRect = originalInRect.RightPart(2f / 3f).LeftHalf().ContractedBy(Margin);
        var rightRect = originalInRect.RightPart(1f / 3f).ContractedBy(Margin);

        DrawLeftRect(leftRect);
        DrawMiddlePart(middleRect);
        DrawRightRect(rightRect);
    }
}