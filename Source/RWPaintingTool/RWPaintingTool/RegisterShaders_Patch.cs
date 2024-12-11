using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWPaintingTool
{
    [HarmonyPatch(typeof(ModContentPack), nameof(ModContentPack.ReloadContentInt))]
    internal static class RegisterShaders_Patch
    {
        static void Postfix(ModContentPack __instance)
        {
            if (__instance == PaintingToolMod.Mod.Content)
            {
                foreach (var shader in __instance.assetBundles.loadedAssetBundles.SelectMany(x=>x.LoadAllAssets<Shader>()))
                {
                    var name = GetShortName(shader.name);
                    if (!ShaderDatabase.lookup.ContainsKey(name))
                    {
                        ShaderDatabase.lookup.Add(name, shader);
                    }
                }
            }
        }
        static string GetShortName(string fullName)
        {
            var name = fullName;
            if (name.Contains("/"))
            {
                name = name.Substring(name.LastIndexOf("/") + 1);
            }
            return name;
        }
    }
}
