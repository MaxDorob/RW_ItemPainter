using System.Collections.Generic;
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

    //
    private static Thing? CurThing;

    [HarmonyPatch(typeof(Thing), nameof(Thing.DrawColor), MethodType.Getter)]
    internal static class ThingDrawColorPatch
    {
        public static void Postfix(Thing __instance, ref Color __result)
        {
        }
    }

    [HarmonyPatch(typeof(Thing), nameof(Thing.DrawColorTwo), MethodType.Getter)]
    internal static class ThingDrawColorTwoPatch
    {
        public static void Postfix(Thing __instance, ref Color __result)
        {
        }
    }

    [HarmonyPatch(typeof(GraphicData), nameof(GraphicData.GraphicColoredFor))]
    internal static class GraphicColoredForPatch
    {
        public static bool Prefix(Thing t, GraphicData __instance, ref Graphic __result)
        {
            CurThing = t;
            return true;
        }

        public static void Postfix(Thing t, ref Graphic __result)
        {
            var extension = t.def.GetModExtension<PaintableExtension>();
            if (extension != null)
            {
                Graphic graphic = __result;
                if (__result is Graphic_RandomRotated grr)
                {
                    graphic = grr.subGraphic;
                }
                ChangeGraphic(graphic, t);
            }

            CurThing = null;
        }

        private static Graphic ChangeGraphic(Graphic graphic, Thing apparel)
        {
            if (graphic.Shader.SupportsMultiColor())
            {
                var tracker = ColorTrackerDB.GetTracker(apparel);
                var maskTracker = ColorTrackerDB.GetMaskTracker(apparel);

                maskTracker.SetMaskOn(graphic);
                tracker.SetColorsOn(graphic);
            }

            return graphic;
        }
    }

    [HarmonyPatch(typeof(Graphic), nameof(Graphic.Init))]
    internal static class GraphicInitPatch
    {
        public static bool Prefix(ref GraphicRequest req, Graphic __instance)
        {
            if (__instance.Shader.SupportsMaskTex())
            {
                //TODO:
                // if (MaskCache.TryGetMaskFor(CurThing, out string maskPath))
                // {
                //     req.maskPath = maskPath;
                // }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(MaterialPool), nameof(MaterialPool.MatFrom), new[] { typeof(MaterialRequest) })]
    internal static class MaterialPoolMatFromPatch
    {
        public static void Postfix(MaterialRequest req, ref Material __result)
        {
            if (CurThing != null && req.shader.SupportsMultiColor())
            {
                var tracker = ColorTrackerDB.GetTracker(CurThing);
                if (tracker == null)
                {
                    Log.WarningOnce("maskTracker is null", 7236811);
                    return;
                }
                var maskTracker = ColorTrackerDB.GetMaskTracker(CurThing);
                if (maskTracker == null)
                {
                    Log.WarningOnce("maskTracker is null", 7236812);
                    return;
                }
                tracker.SetColorsOn(__result);
#if DEBUG
                Log.Message("Getting material data for: " + CurThing.def + " it would receive mask: " + maskTracker.CurMaskID);
#endif
                //__result.SetTexture("_Mask", MaskManager.GetMask(GraphicInitPatch.CurThing.def, maskTracker.CurMaskID));
                __result.SetColor(ColorThree, tracker.ColorThree);
                __result.SetColor(ColorFour, tracker.ColorFour);
                __result.SetColor(ColorFive, tracker.ColorFive);
                __result.SetColor(ColorSix, tracker.ColorSix);
            }
        }
    }

    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel))]
    internal static class ApparelGraphicRecordGetterTryGetGraphicApparelPatch
    {
        private static readonly MethodInfo GraphicChange =
            AccessTools.Method(typeof(ApparelGraphicRecordGetterTryGetGraphicApparelPatch), nameof(ChangeGraphic));

        private static MethodInfo _selectShaderMethod =
            AccessTools.Method(typeof(ApparelGraphicRecordGetterTryGetGraphicApparelPatch), nameof(SelectShader));

        public static bool Prefix(Apparel apparel)
        {
            CurThing = apparel;
            return true;
        }

        public static void Postfix()
        {
            CurThing = null;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                //Change Shader
                if (instruction.opcode == OpCodes.Ldsfld && instruction.operand ==
                    AccessTools.Field(typeof(ShaderDatabase), nameof(ShaderDatabase.Cutout)))
                {
                    yield return new CodeInstruction(OpCodes.Call, _selectShaderMethod).WithLabels(instruction.labels);
                    continue;
                }

                if (instruction.opcode == OpCodes.Stloc_2)
                {
                    //Found where graphic is stored, manipulate
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, GraphicChange);
                    yield return new CodeInstruction(OpCodes.Stloc_2);
                    continue;
                }

                yield return instruction;
            }
        }

        internal static Shader SelectShader()
        {
            Apparel apparel = CurThing as Apparel;
            if (apparel != null && (apparel.def?.HasModExtension<PaintableExtension>() ?? false))
            {
                return ShaderDB.CutoutMultiMask;
            }
            
            return ShaderDatabase.Cutout;
        }

        private static Graphic ChangeGraphic(Graphic graphic, Thing apparel)
        {
            if (graphic.Shader.SupportsMultiColor())
            {
                var tracker = ColorTrackerDB.GetTracker(apparel);
                var maskTracker = ColorTrackerDB.GetMaskTracker(apparel);
                if (tracker == null)
                {
                    Log.WarningOnce("maskTracker is null", 7236813);
                    return graphic;
                }
                if (maskTracker == null)
                {
                    Log.WarningOnce("maskTracker is null", 7236814);
                    return graphic;
                }
                maskTracker.SetMaskOn(graphic);
                tracker.SetColorsOn(graphic);
            }

            return graphic;
        }
    }

}