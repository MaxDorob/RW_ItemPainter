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

    private Material colorPickerMat;
    private Material hueMat;
    private Material satMat;
    private Material valMat;
    
    private Material redMat;
    private Material greenMat;
    private Material blueMat;
    
    private Material alphaMat;

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
        colorPickerMat = new Material(AssetBundleDB.LoadShader("ColorPickerGradient"));
        hueMat = new Material(AssetBundleDB.LoadShader("ColorPickerHue"));
        satMat = new Material(AssetBundleDB.LoadShader("ColorPickerHue"));
        valMat = new Material(AssetBundleDB.LoadShader("ColorPickerHue"));
        
        redMat = new Material(AssetBundleDB.LoadShader("ColorPickerRGB"));
        greenMat = new Material(AssetBundleDB.LoadShader("ColorPickerRGB"));
        blueMat = new Material(AssetBundleDB.LoadShader("ColorPickerRGB"));
        
        hueMat.SetInt("_Invert", 1);
        satMat.SetInt("_Invert", 1);
        valMat.SetInt("_Invert", 1);
        satMat.SetInt("_Mode", 1);
        valMat.SetInt("_Mode", 2);
        
        greenMat.SetInt("_Result", 1);
        blueMat.SetInt("_Result", 2);
        
        //hueMat.SetInt("_Invert", 1);
        alphaMat = new Material(AssetBundleDB.LoadShader("ColorPickerAlpha"));
        
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

    private int[] hsvVals = new int[3] ;
    private int[] colorVals = new int[3];
    private string[] buffers = new string[3] ;
    private string[] buffers2 = new string[3] ;
    
    private void DrawColorTool(Rect rect)
    {
        //Widgets.DrawHighlight(rect);
        var color = _colorsSet[colorIndex];
        
        colorVals = new[] {(int) (color.r * 255), (int) (color.g * 255), (int) (color.b * 255)};
        
       
        var parts = (rect.height / 8) /2;
        var colorSize = parts * 8 + (5 + 10 + (5*7));
        var leftSide = rect.LeftPartPixels(colorSize);
        var rightSide = new Rect(leftSide.xMax + 5, leftSide.y, rect.width / 2, rect.height);
        var colorPicker = leftSide.TopPartPixels(colorSize);

        //RectAggregator rectA = new RectAggregator(new Rect(rightSide.position, new Vector2(rightSide.width, 0)), this.GetHashCode());
        var div = new RectDivider(rightSide, this.GetHashCode(), new Vector2(5, 5));
        var HSV = div.NewRow(parts);
        var HSV_H = div.NewRow(parts);
        var HSV_S = div.NewRow(parts);
        var HSV_V = div.NewRow(parts, marginOverride:10);
        var textSize = Text.CalcSize("1000").x;
        
        var HSV_H_Label = HSV_H.NewCol(textSize/2);
        var HSV_S_Label = HSV_S.NewCol(textSize/2);
        var HSV_V_Label = HSV_V.NewCol(textSize/2);
        
        var HSV_H_Input = HSV_H.NewCol(textSize, HorizontalJustification.Right, 5);
        var HSV_S_Input = HSV_S.NewCol(textSize, HorizontalJustification.Right, 5);
        var HSV_V_Input = HSV_V.NewCol(textSize, HorizontalJustification.Right, 5);
        
        var RGB = div.NewRow(parts);
        var RGB_R = div.NewRow(parts);
        var RGB_G = div.NewRow(parts);
        var RGB_B = div.NewRow(parts);
        
        var RGB_R_Label = RGB_R.NewCol(textSize/2);
        var RGB_G_Label = RGB_G.NewCol(textSize/2);
        var RGB_B_Label = RGB_B.NewCol(textSize/2);
        
        var RGB_R_Input = RGB_R.NewCol(textSize, HorizontalJustification.Right, 5);
        var RGB_G_Input = RGB_G.NewCol(textSize, HorizontalJustification.Right, 5);
        var RGB_B_Input = RGB_B.NewCol(textSize, HorizontalJustification.Right, 5);
        
        var huePicker = rect.LeftPart(0.15f).RightPart(0.5f).TopPart(0.8f);
        var alphaPicker = rect.BottomPart(0.15f).TopPart(0.5f).RightPart(0.80f);
        
        Color.RGBToHSV(color, out var h, out var s, out var v);
        hsvVals[0] = (int) (h * 360);
        hsvVals[1] = (int) (s * 100);
        hsvVals[2] = (int) (v * 100);
        var colorSat = Color.HSVToRGB(h, 1, 1, true);

        Vector2 uvPos = new Vector2(s, 1-v);
        
        colorPickerMat.SetFloat("_Hue", h);
        redMat.SetColor("_Color", color);
        greenMat.SetColor("_Color", color);
        blueMat.SetColor("_Color", color);
        
        satMat.SetColor("_Color", colorSat);
        valMat.SetColor("_Color", colorSat);
        
        //Color Picker
        Graphics.DrawTexture(colorPicker, BaseContent.BadTex, 0, 0, 0, 0, colorPickerMat);
        
        var posRect = new Rect(colorPicker.position + (uvPos * colorPicker.size), new Vector2(7, 7));
        Widgets.DrawBoxSolidWithOutline(posRect, color, Color.black);
        
        //RGB Group
        //Widgets.Label(RGB, "RGB");
        Widgets.Label(RGB, "RGB");
        
        Widgets.Label(RGB_R_Label, "R:");
        Widgets.Label(RGB_G_Label, "G:");
        Widgets.Label(RGB_B_Label, "B:");
        
        Graphics.DrawTexture(RGB_R, BaseContent.BadTex, 0, 0, 0, 0, redMat);
        Graphics.DrawTexture(RGB_G, BaseContent.BadTex, 0, 0, 0, 0, greenMat);
        Graphics.DrawTexture(RGB_B, BaseContent.BadTex, 0, 0, 0, 0, blueMat);
        
        GUI.TextField(RGB_R_Input, colorVals[0].ToString(), style); //Widgets.TextFieldNumeric(HSV_H_Input, ref testVals[0], ref buffers[0], 0, 360);
        GUI.TextField(RGB_G_Input, colorVals[1].ToString(), style);   //Widgets.TextFieldNumeric(HSV_S_Input, ref testVals[1], ref buffers[1], 0, 100);
        GUI.TextField(RGB_B_Input, colorVals[2].ToString(), style);         //Widgets.TextFieldNumeric(HSV_V_Input, ref testVals[2], ref buffers[2], 0, 100);
        
        //HSV Group
        Widgets.Label(HSV, "HSV");
        var size = Text.CalcSize("HSV");
        //Widgets.DrawLineHorizontal(HSV.Rect.x + size.x, HSV.Rect.y + HSV.Rect.height/2, HSV.Rect.width - size.x);
        
        Widgets.Label(HSV_H_Label, "H:");
        Widgets.Label(HSV_S_Label, "S:");
        Widgets.Label(HSV_V_Label, "V:");
        
        Graphics.DrawTexture(HSV_H, BaseContent.BadTex, 0, 0, 0, 0, hueMat);
        Graphics.DrawTexture(HSV_S, BaseContent.BadTex, 0, 0, 0, 0, satMat);
        Graphics.DrawTexture(HSV_V, BaseContent.BadTex, 0, 0, 0, 0, valMat);

        GUI.TextField(HSV_H_Input, hsvVals[0].ToString(), style); //Widgets.TextFieldNumeric(HSV_H_Input, ref testVals[0], ref buffers[0], 0, 360);
        GUI.TextField(HSV_S_Input, hsvVals[1].ToString(), style);   //Widgets.TextFieldNumeric(HSV_S_Input, ref testVals[1], ref buffers[1], 0, 100);
        GUI.TextField(HSV_V_Input, hsvVals[2].ToString(), style);         //Widgets.TextFieldNumeric(HSV_V_Input, ref testVals[2], ref buffers[2], 0, 100);
        
        
        /*Color.RGBToHSV(color, out var h, out var s, out var v);
        var colorSat = Color.HSVToRGB(h, 1, 1, true);

        Vector2 uvPos = new Vector2(s, 1-v);
        
        colorPickerMat.SetFloat("_Hue", h);
        alphaMat.SetColor("_Color", color);
        alphaMat.SetVector("_Scale", new Vector4(alphaPicker.width, alphaPicker.height, 0, 0).normalized);
        //colorPickerMat.SetFloat("_Alpha", );
        
        //Color Picker
        Graphics.DrawTexture(colorPicker, BaseContent.BadTex, 0, 0, 0, 0, colorPickerMat);
        Graphics.DrawTexture(huePicker, BaseContent.BadTex, 0, 0, 0, 0, hueMat);
        Graphics.DrawTexture(alphaPicker, BaseContent.BadTex, 0, 0, 0, 0, alphaMat);
        
        var posRect = new Rect(colorPicker.position + (uvPos * colorPicker.size), new Vector2(7, 7));
        Widgets.DrawBoxSolidWithOutline(posRect, color, Color.black);
        
        var hueRect = new Rect(huePicker.x - 2,  huePicker.y + (huePicker.height * (1-h)) + 4, huePicker.width + 4, 8);
        Widgets.DrawBoxSolidWithOutline(hueRect, colorSat, Color.black);

        var alphaLine = alphaPicker.x + alphaPicker.width * color.a;
        Widgets.DrawLineVertical(alphaLine, alphaPicker.y -2, alphaPicker.height+4);*/

    }

    private void RGBSlider(Color color, int mode)
    {
        switch (mode)
        {
            case 0:
            {

            }
                break;
        }
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
    
    private void DrawColorPicker(Rect rect)
    {
        
    }
}