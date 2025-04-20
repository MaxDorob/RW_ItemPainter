using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    //private void DrawItemPreview(Rect rect)
    //{
    //    var help = Mathf.Floor(rect.height / 3);
    //    var group = rect.LeftPartPixels(help);
    //    var topRect = group.TopPartPixels(help).ContractedBy(5);
    //    var middleRect = group.TopPartPixels(help*2).BottomPartPixels(help).ContractedBy(5);
    //    var bottomRect = group.TopPartPixels(help*3).BottomPartPixels(help).ContractedBy(5);

    //    var maskId = _hoveredMaskIndex ?? _selectedMaskIndex;
        
    //    //Front
    //    Widgets.DrawBox(topRect, 1, BaseContent.GreyTex);
    //    DrawThing(topRect, Rot4.South, _colorsSet, maskId);
        
    //    //Sideviews
    //    Widgets.DrawBox(middleRect, 1, BaseContent.GreyTex);
    //    DrawThing(middleRect, Rot4.East, _colorsSet, maskId);
        
    //    //Back
    //    Widgets.DrawBox(bottomRect, 1, BaseContent.GreyTex);
    //    DrawThing(bottomRect, Rot4.North, _colorsSet, maskId);
    //}
    
    private void DrawThing(Rect rect, Rot4 rot, SixColorSet colors, int maskId)
    {
        //var render = GetRenderData(rot);
        //var mask = new TextureID
        //{
        //    Def = _thing.def,
        //    BodyType = BodyTypeDefOf.Male,
        //    MaskID = maskId,
        //    Rotation = rot
        //};

        //Color oldCol = Color.white;
        //if (_highLightedIndex != -1)
        //{
        //    oldCol = colors[_highLightedIndex];
        //    colors[_highLightedIndex] = ColorFor(_highLightedIndex);
        //}
        
        //ChangeData(render.Mat, colors, mask, out var oldColors, out var oldMask);

        //if (_highLightedIndex != -1)
        //{
        //    colors[_highLightedIndex] = oldCol;
        //    _highLightedIndex = -1;
        //}

        //Graphics.DrawTexture(rect, render.Tex, 0, 0, 0, 0, render.Mat);
        
        ////Reset
        //ChangeData(render.Mat, oldColors, oldMask, out _, out _);
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
    
    //private (Texture2D Tex, Material Mat) GetRenderData(Rot4 rotation)
    //{
    //    if (rotation == Rot4.North)
    //        return (_north.mainTexture as Texture2D, _north)!;
        
    //    if (rotation == Rot4.East)
    //        return (_east.mainTexture as Texture2D, _east)!;
        
    //    if (rotation == Rot4.South)
    //        return (_south.mainTexture as Texture2D, _south)!;
        
    //    return (_west.mainTexture as Texture2D, _west)!;
    //}
    
    private void ChangeData(Material material, SixColorSet colors, TextureID mask, out SixColorSet oldColors, out TextureID oldMask)
    {
        oldColors = new SixColorSet
        {
            colorOne = material.GetColor("_Color"),
            colorTwo = material.GetColor("_ColorTwo"),
            colorThree = material.GetColor("_ColorThree"),
            colorFour = material.GetColor("_ColorFour"),
            colorFive = material.GetColor("_ColorFive"),
            colorSix = material.GetColor("_ColorSix")
        };
        oldMask = MaskManager.IdFromTexture(material.GetTexture("_MaskTex"));
        
        material.SetColor("_Color", colors.colorOne);
        material.SetColor("_ColorTwo", colors.colorTwo);
        material.SetColor("_ColorThree", colors.colorThree);
        material.SetColor("_ColorFour", colors.colorFour);
        material.SetColor("_ColorFive", colors.colorFive);
        material.SetColor("_ColorSix", colors.colorSix);
        material.SetTexture("_MaskTex", MaskManager.GetMask(mask));
    }
}