using System;
using RimWorld;
using Verse;

namespace RWPaintingTool;

public struct MaskTextureID
{
    public int MaskID { get; set; }
    public Rot4 Rotation { get; set; }
    public BodyTypeDef BodyType { get; set; }
    
    private static string RotationAsPostfix(Rot4 rotation)
    {
        return rotation.AsInt switch
        {
            0 => "_North",
            1 => "_East",
            2 => "_South",
            3 => "_West",
            _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
        };
    }
}