using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace RWPaintingTool;
[HarmonyPatch]
internal static class ColorTrackerDB
{
    private static Dictionary<int, ColorTracker> _trackers = new();
    private static Dictionary<int, MaskTracker> _masks = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Thing), nameof(Thing.Discard))]

    private static void OnThingDiscarded(Thing __instance)
    {
        var id = __instance.thingIDNumber;
        _trackers.Remove(id);
        _masks.Remove(id);
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Thing), nameof(Thing.PostMake))]
    private static void OnThingSpawned(Thing __instance)
    {
        var thing = __instance;
        var id = thing.thingIDNumber;
        var data = thing.def.GetModExtension<PaintableExtension>();
        if(data == null) return;
        if (_trackers.ContainsKey(id)) return;
#if DEBUG
        Log.Warning("Registering tracker for thing: " + thing);
#endif
        _trackers[id] = new ColorTracker(thing, data);
        _masks[id] = new MaskTracker(thing);
    }
    
    internal static void ExposeData()
    {
        Scribe_Collections.Look(ref _trackers, "trackers", LookMode.Value, LookMode.Deep);
        Scribe_Collections.Look(ref _masks, "masks", LookMode.Value, LookMode.Deep);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorTracker? GetTracker(Thing thing) => _trackers.GetValueOrDefault(thing.thingIDNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MaskTracker? GetMaskTracker(Thing thing) => _masks.GetValueOrDefault(thing.thingIDNumber);
}