using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TeleCore.Loader;
using TeleCore.Shared;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

internal static class GraphicsPatches
{
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
                        TLog.Debug("Injecting shader selection");
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, _selectShaderMethod).WithLabels(instruction.labels);
                        continue;
                    }

                    if (instruction.opcode == OpCodes.Call && instruction.operand == AccessTools.Method(typeof(GraphicDatabase), nameof(GraphicDatabase.Get),
                            new[] { typeof(string), typeof(Shader), typeof(Vector2), typeof(Color) },
                            new[] { typeof(Graphic_Multi) }))
                    {
                        TLog.Debug("Injecting graphic edit");
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
                    TLog.Message($"[RWPaintingTool] - Found paintable apparel: {apparel.def.label}");
                    return ShaderDB.CutoutMultiMask;
                }
                return ShaderDatabase.Cutout;
            }

            internal static void ProcessGraphic(Apparel apparel, Graphic graphic)
            {
                if (!apparel.def.HasModExtension<PaintableExtension>()) return;
                TLog.Debug("Changing colors");
                SetColorMat(graphic.MatNorth);
                SetColorMat(graphic.MatEast);
                SetColorMat(graphic.MatSouth);
                SetColorMat(graphic.MatWest);
                SetColorMat(graphic.MatSingle);
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

    internal static class GraphicDataPatches
    {
        [HarmonyPatch(typeof(GraphicData), nameof(GraphicData.GraphicColoredFor))]
        internal static class GraphicColoredForPatch
        {
            //TODO: Ensure that unique instances for custom coloring are generated
            public static bool Prefix(Thing thing)
            {
                var extension = thing.def.GetModExtension<PaintableExtension>();
                if(extension != null)
                {
                    TLog.Message($"[RWPaintingTool] - Found paintable thing: {thing.def.label}");
                    return false;
                }

                return true;
            }
        }
    }
}