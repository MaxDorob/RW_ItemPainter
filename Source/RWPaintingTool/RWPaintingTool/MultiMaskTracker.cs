using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using RimWorld;
using TeleCore.Loader;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public class MaskCollection
{
    private Dictionary<ThingState, Texture2D[]> _maskyByState;
    
    
}

public static class MultiMaskTracker
{
    private static HashSet<string> _cachedMasks;
    private static Dictionary<MaskSelection, Texture2D> _maskTextures = new();

    public static Texture2D GetAllMasks(ThingDef def, Rot4? _rot = null, BodyTypeDef? bodyType = null)
    {
        
    }
    
    public static Texture2D GetMaskTexture(MaskSelection maskSelection)
    {
        
    }

    public static void CacheMasks(string path, out bool alreadyCached)
    {
        alreadyCached = _cachedMasks.Contains(path);
        if (alreadyCached) return;

        TLog.Message($"[RWPaintingTool] - Caching mask textures: {path}");
        string[] maskFiles = null;

        //Potential masks
        if (Directory.Exists(path))
        {
            maskFiles = Directory.GetFiles(path, path + "*Mask_??");
        }
        else if (File.Exists(path))
        {
            maskFiles = Directory.GetFiles(Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path) + "*Mask_??");
        }

        if (maskFiles == null)
        {
            var reg = new Regex(@"(?<path>.*?)(?<bodyType>_(Thin|Hulk|Male|Female|Fat))?(?<cardinalDirection>_(North|East|South|West))?(?<mask>_Mask\d+)?\.png$");

            foreach (var file in maskFiles)
            {
                var match = reg.Match(file);
                if (match.Success)
                {
                    var pathMatch = match.Groups["path"].Value;
                    var bodyType = match.Groups["bodyType"].Value;
                    var cardinalDirection = match.Groups["cardinalDirection"].Value;
                    var mask = match.Groups["mask"].Value;

                    TLog.Message($"[RWPaintingTool] - Found mask: {pathMatch} {bodyType} {cardinalDirection} {mask}");
                }
            }

            //Found mask selection
            //Analyze state
            // Path/[Name] _BodyType _Rotation _MaskXX
            //Eg: Apparel/Armor/Maximus_backpack_Hulk_north

            // => _BodyType must be defined as a list in regex
            // => _Rotation must be defined as a list in regex
            // => _MaskXX must always be the last part of the string

            //Regex to find _BodyType _Rotation _MaskXX:
            // ^(?<Name>.+?)(?:_(?<BodyType>.+?))?(?:_(?<Rotation>.+?))_(?<Mask>.+)$



        }

        _cachedMasks.Add(path);
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