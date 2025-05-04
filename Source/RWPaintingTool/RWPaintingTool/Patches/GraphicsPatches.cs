using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

internal static class GraphicsPatches
{
    private static readonly int ColorThree = Shader.PropertyToID("_ColorThree");
    private static readonly int ColorFour = Shader.PropertyToID("_ColorFour");
    private static readonly int ColorFive = Shader.PropertyToID("_ColorFive");
    private static readonly int ColorSix = Shader.PropertyToID("_ColorSix");

    private static Thing curThing;
    internal static Thing CurThing
    {
        get => curThing;
        set
        {
            if (value != null && curThing != null && curThing != value)
            {
                Log.Error($"Trying to change curThing from {curThing} to {value}");
            }
            if (value == null)
            {
                colors = null;
            }
            curThing = value;
        }
    }
    static Color[] colors;
    internal static Color[] Colors
    {
        get
        {
            if (colors == null)
            {
                var thing = CurThing;
                if (thing != null)
                {
                    var colorTracker = ColorTrackerDB.GetTracker(thing);
                    if (colorTracker != null)
                    {
                        colors = [colorTracker.ColorOne, colorTracker.ColorTwo, colorTracker.ColorThree, colorTracker.ColorFour, colorTracker.ColorFive, colorTracker.ColorSix];
                    }
                }
            }
            return colors;
        }
        set
        {
            if (colors != null && value != null && !Enumerable.SequenceEqual(colors, value))
            {
                var errorText = $"Trying to change used colors from\n{string.Join(";", colors)}\to\n{string.Join(";", value)}";
                Log.ErrorOnce(errorText, errorText.GetHashCode());
            }
            colors = value;
        }
    }

    static string MaskPath
    {
        get
        {
            return ColorTrackerDB.GetMaskTracker(CurThing)?.CurrentMaskPath;
        }
    }

