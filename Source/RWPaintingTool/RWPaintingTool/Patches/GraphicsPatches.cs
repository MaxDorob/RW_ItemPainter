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
    private static Thing CurThing
    {
        get => curThing;
        set
        {
            if (value != null && curThing != null && curThing != value)
            {
                Log.Error($"Trying to change curThing from {curThing} to {value}");
            }
            curThing = value;
        }
    }
    static Color[] colors;
    private static Color[] Colors
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
                        Log.Message(string.Join(", ", colors));
                    }
                }
            }
            return colors;
        }
        set
        {
            if (colors != null && value != null && Enumerable.SequenceEqual(colors, value))
            {
                Log.Error($"Trying to change used colors");
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
                    __result = RWPT_GraphicDatabase.Get(new RWPT_GraphicRequest(graphicBase.GetType(), graphicBase.path, graphicBase.Shader, graphicBase.drawSize, graphicBase.data, graphicBase.data?.renderQueue ?? 0, graphicBase.data?.shaderParameters, MaskPath, Colors));
                    return false;
                }
            }
            return true;
        }

        internal static void Postfix() => CurThing = null;
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
                Log.Message($"Changing material. Colors is null {Colors == null}, {string.Join(", ", Colors)}");
                __result = RWPT_MaterialPool.MatFrom(new RWPT_MaterialRequest(req, Colors));
            }
        }
    }


    //[HarmonyPatch(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel))]
    //internal static class ApparelGraphicRecordGetterTryGetGraphicApparelPatch
    //{
    //    private static readonly MethodInfo GraphicChange =
    //        AccessTools.Method(typeof(ApparelGraphicRecordGetterTryGetGraphicApparelPatch), nameof(ChangeGraphic));

    //    private static MethodInfo _selectShaderMethod =
    //        AccessTools.Method(typeof(ApparelGraphicRecordGetterTryGetGraphicApparelPatch), nameof(SelectShader));

    //    public static bool Prefix(Apparel apparel)
    //    {
    //        CurThing = apparel;
    //        return true;
    //    }

    //    public static void Postfix()
    //    {
    //        CurThing = null;
    //    }

    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        foreach (var instruction in instructions)
    //        {
    //            //Change Shader
    //            if (instruction.opcode == OpCodes.Ldsfld && instruction.operand ==
    //                AccessTools.Field(typeof(ShaderDatabase), nameof(ShaderDatabase.Cutout)))
    //            {
    //                yield return new CodeInstruction(OpCodes.Call, _selectShaderMethod).WithLabels(instruction.labels);
    //                continue;
    //            }

    //            if (instruction.opcode == OpCodes.Stloc_2)
    //            {
    //                //Found where graphic is stored, manipulate
    //                yield return instruction;
    //                yield return new CodeInstruction(OpCodes.Ldloc_2);
    //                yield return new CodeInstruction(OpCodes.Ldarg_0);
    //                yield return new CodeInstruction(OpCodes.Call, GraphicChange);
    //                yield return new CodeInstruction(OpCodes.Stloc_2);
    //                continue;
    //            }

    //            yield return instruction;
    //        }
    //    }

    //    internal static Shader SelectShader()
    //    {
    //        Apparel apparel = CurThing as Apparel;
    //        if (apparel != null && (apparel.def?.HasModExtension<PaintableExtension>() ?? false))
    //        {
    //            return ShaderDB.CutoutMultiMask;
    //        }

    //        return ShaderDatabase.Cutout;
    //    }

    //    private static Graphic ChangeGraphic(Graphic graphic, Thing apparel)
    //    {
    //        if (graphic.Shader.SupportsMultiColor())
    //        {
    //            var tracker = ColorTrackerDB.GetTracker(apparel);
    //            var maskTracker = ColorTrackerDB.GetMaskTracker(apparel);
    //            if (tracker == null)
    //            {
    //                Log.WarningOnce("maskTracker is null", 7236813);
    //                return graphic;
    //            }
    //            if (maskTracker == null)
    //            {
    //                Log.WarningOnce("maskTracker is null", 7236814);
    //                return graphic;
    //            }
    //            maskTracker.SetMaskOn(graphic);
    //            tracker.SetColorsOn(graphic);
    //        }

    //        return graphic;
    //    }
    //}

}