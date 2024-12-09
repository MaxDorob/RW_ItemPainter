using HarmonyLib;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

internal static class ThingPatches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.DrawColor), MethodType.Getter)]
    internal static class ColorPatch
    {
        public static bool Prefix(Thing __instance, ref Color __result)
        {
            var tracker = ColorTrackerDB.GetTracker(__instance);
            if (tracker == null) return true;
            __result = tracker.ColorOneNullable ?? __result;
            return false;
        }
    }
        
    [HarmonyPatch(typeof(Thing), nameof(Thing.DrawColorTwo), MethodType.Getter)]
    internal static class ColorTwoPatch
    {
        public static bool Prefix(Thing __instance, ref Color __result)
        {
            var tracker = ColorTrackerDB.GetTracker(__instance);
            if (tracker == null) return true;
            __result = tracker.ColorTwoNullable ?? __result;
            return false;
        }
    }
}