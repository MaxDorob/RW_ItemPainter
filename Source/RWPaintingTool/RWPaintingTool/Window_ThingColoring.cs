using System.IO;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public class Window_ThingColoring : Window
{
    private Thing _thing;
    
    //Editor
    private Color[] _colors = new Color[6];
    private Color[] _colorsOld = new Color[6];
    private Rot4 _rotation;

    #region Tweaky

    [TweakValue("_RWIP", 0f, 700f)]
    private static readonly float colorsWidthPx = 448f;

    #endregion
    
    public override void DoWindowContents(Rect inRect)
    {
        var leftPart = inRect.LeftPartPixels(256);
        var rightPart = inRect.RightPartPixels(colorsWidthPx); //;384
        var center = new Rect(leftPart.xMax, leftPart.y, rightPart.x - leftPart.xMax, leftPart.height);
    }

    private void DrawItemPreview(Rect rect)
    {
        //Front
        
        //Sideviews
        
        //Back
    }

    private void DrawThing(Rect rect, Thing thing, Rot4 rot, SixColorSet colors, MaskSelection mask)
    {
        var render = GetRenderData(thing, rot);
        ChangeData(render.Mat, colors, mask, out var oldColors, out var oldMask);
        
        Graphics.DrawTexture(rect, render.Tex, 0, 0, 0, 0, render.Mat);
        
        //Reset
        ChangeData(render.Mat, oldColors, oldMask, out _, out _);
    }

    private (Texture2D Tex, Material Mat) GetRenderData(Thing thing, Rot4 rotation)
    {
        var result = BaseContent.BadMat;
        if (thing is Apparel apparel)
        {
            if (ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, BodyTypeDefOf.Male, out var apparelGraphicRecord))
                result = apparelGraphicRecord.graphic.MatAt(rotation, thing);
        }
        else
        {
            result = thing.Graphic.MatAt(rotation, thing);
        }

        return (result.mainTexture as Texture2D, result);
    }

    private void ChangeData(Material material, SixColorSet colors, MaskSelection mask, out SixColorSet oldColors, out MaskSelection oldMask)
    {
        oldColors = new SixColorSet
        {
            ColorOne = material.GetColor("_Color"),
            ColorTwo = material.GetColor("_ColorTwo"),
            ColorThree = material.GetColor("_ColorThree"),
            ColorFour = material.GetColor("_ColorFour"),
            ColorFive = material.GetColor("_ColorFive"),
            ColorSix = material.GetColor("_ColorSix")
        };
        oldMask = MaskSelection.FromTexture(material.GetTexture("_MaskTex"));
        
        
        material.SetColor("_Color", colors.ColorOne);
        material.SetColor("_ColorTwo", colors.ColorTwo);
        material.SetColor("_ColorThree", colors.ColorThree);
        material.SetColor("_ColorFour", colors.ColorFour);
        material.SetColor("_ColorFive", colors.ColorFive);
        material.SetColor("_ColorSix", colors.ColorSix);
        material.SetTexture("_MaskTex", mask.Texture);
    }
    
    private void DrawColorPicker(Rect rect)
    {
        
    }
}