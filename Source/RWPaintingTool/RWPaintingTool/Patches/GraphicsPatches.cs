using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Mono.Cecil;
using MonoMod.Utils;
using Prepatcher;
using RimWorld;
using TeleCore.Loader;
using UnityEngine;
using Verse;
using Instruction = Mono.Cecil.Cil.Instruction;

namespace RWPaintingTool;

internal static class GraphicsPatches
{
    private static readonly int ColorThree = Shader.PropertyToID("_ColorThree");
    private static readonly int ColorFour = Shader.PropertyToID("_ColorFour");
    private static readonly int ColorFive = Shader.PropertyToID("_ColorFive");
    private static readonly int ColorSix = Shader.PropertyToID("_ColorSix");

    internal static class ThingPatches
    {
        [HarmonyPatch(typeof(Thing), nameof(Thing.DrawColor), MethodType.Getter)]
        internal static class ColorPatch
        {
            public static bool Prefix(Thing __instance, ref Color __result)
            {
                var tracker = ColorTrackerDB.GetTracker(__instance);
                __result = tracker.ColorOne;
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Thing), nameof(Thing.DrawColorTwo), MethodType.Getter)]
        internal static class ColorTwoPatch
        {
            public static bool Prefix(Thing __instance, ref Color __result)
            {
               var tracker = ColorTrackerDB.GetTracker(__instance);
                __result = tracker.ColorTwo;
                return false;
            }
        }
    }
    
    internal static class ApparelPatches
    {
        [HarmonyPatch(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel))]
        internal static class TryGetGraphicApparelPatch
        {
            private static MethodInfo _selectShaderMethod = AccessTools.Method(typeof(TryGetGraphicApparelPatch), nameof(SelectShader));
            private static MethodInfo _processGraphicMethod = AccessTools.Method(typeof(TryGetGraphicApparelPatch), nameof(ProcessGraphic));
         
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                using var enumer = instructions.GetEnumerator();
                
                while (enumer.MoveNext())
                {
                    var instruction = enumer.Current;
                    if(instruction == null) continue;
                    if (instruction.opcode == OpCodes.Ldsfld && instruction.operand == AccessTools.Field(typeof(ShaderDatabase), nameof(ShaderDatabase.Cutout)))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, _selectShaderMethod).WithLabels(instruction.labels);
                        continue;
                    }

                    if (instruction.opcode == OpCodes.Call && instruction.operand == AccessTools.Method(typeof(GraphicDatabase), nameof(GraphicDatabase.Get),
                            new[] { typeof(string), typeof(Shader), typeof(Vector2), typeof(Color) },
                            new[] { typeof(Graphic_Multi) }))
                    {
                        yield return instruction; // call GraphicDatabase.Get
                        enumer.MoveNext();
                        yield return enumer.Current; // stloc.2
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldloc_2);
                        yield return new CodeInstruction(OpCodes.Call, _processGraphicMethod);
                        continue;
                    }

                    yield return instruction;
                }
                
            }
            
            internal static Shader SelectShader(Apparel apparel)
            {
                if (apparel.def.HasModExtension<PaintableExtension>())
                {
                    return ShaderDB.CutoutMultiMask;
                }
                return ShaderDatabase.Cutout;
            }

            internal static void ProcessGraphic(Apparel apparel, Graphic graphic)
            {
                if (!apparel.def.HasModExtension<PaintableExtension>()) return;
                //SetColorMat(graphic.MatNorth);
                //SetColorMat(graphic.MatEast);
                //SetColorMat(graphic.MatSouth);
                //SetColorMat(graphic.MatWest);
                //SetColorMat(graphic.MatSingle);
            }

            internal static void SetColorMat(Material material)
            {
                if (material.shader != ShaderDB.CutoutMultiMask) return;
                material.SetColor("_Color", Color.cyan);
                material.SetColor("_ColorTwo", Color.magenta);
                material.SetColor("_ColorThree", Color.yellow);

            }
        } 
    }

    internal static class GraphicPatches
    {
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
            public static bool Prefix(Thing t)
            {
                // var extension = thing.def.GetModExtension<PaintableExtension>();
                // if(extension != null)
                // {
                //     TLog.Message($"[RWPaintingTool] - Found paintable thing: {thing.def.label}");
                //     GraphicInitPatch.CurThing = thing;
                //     return false;
                // }
                GraphicInitPatch.CurThing = t;
                return true;
            }
            
            public static void Postfix()
            {
                GraphicInitPatch.CurThing = null;
            }
        }
        
        [HarmonyPatch(typeof(Graphic), nameof(Graphic.Init))]
        internal static class GraphicInitPatch
        {
            internal static Thing? CurThing = null;
            
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
                if (GraphicInitPatch.CurThing != null && req.shader.SupportsMultiColor())
                {
                    var tracker = ColorTrackerDB.GetTracker(GraphicInitPatch.CurThing);
                    var maskTracker = ColorTrackerDB.GetMaskTracker(GraphicInitPatch.CurThing);
                    tracker.SetColorsOn(__result);
                    TLog.Debug("Getting material data for: " + GraphicInitPatch.CurThing.def + " it would receive mask: " + maskTracker.CurMaskID);
                    //__result.SetTexture("_Mask", MaskManager.GetMask(GraphicInitPatch.CurThing.def, maskTracker.CurMaskID));
                    __result.SetColor(ColorThree, tracker.ColorThree);
                    __result.SetColor(ColorFour, tracker.ColorFour);
                    __result.SetColor(ColorFive, tracker.ColorFive);
                    __result.SetColor(ColorSix, tracker.ColorSix);
                }
            }
        }

        internal static class PrepareGraphicPatchClass
        {
            //internal static MethodDefinition? _methodDef;
            //internal static MethodBase? _method = AccessTools.Method(typeof(PrepareGraphicPatchClass), nameof(TestPatchCall));
            internal static int calls = 0;
            
            //[FreePatch]
            internal static void GetType(ModuleDefinition module)
            {
                //var type = module.GetType(nameof(Verse),nameof(GraphicDatabase));
               //_methodDef = type.FindMethod(nameof(GraphicDatabase.GetInner));
                //_methodDef.body.instructions.Insert(0, new Instruction(Mono.Cecil.Cil.OpCodes.Call, _method));
                calls++;
            }

            private static void TestPatchCall()
            {
                TLog.Debug("Freepatched a prepatch to get inner!");
            }
        }
    }
}