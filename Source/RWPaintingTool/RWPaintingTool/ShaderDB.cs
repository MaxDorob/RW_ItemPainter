using System.Linq;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

[StaticConstructorOnStartup]
internal static class ShaderDB
{
    internal static readonly Shader CutoutMultiMask = ShaderDatabase.LoadShader("CutoutMultiMask");

    static ShaderDB()
    {
        //Log.Message(string.Join("\n", PaintingToolMod.Mod.Content.AllAssetNamesInBundle(0)));
        //Log.Message(string.Join("\n", Resources.FindObjectsOfTypeAll<Shader>().Select(x=>x.name).OrderBy(x=>x)));
        ////Log.Message(string.Join("\n", Resources.);
        //if (ShaderDatabase.lookup == null)
        //{
        //    ShaderDatabase.lookup = new System.Collections.Generic.Dictionary<string, UnityEngine.Shader>();
        //    Log.Message("lookup was null");
        //}
        //if (!ShaderDatabase.lookup.ContainsKey("CutoutMultiMask"))
        //{
        //    var shader = (Shader)PaintingToolMod.Mod.Content.assetBundles.loadedAssetBundles.Select(x => x.LoadAsset("assets/rwcolorpicker/shaders/cutoutmultimask.shader")).FirstOrDefault(x => x != null);
        //    ShaderDatabase.lookup.Add("CutoutMultiMask", shader);
        //    Log.Message($"{shader}, {shader == null}, {ShaderDatabase.lookup.ContainsKey("CutoutMultiMask")}");
        //}
        //CutoutMultiMask = ShaderDatabase.LoadShader("CutoutMultiMask");


    }
}