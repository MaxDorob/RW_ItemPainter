using System.Collections.Generic;
using System.IO;
using LudeonTK;
using RimWorld;
using TeleCore.AssetLoader;
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
    
    SixColorSet _colorsSet;
    
    private int colorIndex = 0;


    private ColorPicker _colorPicker;

    #region Tweaky

    [TweakValue("_RWIP", 0f, 700f)]
    private static readonly float colorsWidthPx = 448f;

    #endregion

    public override Vector2 InitialSize => new(950f, 750f);

    private static GUIStyle style;
    
    public Window_ThingColoring(Thing thing)
    {
        //TODO: Make custom style library for telecore.gui
        style = new GUIStyle(GUI.skin.textField);
        style.normal.background = style.active.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom");
        style.hover.background = style.focused.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom_Sel");
        style.border = new RectOffset(2, 2, 2, 2);
        
        _thing = thing;
        _colorPicker = new ColorPicker();
        
        _colorsSet = new SixColorSet
        {
            ColorOne = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorTwo = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorThree = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorFour = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorFive = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorSix = new Color(Rand.Value, Rand.Value, Rand.Value),
        };
    }
    
    public override void DoWindowContents(Rect inRect)
    {
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.Label(inRect, _thing.LabelCap);
        Text.Anchor = 0;
        
        var topBar = inRect.TopPartPixels(32);
        var body = inRect.BottomPartPixels(inRect.height - 32);
        var leftPart = body.LeftPartPixels(body.height / 3);
        
        var rightPart = body.RightPartPixels(body.width - leftPart.width).ContractedBy(5,0);
        
        //var rightPart = inRect.RightPartPixels(colorsWidthPx); //;384
        //var center = new Rect(leftPart.xMax, leftPart.y, rightPart.x - leftPart.xMax, leftPart.height);
        
        //Widgets.DrawMenuSection(leftPart);
        //Widgets.DrawHighlight(rightPart);
        //Widgets.DrawHighlight(center);

        DrawItemPreview(leftPart);
        
        Widgets.DrawMenuSection(rightPart);
        DrawColorTool(rightPart.ContractedBy(5).TopPartPixels(300).LeftPartPixels(300));
    }
    
    private void DrawColorTool(Rect rect)
    {
        var colorPickRect = rect.RightPart(0.85f);
        var colorSelectArea = rect.LeftPart(0.15f);

        var div = (colorSelectArea.height - 20) / 6;

        var y = colorSelectArea.y + 5;
        for (int i = 0; i < 6; i++)
        {
            var colorRect = new Rect(colorSelectArea.x + 5, y, colorSelectArea.width-10, div);
            var color = _colorsSet[i];
            Widgets.DrawBoxSolid(colorRect, color);
            if (Widgets.ButtonInvisible(colorRect))
            {
                _colorPicker.SetColor(color);
            }
        }
        
        _colorPicker.Draw(colorPickRect);
    }

    private void DrawItemPreview(Rect rect)
    {
        var help = Mathf.Floor(rect.height / 3);
        var group = rect.LeftPartPixels(help);
        var topRect = group.TopPartPixels(help).ContractedBy(5);
        var middleRect = group.TopPartPixels(help*2).BottomPartPixels(help).ContractedBy(5);
        var bottomRect = group.TopPartPixels(help*3).BottomPartPixels(help).ContractedBy(5);
        
        //Front
        Widgets.DrawBox(topRect, 1, BaseContent.GreyTex);
        DrawThing(topRect, _thing, Rot4.North, _colorsSet, 0);
        
        //Sideviews
        Widgets.DrawBox(middleRect, 1, BaseContent.GreyTex);
        DrawThing(middleRect, _thing, Rot4.East, _colorsSet, 0);
        
        //Back
        Widgets.DrawBox(bottomRect, 1, BaseContent.GreyTex);
        DrawThing(bottomRect, _thing, Rot4.South, _colorsSet, 0);
    }

    private void DrawMaskSelection(Rect rect)
    {
        var allMasks = GetTextures();
        
        
    }

    private List<Texture2D> GetTextures()
    {
        if(this._thing is Apparel apparel)
        {
            return MaskManager.GetMasksMulti(apparel.def, BodyTypeDefOf.Male, Rot4.North);
        }
        
        if (_thing is Building b)
        {
            return MaskManager.GetMasksMulti(b.def, Rot4.North);
        }
        
        return MaskManager.GetMasksSingle(_thing.def);
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