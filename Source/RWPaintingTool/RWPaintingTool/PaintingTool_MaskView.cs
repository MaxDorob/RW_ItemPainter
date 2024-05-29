using System.Collections.Generic;
using RimWorld;
using TeleCore.UI;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private Vector2 _maskScrollPos;
    private Texture2D _selectedMask;
    private Texture2D? _hoveredMask;
    
    private void DrawMaskSelection(Rect inRect)
    {
        var textures = GetTextures();
        var scrollWidth = textures.Count * inRect.height;
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
                _hoveredMask = textures[i];
                anyHovered = true;
            }

            if (Widgets.ButtonImage(maskRect, textures[i], Color.white, Color.white))
            {
                _selectedMask = textures[i];
            }
        }
        if(!anyHovered)
        {
            _hoveredMask = null;
        }

        Widgets.EndScrollView();
    }
    
    private List<Texture2D> GetTextures()
    {
        if(_thing is Apparel apparel)
        {
            return MaskManager.GetMasksMulti(apparel.def, BodyTypeDefOf.Male, Rot4.North);
        }
        
        if (_thing is Building b)
        {
            return MaskManager.GetMasksMulti(b.def, Rot4.North);
        }
        
        return MaskManager.GetMasksSingle(_thing.def);
    }
}