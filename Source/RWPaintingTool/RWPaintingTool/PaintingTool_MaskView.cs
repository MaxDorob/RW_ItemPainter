using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RWPaintingTool;

[StaticConstructorOnStartup]
public partial class PaintingTool
{
    private static Texture2D borderWhite05 = SolidColorMaterials.NewSolidColorTexture(Colors.White05);
    private static Texture2D borderWhite025 = SolidColorMaterials.NewSolidColorTexture(Colors.White025);
    
    private Vector2 _maskScrollPos;
    
    private int? _hoveredMaskIndex;
    //private int _selectedMaskIndex;
    
    private void DrawMaskSelection(Rect inRect)
    {
        //inRect = inRect.ContractedBy(1);
        //Widgets.BeginGroup(inRect);
        //inRect = inRect.AtZero();
        //Widgets.DrawBoxSolid(inRect, Colors.BGDarker);
        //var textures = GetTextures();
        //var scrollWidth = Mathf.Max(textures.Count * inRect.height, inRect.width);
        //var scrollRect = new Rect(0,0, scrollWidth, inRect.height);
        
        //Widgets.ScrollHorizontal(scrollRect, ref _maskScrollPos, inRect, 20f);
        
        ////TODO: Change scroller style
        //Widgets.BeginScrollView(scrollRect, ref _maskScrollPos, inRect, true);

        //var curX = 0;
        //bool anyHovered = false;
        //for (var i = 0; i < textures.Count; i++)
        //{
        //    var maskRect = new Rect(curX, 0, inRect.height, inRect.height).ContractedBy(5);
        //    if (Mouse.IsOver(maskRect))
        //    {
        //        _hoveredMaskIndex = i;
        //        anyHovered = true;
        //    }

        //    //DrawMaskOption(maskRect, textures[i], i);
        //}
        
        //if(!anyHovered)
        //{
        //    _hoveredMaskIndex = null;
        //}

        //Widgets.EndScrollView();
        //Widgets.EndGroup();
    }

    private static Material RenderMat = new Material(ShaderDB.CutoutMultiMask);
    
    //private void DrawMaskOption(Rect rect, Texture2D mask, int index)
    //{
    //    var hovered = Mouse.IsOver(rect);
    //    var selBorder = _selectedMaskIndex == index? BaseContent.WhiteTex : borderWhite025;
    //    selBorder = hovered ? borderWhite05 : selBorder;
        
    //    RenderMaskMat(rect, mask);
    //    Widgets.DrawBox(rect, 2, selBorder);
        
    //    if (Widgets.ButtonInvisible(rect))
    //    {
    //        _selectedMaskIndex = index;
    //        Notify_MaskChanged();
    //    }
    //}

    private void RenderMaskMat(Rect rect, Texture2D mask)
    {
        //RenderMat.SetTexture("_MainTex", _south.mainTexture);
        //RenderMat.SetTexture("_MaskTex", mask);
        //RenderMat.SetColor("_Color", Color.red);
        //RenderMat.SetColor("_ColorTwo", Color.green);
        //RenderMat.SetColor("_ColorThree", Color.blue);
        //RenderMat.SetColor("_ColorFour", Color.cyan);
        //RenderMat.SetColor("_ColorFive", Color.yellow);
        //RenderMat.SetColor("_ColorSix", Color.magenta);
        //Widgets.DrawTextureFitted(rect, _south.mainTexture, 1, RenderMat);
    }
    
    private List<Texture2D> GetTextures()
    {
        if(_thing is Apparel apparel)
        {
            return MaskManager.GetMasksMulti(apparel.def, BodyTypeDefOf.Male, Rot4.South);
        }
        
        if (_thing is Building b)
        {
            return MaskManager.GetMasksMulti(b.def, Rot4.South);
        }
        
        return MaskManager.GetMasksSingle(_thing.def);
    }
}