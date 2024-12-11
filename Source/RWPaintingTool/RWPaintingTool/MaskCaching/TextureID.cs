using RimWorld;
using Verse;

namespace RWPaintingTool;

public struct TextureID
{
    public TextureID()
    {
    }

    public ThingDef Def { get; set; }
    public BodyTypeDef? BodyType { get; set; } = null;
    public int MaskID { get; set; } = -1;
    public Rot4 Rotation { get; set; } = Rot4.Invalid;
}