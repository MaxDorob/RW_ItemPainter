using TeleCore.AssetLoader;
using TeleCore.Shared;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

[StaticConstructorOnStartup]
internal static class ShaderDB
{
    internal static readonly Shader CutoutMultiMask = AssetBundleDB.LoadShader("CutoutMultiMask");

    // static ShaderDB()
    // {
    //     var count = CutoutMultiMask.GetPropertyCount();
    //     TLog.Debug($"Properties of {CutoutMultiMask.name}");
    //     for(int i = 0; i < count; i++)
    //     {
    //         var name = CutoutMultiMask.GetPropertyName(i);
    //         var type = CutoutMultiMask.GetPropertyType(i);
    //         TLog.Debug($"Property {i}: {name} ({type})");
    //     }
    // }
}