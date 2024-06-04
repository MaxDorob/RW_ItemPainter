using System.Collections.Generic;
using TeleCore.Events;
using TeleCore.Events.Args;
using Verse;

namespace RWPaintingTool;

public static class ColorTrackerDB
{
    private static Dictionary<Thing, ColorTracker> _trackers;
    private static Dictionary<Thing, MaskTracker> _masks;

    static ColorTrackerDB()
    {
        _trackers = new Dictionary<Thing, ColorTracker>();
        _masks = new Dictionary<Thing, MaskTracker>();
        
        GlobalEventHandler.Things.Spawned += OnThingSpawned;
        //GlobalEventHandler.Things.Despawned += OnThingDeSpawned;
        GlobalEventHandler.Things.Discarded += OnThingDiscarded;
        
    }

    private static void OnThingDiscarded(ThingStateChangedEventArgs args)
    {
        var thing = args.Thing;
        if (!_trackers.ContainsKey(thing)) return;
        _trackers.Remove(thing);
    }

    private static void OnThingSpawned(ThingStateChangedEventArgs args)
    {
        var thing = args.Thing;
        if (!_trackers.ContainsKey(thing))
        {
            var tracker = new ColorTracker(thing);
            _trackers.Add(thing, tracker);
        }

        if (!_masks.ContainsKey(thing))
        {
            var mask = new MaskTracker(thing);
            _masks.Add(thing, mask);
        }
    }
    
    internal static void ExposeData()
    {
        Scribe_Collections.Look(ref _trackers, "trackers", LookMode.Reference, LookMode.Deep);
        Scribe_Collections.Look(ref _masks, "trackers", LookMode.Reference, LookMode.Deep);

    }
    
    public static ColorTracker GetTracker(Thing curThing)
    {
        return _trackers.TryGetValue(curThing, out var tracker) ? tracker : null;
    }

    public static MaskTracker GetMaskTracker(Thing thing)
    {
        return _masks.TryGetValue(thing, out var mask) ? mask : null;
    }
}