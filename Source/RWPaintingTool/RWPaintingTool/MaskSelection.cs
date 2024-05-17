using RimWorld;
using TeleCore.Loader;
using UnityEngine;
using Verse;

namespace RWPaintingTool;


public struct ThingState
{
    private DefID<ThingDef> _def;
    private DefID<BodyTypeDef>? _bodyType;
    private Rot4 _rot4;
}

public struct MaskSelection
{
    public DefID<ThingDef> DefID { get; set; }
    public int Index { get; set; }

    public ThingDef Def => DefID.Def;
    public Texture2D Texture => MultiMaskTracker.GetMaskTexture(this);

    public string TexturePath
    {
        get
        {
            var def = Def;
            var apparel = def.apparel;
            if (apparel != null)
            {
                return apparel.wornGraphicPath;
            }
            return def.graphicData.texPath;
        }
    }
    
    public static MaskSelection FromTexture(Texture maskTexture)
    {
        return default;
    }
}