using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private void DrawItemPreview(Rect rect)
    {
        var help = Mathf.Floor(rect.height / 3);
        var group = rect.LeftPartPixels(help);
        var topRect = group.TopPartPixels(help).ContractedBy(5);
        var middleRect = group.TopPartPixels(help*2).BottomPartPixels(help).ContractedBy(5);
        var bottomRect = group.TopPartPixels(help*3).BottomPartPixels(help).ContractedBy(5);
        
        //Front
        Widgets.DrawBox(topRect, 1, BaseContent.GreyTex);
        DrawThing(topRect, _thing, Rot4.South, _colorsSet, 0);
        
        //Sideviews
        Widgets.DrawBox(middleRect, 1, BaseContent.GreyTex);
        DrawThing(middleRect, _thing, Rot4.East, _colorsSet, 0);
        
        //Back
        Widgets.DrawBox(bottomRect, 1, BaseContent.GreyTex);
        DrawThing(bottomRect, _thing, Rot4.North, _colorsSet, 0);
    }
    
    private void DrawThing(Rect rect, Thing thing, Rot4 rot, SixColorSet colors, int maskId)
    {
        var render = GetRenderData(thing, rot);
        var mask = new TextureID
        {
            Def = thing.def,
            BodyType = BodyTypeDefOf.Male,
            MaskID = maskId,
            Rotation = rot
        };

        Color oldCol = Color.white;
        if (_highLightedIndex != -1)
        {
            oldCol = colors[_highLightedIndex];
            colors[_highLightedIndex] = ColorFor(_highLightedIndex);
        }
        
        ChangeData(render.Mat, colors, mask, out var oldColors, out var oldMask);

        if (_highLightedIndex != -1)
        {
            colors[_highLightedIndex] = oldCol;
            _highLightedIndex = -1;
        }

        Graphics.DrawTexture(rect, render.Tex, 0, 0, 0, 0, render.Mat);
        
        //Reset
        ChangeData(render.Mat, oldColors, oldMask, out _, out _);
    }
    
    private static Color ColorFor(int index)
    {
        return index switch
        {
            0 => Color.red,
            1 => Color.green,
            2 => Color.blue,
            3 => Color.yellow,
            4 => Color.cyan,
            5 => Color.magenta,
            _ => Color.white
        };
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
    
    private void ChangeData(Material material, SixColorSet colors, TextureID mask, out SixColorSet oldColors, out TextureID oldMask)
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
        oldMask = MaskManager.IdFromTexture(material.GetTexture("_MaskTex"));

        material.SetColor("_Color", colors.ColorOne);
        material.SetColor("_ColorTwo", colors.ColorTwo);
        material.SetColor("_ColorThree", colors.ColorThree);
        material.SetColor("_ColorFour", colors.ColorFour);
        material.SetColor("_ColorFive", colors.ColorFive);
        material.SetColor("_ColorSix", colors.ColorSix);
        material.SetTexture("_MaskTex", MaskManager.GetMask(mask));
    }
}