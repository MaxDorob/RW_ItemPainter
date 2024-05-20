using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public class MaskCollectionData
{
    public ThingDef forDef;
    public int maskCount;

    private Dictionary<MaskTextureID, Texture2D> _masks;
    
    public MaskCollectionData(ThingDef thingDef, PaintableExtension paintable)
    {
        forDef = thingDef;
        maskCount = paintable.maskCount;
        _masks = new Dictionary<MaskTextureID, Texture2D>();
        
        
    }

    public static string GetMaskPathFor(Thing thing, int maskID)
    {
        var rootPath = thing.def.apparel?.wornGraphicPath ?? thing.def.graphicData.texPath;
        var bodyTypePart = thing.ParentHolder is Pawn pawn ? $"_{pawn.story.bodyType.defName}" : "";
        var maskPart = $"_Mask{maskID}";

        return rootPath + bodyTypePart + maskPart; //Rotation is implied for apparel
        // Things/Armor/HeavySuit_Thin_Mask1
    }
    
    public Texture2D GetMaskTexture(int maskID, Rot4? withRotation = null, BodyTypeDef? withBodyType = null)
    {
        var mti = new MaskTextureID
        {
            MaskID = maskID,
            Rotation = withRotation ?? Rot4.North,
            BodyType = withBodyType
        };
        
        return _masks.TryGetValue(mti, out var texture) ? texture : null;
    }

    public IEnumerable<Texture2D> GetAllMaskVariants(Rot4? withRotation = null, BodyTypeDef? withBodyType = null)
    {
        foreach (var mask in _masks)
        {
            if (mask.Key.Rotation == withRotation)
            {
                if (withBodyType != null)
                {
                    if (mask.Key.BodyType != withBodyType) continue;
                }
                yield return mask.Value;
            }
        }
    }
    
}