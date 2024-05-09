using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using TeleCore.Loader;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public static class MultiMaskTracker
{
    private static Dictionary<MaskSelection, Texture2D> _maskTextures = new();

    public static Texture2D GetMaskTexture(MaskSelection maskSelection)
    {
        
    }

    public static void RegisterDef(ThingDef def)
    {
        
    }
}

public struct MaskTextureID
{
    private DefID<BodyTypeDef>? _bodyType;
    private int _index;
    private Rot4 rotation;

    public string GetTexturePath(string basePath)
    {
        var bodyType = _bodyType is null ? "" : $"_{_bodyType.Value.Def.defName}";
        return $"{basePath}{bodyType}_{_index}_{RotationAsPostfix(rotation)}";
    }

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