using RimWorld;
using TeleCore.Loader;
using Verse;

namespace RWPaintingTool;

public struct TextureID
{
    public TextureID()
    {
    }

    public DefID<ThingDef> Def { get; set; } = null;
    public DefID<BodyTypeDef>? BodyType { get; set; } = null;
    public int MaskID { get; set; } = -1;
    public Rot4 Rotation { get; set; } = Rot4.Invalid;
}