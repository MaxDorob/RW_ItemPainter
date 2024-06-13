using System.Collections.Generic;
using RimWorld;
using TeleCore.Shared;
using TeleCore.UI;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private Vector2 _maskScrollPos;
    
    private int? _hoveredMaskIndex;
    private int _selectedMaskIndex;
    
    private void DrawMaskSelection(Rect inRect)
    {
        inRect = inRect.ContractedBy(1);
        Widgets.BeginGroup(inRect);
        inRect = inRect.AtZero();
        Widgets.DrawBoxSolid(inRect, TColor.BGDarker);
        var textures = GetTextures();
        var scrollWidth = Mathf.Max(textures.Count * inRect.height, inRect.width);
        var scrollRect = new Rect(0,0, scrollWidth, inRect.height);
        
        Widgets.ScrollHorizontal(scrollRect, ref _maskScrollPos, inRect, 20f);
        
        //TODO: Change scroller style
        Widgets.BeginScrollView(scrollRect, ref _maskScrollPos, inRect, true);

        var curX = 0;
        bool anyHovered = false;
        for (var i = 0; i < textures.Count; i++)
        {
            var maskRect = new Rect(curX, 0, inRect.height, inRect.height).ContractedBy(5);
            if (Mouse.IsOver(maskRect))
            {
                _hoveredMaskIndex = i;
                anyHovered = true;
            }

            if (Widgets.ButtonImage(maskRect, textures[i], Color.white, Color.white))
            {
                _selectedMaskIndex = i;
                Notify_MaskChanged();
            }
        }
        if(!anyHovered)
        {
            _hoveredMaskIndex = null;
        }

        Widgets.EndScrollView();
        Widgets.EndGroup();
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