    [HarmonyPatch]
    internal static class GraphicGetterPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Graphic));
        }
        internal static void Prefix(Thing __instance) => CurThing = __instance;
        internal static void Postfix() => CurThing = null;
    }
    [HarmonyPatch]
    internal static class GetColoredVersionPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(GraphicData), nameof(GraphicData.GraphicColoredFor));
        }
        internal static bool Prefix(Thing t, ref Graphic __result)
        {
            CurThing = t;
            if (Colors != null && !string.IsNullOrWhiteSpace(MaskPath))
            {
                var graphicBase = t.def.graphicData.Graphic;
                if (graphicBase.GetType().GetConstructor(Type.EmptyTypes) != null)
                {
                    __result = RWPT_GraphicDatabase.Get(new RWPT_GraphicRequest(graphicBase.GetType(), graphicBase.path, ShaderDB.CutoutMultiMask, graphicBase.drawSize, graphicBase.data, graphicBase.data?.renderQueue ?? 0, graphicBase.data?.shaderParameters, MaskPath, Colors));
                    return false;
                }
            }
            return true;
        }

        internal static void Postfix() => CurThing = null;
    }

    [HarmonyPatch(typeof(Graphic_Multi), nameof(Graphic_Multi.Init))]
    internal static class Graphic_Multi_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var getMethod = AccessTools.Method(typeof(ContentFinder<Texture2D>), nameof(ContentFinder<Texture2D>.Get));
            var result = instructions.ToList();
            var lastTextureGetterIndex = result.IndexOf(result.Last(x => x.Calls(getMethod)));
            result.InsertRange(lastTextureGetterIndex + 2,
                [new CodeInstruction(OpCodes.Ldloc_1), CodeInstruction.Call(typeof(Graphic_Multi_Patch), nameof(Graphic_Multi_Patch.ChangeMasksIfNeeded))]
                );

            return result;
        }

        public static void ChangeMasksIfNeeded(Texture2D[] masksArray)
        {
            var thing = CurThing;
            if (masksArray == null || thing == null)
            {
                return;
            }
            var maskManager = ColorTrackerDB.GetMaskTracker(thing);
            if (maskManager != null)
            {
                masksArray[0] = maskManager.GetMask(Rot4.North) ?? masksArray[0];
                masksArray[1] = maskManager.GetMask(Rot4.East) ?? masksArray[1];
                masksArray[2] = maskManager.GetMask(Rot4.South) ?? masksArray[2];
                masksArray[3] = maskManager.GetMask(Rot4.West, true) ?? masksArray[3];
            }

        }
    }

    [HarmonyPatch(typeof(Graphic_Single), nameof(Graphic_Single.Init))]
    internal static class Graphic_Single_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var field = AccessTools.Field(typeof(MaterialRequest), nameof(MaterialRequest.maskTex));
            foreach (var instruction in instructions)
            {
                if (instruction.StoresField(field))
                {
                    yield return CodeInstruction.Call(typeof(Graphic_Single_Patch), nameof(Graphic_Single_Patch.ChangeMaskIfNeeded));
                }
                yield return instruction;
            }
        }

        public static Texture2D ChangeMaskIfNeeded(Texture2D texture)
        {
            var thing = CurThing;
            if (thing == null)
            {
                return texture;
            }
            var maskManager = ColorTrackerDB.GetMaskTracker(thing);
            if (maskManager != null)
            {
                texture = maskManager.GetMask() ?? texture;
            }
            return texture;
        }
    }
    //[HarmonyPatch]
    //internal static class GraphicDBPatch
    //{
    //    public static IEnumerable<MethodBase> TargetMethods()
    //    {
    //        //yield return AccessTools.Method(typeof(GraphicDatabase), nameof(GraphicDatabase.GetInner), generics: [typeof(Graphic_Single)]);
    //        //foreach (var graphicType in typeof(Graphic).AllSubclassesNonAbstract().Where(x => x.GetConstructor(Type.EmptyTypes) != null))
    //        //{
    //        //    Log.Message(graphicType.Name);
    //        //    var method = AccessTools.Method(typeof(GraphicDatabase), nameof(GraphicDatabase.GetInner), generics: [graphicType]);
    //        //    Log.Message($"Patching {graphicType.Name} {method?.Name ?? "null"}");
    //        //    yield return method;
    //        //}
    //    }
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        foreach (var codeInstruction in instructions)
    //        {
    //            yield return codeInstruction;
    //        }
    //    }
    //    internal static void Postfix(GraphicRequest req, ref Graphic __result)
    //    {
    //        Log.Message(__result);

    //    }
    //}
    [HarmonyPatch]
    internal static class MaterialPoolPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(MaterialPool), nameof(MaterialPool.MatFrom), [typeof(MaterialRequest)]);
        }
        public static void Postfix(MaterialRequest req, ref Material __result)
        {
            //Log.Message(__result);
            if (Colors != null)
            {
                __result = RWPT_MaterialPool.MatFrom(new RWPT_MaterialRequest(req, Colors));
            }
        }
    }


    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel))]
    internal static class ApparelGraphicRecordGetterTryGetGraphicApparelPatch
    {
        private static readonly MethodInfo GraphicChange =
            AccessTools.Method(typeof(ApparelGraphicRecordGetterTryGetGraphicApparelPatch), nameof(ChangeGraphicIfNeeded));


        public static void Prefix(Apparel apparel)
        {
            CurThing = apparel;
        }

        public static void Postfix()
        {
            CurThing = null;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {

                if (instruction.opcode == OpCodes.Stloc_2)
                {
                    //Found where graphic is stored, manipulate
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, GraphicChange);
                    yield return new CodeInstruction(OpCodes.Stloc_2);
                    continue;
                }

                yield return instruction;
            }
        }


        private static Graphic ChangeGraphicIfNeeded(Graphic graphic, Thing apparel, string path)
        {
            CurThing = apparel;
            if (Colors != null && !string.IsNullOrWhiteSpace(MaskPath))
            {
                graphic = RWPT_GraphicDatabase.Get(new RWPT_GraphicRequest(typeof(Graphic_Multi), path, ShaderDB.CutoutMultiMask, apparel.def.graphicData.drawSize, MaskPath, Colors));
            }

            return graphic;
        }
    }

    [HarmonyPatch(typeof(ShaderUtility), nameof(ShaderUtility.SupportsMaskTex))]
    internal static class ShaderUtility_SupportsMaskTex
    {
        public static void Postfix(Shader shader, ref bool __result)
        {
            if (shader == ShaderDB.CutoutMultiMask)
            {
                __result = true;
            }
        }
    }

}