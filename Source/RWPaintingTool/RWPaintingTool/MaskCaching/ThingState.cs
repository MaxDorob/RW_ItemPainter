using RimWorld;
using TeleCore.Loader;
using Verse;

namespace RWPaintingTool;

public struct ThingState
{
    public DefID<ThingDef> Def { get; set; }
    public DefID<BodyTypeDef>? BodyType { get; set; }
    public Rot4 Rot4 { get; set; }
}