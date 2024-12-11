using HarmonyLib;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public sealed class PaintingToolMod : Mod
{
    private readonly Harmony _harmony;

    public static PaintingToolMod Mod { get; private set; }
    
    public PaintingToolMod(ModContentPack content) : base(content)
    {
        Mod = this;
        _harmony = new Harmony("telefonmast.paintingtool");
        _harmony.PatchAll();
        Log.Message(content.assetBundles.loadedAssetBundles.Count);
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(content.RootDir, "AssetBundles"));
        if (!directoryInfo.Exists)
        {
            Log.Message($"{directoryInfo.FullName} do not exists");
            return;
        }
        Log.Message($"Files into {directoryInfo.FullName}\n:{string.Join("\n", directoryInfo.GetFiles("*", SearchOption.AllDirectories).Select(x=>x.FullName))}");
        foreach (FileInfo fileInfo in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            if (fileInfo.Extension.NullOrEmpty())
            {
                //AssetBundle assetBundle = AssetBundle.LoadFromFile(fileInfo.FullName);
                //if (assetBundle != null)
                //{
                //    if (content.assetBundles.loadedAssetBundles.Contains(assetBundle))
                //    {
                //        Log.Message("Contains loaded asset");
                //    }
                //}
                //else
                //{
                //    Log.Error("Could not load asset bundle at " + fileInfo.FullName);
                //}
            }
            else
            {
                Log.Message($"{fileInfo.FullName} contains extension");
            }
        }
    }

}