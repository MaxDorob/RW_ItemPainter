using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RWPaintingTool;

internal static class DebugPatches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    public static class Pawn_GetGizmosPatch
    {
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance is { equipment: { } tracker, apparel: { } apparel })
            {
                var gizmos = __result.ToList();
                __result = gizmos.Append(new Command_Action()
                {
                    defaultLabel = "Debug",
                    action = () =>
                    {
                        var thing = apparel.WornApparel.Find(a => a.def.HasModExtension<PaintableExtension>());
                        if (thing == null) return;
                        Find.WindowStack.Add(new Window_ThingColoring(thing));
                    }
                });
            }
        }
    }
}