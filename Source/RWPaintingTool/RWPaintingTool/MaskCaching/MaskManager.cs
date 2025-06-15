using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using RimWorld;
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

    public static Texture2D GetMask(TextureID id, bool silent = false)
    {
        var originalId = id;
        if (!_masks.TryGetValue(id, out Texture2D result))
        {
            if (id.BodyType != null)
            {
                id.BodyType = null;
                _masks.TryGetValue(id, out result);
            }
        }
        if (result == null && !silent)
        {
            Log.Error($"{originalId}\n\nAll masks of {id.Def}:\n{string.Join("\n\n", _masks.Keys.Where(x => x.Def == id.Def))}");
        }
        return result;
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
#if DEBUG
        Log.Message($"Trying to cache masks for: {thingDef}");
#endif

        var maskFiles = Enumerable.Empty<string>();


        if (thingDef.apparel != null)
        {
            maskFiles = maskFiles.Union(GetMaskFiles(thingDef, thingDef.apparel.wornGraphicPath).Where(x=> x == thingDef.apparel.wornGraphicPath));
        }
        if (!string.IsNullOrWhiteSpace(thingDef.graphicData.texPath))
        {
            maskFiles = maskFiles.Union(GetMaskFiles(thingDef, thingDef.graphicData.texPath).Where(x => x == thingDef.graphicData.texPath));
        }
        maskFiles = maskFiles.Distinct().ToList();
#if DEBUG
            Log.Message($"Discovered {maskFiles.Count()} maskFiles for {thingDef.defName}(\"{thingDef.apparel?.wornGraphicPath}\", \"{thingDef.graphicData.texPath}\"):\n{string.Join("\n", maskFiles)}");
#endif
        foreach (var maskFile in maskFiles)
        {
            TryCache(thingDef, maskFile);
        }

    }
    private static void TryCache(ThingDef def, string path)
    {
        TryCache(def, path, ContentFinder<Texture2D>.Get(path));
    }
    private static void TryCache(ThingDef def, string path, Texture2D texture)
    {
        if (texture == null)
        {
            Log.Error($"Texture was null for {def} with path \"{path}\"");
            return;
        }
        var bodyTypes = "Male|Female|Thin|Fat|Hulk";
        var cardinalDirections = "North|South|East|West";
        var regs = $@"^(?<Path>.*\/)_*(?<Name>[^_/]+(?:_[^_/]+)*?)(?:_(?<BodyType>{bodyTypes}))?(?:_(?<Mask>Mask\d+))?(?:_(?<Rotation>{cardinalDirections}))?_?(?<VanillaMask>m\d?)?$";
        var reg = new Regex(regs, RegexOptions.IgnoreCase);
        var match = reg.Match(path);
        if (match.Success)
        {
            var pathMatch = match.Groups["Path"].Value;
            var name = match.Groups["Name"].Value;
            var bodyType = match.Groups["BodyType"].Value;
            var cardinalDirection = match.Groups["Rotation"].Value;
            var mask = match.Groups["Mask"].Value;
            if (string.IsNullOrWhiteSpace(mask))
            {
                mask = match.Groups["VanillaMask"].Value;
            }

            Regex regex = new Regex(@"\d+");
            Match maskNum = regex.Match(mask);

            var bodyTypeDef = DefDatabase<BodyTypeDef>.GetNamed(bodyType, false);
            if (!int.TryParse(maskNum.Value, out var maskID))
            {
                maskID = 0;
            }
            Rot4 rotation = Rot4.Invalid;
            if (!string.IsNullOrWhiteSpace(cardinalDirection))
            {
                rotation = Rot4.FromString(cardinalDirection.CapitalizeFirst());
            }
            var textureId = new TextureID
            {
                Def = def,
                BodyType = bodyTypeDef,
                MaskID = maskID,
                Rotation = rotation
            };
            if (!_masks.TryAdd(textureId, texture))
            {
                Log.Warning($"Failed to add a new mask for {def.defName} with path \"{path}\". Replacing existing mask...");
                _masks[textureId] = texture;
            }
        }
        else
        {
            Log.Error($"Can't cache {path}");
        }
    }

    public static void Resolve()
    {
        IEnumerable<IGrouping<ThingDef, KeyValuePair<TextureID, Texture2D>>> groupedByDef = _masks
            .Where(kvp => kvp.Key.MaskID >= 0)
            .GroupBy(kvp => kvp.Key.Def);

        foreach (var defGroup2 in groupedByDef)
        {
            var apparel = defGroup2.Key.apparel;
            var isApparel = apparel != null;
            var isGraphicSingle = defGroup2.Key.graphicData.graphicClass.IsAssignableFrom(typeof(Graphic_Single));
            var isGraphicMulti = isApparel || defGroup2.Key.graphicData.graphicClass.IsAssignableFrom(typeof(Graphic_Multi));

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
                    _cachedMasksBody.Add((defGroup2.Key, bodyGroup.Key), list);
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
                        _cachedMasksRotBody.Add((defGroup2.Key, bodyGroup.Key, subRotGroup.Key), maskTexturedByRotByGroup);
                    }
                }
            }
            if (isGraphicMulti)
            {
                var list = new List<Texture2D[]>();
                _cachedMasksMulti.Add(defGroup2.Key, list);
                foreach (var maskGroup in maskGroups)
                {
                    var maskTexturedByMask = maskGroup.Select(bg => bg.Value).ToArray();
                    list.Add(maskTexturedByMask);
                }

                foreach (var rotGroup in rotGroups)
                {
                    var maskTexturedByRot = rotGroup.Select(bg => bg.Value).ToList();
                    _cachedMasksRot.Add((defGroup2.Key, rotGroup.Key), maskTexturedByRot);
                }
            }
            else if (isGraphicSingle)
            {
                _cachedMasksSingle.Add(defGroup2.Key, defGroup2.Select(kvp => kvp.Value).ToList());
            }
        }
    }

    private static string[] GetMaskFiles(Def def, string rootPath)
    {
        var textureRoot = Path.Combine(def.modContentPack.RootDir, GenFilePaths.TexturesFolder);
        var fullPath = Path.Combine(textureRoot, rootPath);
        var directoryPath = Path.GetDirectoryName(fullPath);
        var fileName = Path.GetFileNameWithoutExtension(fullPath);

        var maskFiles = Directory.GetFiles(directoryPath).Where(IsMaskFile).ToArray();
        for (var i = 0; i < maskFiles.Length; i++)
        {
            maskFiles[i] = Path.Combine(Path.GetDirectoryName(rootPath), Path.GetFileNameWithoutExtension(maskFiles[i])).Replace("\\", "/");
        }
        return maskFiles;
    }
    private static bool IsMaskFile(string fileName)
    {
        fileName = Path.GetFileNameWithoutExtension(fileName);
        if (new Regex(@".*m\d?$").IsMatch(fileName))
        {
            return true;
        }
        if (fileName.Contains("_Mask"))
        {
            return true;
        }
        if (fileName.Contains("northm", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("southm", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("eastm", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("westm", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        return false;
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