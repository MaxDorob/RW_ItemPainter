using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TeleCore.Events;
using TeleCore.Events.Args;
using TeleCore.Loader;
using Verse;

namespace RWPaintingTool;

internal static class ColorTrackerDB
{
    private static Dictionary<int, ColorTracker> _trackers = new();
    private static Dictionary<int, MaskTracker> _masks = new();

    static ColorTrackerDB()
    {
        GlobalEventHandler.Things.Spawned += OnThingSpawned;
        GlobalEventHandler.Things.Discarded += OnThingDiscarded;
    }

    private static void OnThingDiscarded(ThingStateChangedEventArgs args)
    {
        var id = args.Thing.thingIDNumber;
        _trackers.Remove(id);
        _masks.Remove(id);
    }

    private static void OnThingSpawned(ThingStateChangedEventArgs args)
    {
        var thing = args.Thing;
        var id = thing.thingIDNumber;
        var data = thing.def.GetModExtension<PaintableExtension>();
        if(data == null) return;
        if (_trackers.ContainsKey(id)) return;
        TLog.Debug("Registering tracker for thing: " + thing);
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