using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using RimWorld;
using TeleCore.Loader;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public static class MaskManager
{
    //All masks bound to a unique identifier
    private static readonly Dictionary<TextureID, Texture2D> _masks = new Dictionary<TextureID, Texture2D>();

    private static Dictionary<ThingDef, List<Texture2D>> _cachedMasksSingle = new Dictionary<ThingDef, List<Texture2D>>();
    private static Dictionary<ThingDef, List<Texture2D[]>> _cachedMasksMulti = new Dictionary<ThingDef, List<Texture2D[]>>();
    private static Dictionary<(ThingDef, BodyTypeDef), List<Texture2D[]>> _cachedMasksBody = new Dictionary<(ThingDef, BodyTypeDef), List<Texture2D[]>>();
    private static Dictionary<(ThingDef, Rot4), List<Texture2D>> _cachedMasksRot = new Dictionary<(ThingDef, Rot4), List<Texture2D>>();
    private static Dictionary<(ThingDef, BodyTypeDef, Rot4), List<Texture2D>> _cachedMasksRotBody = new Dictionary<(ThingDef, BodyTypeDef, Rot4), List<Texture2D>>();
    
    public static List<Texture2D> GetMasksSingle(ThingDef forDef)
    {
        return _cachedMasksSingle.GetValueOrDefault(forDef);
    }

    public static List<Texture2D> GetMasksMulti(ThingDef forDef, Rot4 rot)
    {
        return _cachedMasksRot.GetValueOrDefault((forDef, rot));
    }
    
    public static List<Texture2D> GetMasksMulti(ThingDef forDef, BodyTypeDef bodyType, Rot4 rot)
    {
        return _cachedMasksRotBody.GetValueOrDefault((forDef, bodyType, rot));
    }
    
    public static List<Texture2D[]> GetMasksMulti(ThingDef forDef)
    {
        return _cachedMasksMulti.GetValueOrDefault(forDef);
    }
    
    public static List<Texture2D[]> GetMasksMulti(ThingDef forDef, BodyTypeDef bodyType)
    {
        return _cachedMasksBody.GetValueOrDefault((forDef, bodyType));
    }
    
    public static Texture2D GetMask(TextureID id)
    {
        return _masks.TryGetValue(id, out var texture) ? texture : null;
    }
    
    public static Texture2D GetMask(ThingDef forDef, int maskID, Rot4? withRotation = null, BodyTypeDef? withBodyType = null)
    {
        var mti = new TextureID
        {
            Def = forDef,
            MaskID = maskID,
            Rotation = withRotation ?? Rot4.North,
            BodyType = withBodyType
        };
        
        return _masks.TryGetValue(mti, out var texture) ? texture : null;
    }

    public static string GetMaskPath(Thing thing, int maskID)
    {
        var rootPath = thing.def.apparel?.wornGraphicPath ?? thing.def.graphicData.texPath;
        return GetMaskPath(rootPath, (thing.ParentHolder is Pawn pawn ? pawn.story.bodyType : null), maskID);
    }
    
    public static string GetMaskPath(string rootPath, BodyTypeDef? bodyType, int maskID)
    {
        var bodyTypePart = "_" + bodyType?.defName ?? "";
        var maskPart = $"_Mask{maskID}";

        return rootPath + bodyTypePart + maskPart; //Rotation is implied for apparel
    }
    
    private static bool HasBodyType(ThingDef def)
    {
        var apparel = def.apparel;
        if (apparel == null) return false;

        //Check gotten from ApparelGraphicRecordGetter.TryGetGraphicApparel
        var isOnHead = apparel.LastLayer == ApparelLayerDefOf.Overhead ||
                       apparel.LastLayer == ApparelLayerDefOf.EyeCover ||
                       (apparel.LastLayer.IsUtilityLayer && 
                        (apparel.wornGraphicData == null || 
                         apparel.wornGraphicData.renderUtilityAsPack));
        return !isOnHead;
    }
    
    internal static void CacheMasks(ThingDef thingDef)
    {
        var paintable = thingDef.GetModExtension<PaintableExtension>();
        if (paintable == null) return;
        TLog.Debug($"Trying to cache masks for: {thingDef}");
        
        string[] maskFiles = null;
        
        if (thingDef.apparel != null)
        {
            maskFiles = GetMaskFiles(thingDef, thingDef.apparel.wornGraphicPath);
        }
        else
        {
            maskFiles = GetMaskFiles(thingDef, thingDef.graphicData.texPath);
        }

        if (maskFiles == null) return;
        
        TLog.Debug($"Discovered {maskFiles.Length} maskFiles");
        foreach (var file in maskFiles)
        {
            var texture = ContentFinder<Texture2D>.Get(file);
            TryCache(thingDef, file, texture);
        }
    }

    private static void TryCache(ThingDef def, string path, Texture2D texture)
    {
        var bodyTypes = "Male|Female|Thin|Fat|Hulk";
        var cardinalDirections = "North|South|East|West";
        var regs = $@"^(?<Path>.*\/)(?<Name>[^_/]+(?:_[^_/]+)*?)(?:_(?<BodyType>{bodyTypes}))?(?:_(?<Mask>Mask\d+))?(?:_(?<Rotation>{cardinalDirections}))?$";
        var reg = new Regex(regs, RegexOptions.IgnoreCase);
        var match = reg.Match(path);
        if (match.Success)
        {
            var pathMatch = match.Groups["Path"].Value;
            var name = match.Groups["Name"].Value;
            var bodyType = match.Groups["BodyType"].Value;
            var cardinalDirection = match.Groups["Rotation"].Value;
            var mask = match.Groups["Mask"].Value;
            
            Regex regex = new Regex(@"\d+");
            Match maskNum = regex.Match(mask);

            var bodyTypeDef = DefDatabase<BodyTypeDef>.GetNamed(bodyType, false);
            var maskID = int.Parse(maskNum.Value);
            var rotation = Rot4.FromString(cardinalDirection.CapitalizeFirst());
            
            _masks.Add(new TextureID
            {
                Def = def,
                BodyType = bodyTypeDef,
                MaskID = maskID,
                Rotation = rotation
            }, texture);
        }
    }
    
    public static void Resolve()
    {
        IEnumerable<IGrouping<DefID<ThingDef>, KeyValuePair<TextureID, Texture2D>>> groupedByDef = _masks
            .Where(kvp => kvp.Key.MaskID >= 0)
            .GroupBy(kvp => kvp.Key.Def);

        foreach (var defGroup2 in groupedByDef)
        {
            var apparel = defGroup2.Key.Def.apparel;
            var isApparel = apparel != null;
            var isGraphicSingle = defGroup2.Key.Def.graphicData.graphicClass.IsAssignableFrom(typeof(Graphic_Single));
            var isGraphicMulti = isApparel || defGroup2.Key.Def.graphicData.graphicClass.IsAssignableFrom(typeof(Graphic_Multi));
         
            var bodyGroups = defGroup2.GroupBy(kvp => kvp.Key.BodyType);
            var maskGroups = defGroup2.GroupBy(kvp => kvp.Key.MaskID);
            var rotGroups = defGroup2.GroupBy(kvp => kvp.Key.Rotation);
            
            if (isApparel)
            {
                foreach (var bodyGroup in bodyGroups)
                {
                    //Now group by mask
                    var subMaskGroups = bodyGroup.GroupBy(kvp => kvp.Key.MaskID);
                    var list = new List<Texture2D[]>();
                    _cachedMasksBody.Add((defGroup2.Key.Def, bodyGroup.Key), list);
                    foreach (var subMaskGroup in subMaskGroups)
                    {
                        var maskTexturedByMaskByGroup = subMaskGroup.Select(bg => bg.Value).ToArray();   
                        list.Add(maskTexturedByMaskByGroup);
                    }
                    
                    //Group by rotation
                    var subRotGroups = bodyGroup.GroupBy(kvp => kvp.Key.Rotation);
                    foreach (var subRotGroup in subRotGroups)
                    {
                        var maskTexturedByRotByGroup = subRotGroup.Select(bg => bg.Value).ToList();
                        _cachedMasksRotBody.Add((defGroup2.Key.Def, bodyGroup.Key, subRotGroup.Key), maskTexturedByRotByGroup);
                    }
                }
            }
            if (isGraphicMulti)
            {
                var list = new List<Texture2D[]>();
                _cachedMasksMulti.Add(defGroup2.Key.Def, list);
                foreach (var maskGroup in maskGroups)
                {
                    var maskTexturedByMask = maskGroup.Select(bg => bg.Value).ToArray();
                    list.Add(maskTexturedByMask);
                }
                
                foreach (var rotGroup in rotGroups)
                {
                    var maskTexturedByRot = rotGroup.Select(bg => bg.Value).ToList();
                    _cachedMasksRot.Add((defGroup2.Key.Def, rotGroup.Key), maskTexturedByRot);
                }
            }
            else if (isGraphicSingle)
            {
                _cachedMasksSingle.Add(defGroup2.Key.Def, defGroup2.Select(kvp => kvp.Value).ToList());
            }
        }
    }
    
    private static string[] GetMaskFiles(Def def, string rootPath)
    {
        var textureRoot = Path.Combine(def.modContentPack.RootDir, GenFilePaths.TexturesFolder);
        var fullPath = Path.Combine(textureRoot, rootPath);
        var directoryPath = Path.GetDirectoryName(fullPath);
        var fileName = Path.GetFileNameWithoutExtension(fullPath);
            
        string[] maskFiles = Directory.GetFiles(directoryPath, fileName + "*_Mask*");
        for (var i = 0; i < maskFiles.Length; i++)
        {
            maskFiles[i] = Path.Combine(Path.GetDirectoryName(rootPath),Path.GetFileNameWithoutExtension(maskFiles[i])).Replace("\\", "/");
        }
        return maskFiles;
    }

    public static TextureID IdFromTexture(Texture getTexture)
    {
        var id = _masks.FirstOrDefault(kvp => kvp.Value == getTexture).Key;
        return id;
    }
    
    internal static bool SupportsMultiColor(this Shader shader)
    {
        return shader == ShaderDB.CutoutMultiMask;
    }
}