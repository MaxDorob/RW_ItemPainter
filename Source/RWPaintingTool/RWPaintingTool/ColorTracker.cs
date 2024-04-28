using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public class GameComponent_ColorTracking : GameComponent
{
    private Dictionary<Thing, ColorTracker> _trackers;
    
    public GameComponent_ColorTracking(Game game)
    {
        _trackers = new Dictionary<Thing, ColorTracker>();
        GlobalEventHandler.Things.Spawned += OnThingSpawned;
        GlobalEventHandler.Things.DeSpawned += OnThingDeSpawned;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref _trackers, "trackers", LookMode.Reference, LookMode.Deep);
    }
    
    private void OnThingSpawned(Thing thing)
    {
        if (_trackers.ContainsKey(thing)) return;

        var tracker = new ColorTracker(thing);
        _trackers.Add(thing, tracker);
    }
    
    private void OnThingDeSpawned(Thing thing)
    {
        if (!_trackers.ContainsKey(thing)) return;

        _trackers.Remove(thing);
    }
}

public class ColorTracker : IExposable
{
    private Thing _thing;
    private Color? _colorOneOverride;
    private Color? _colorTwoOverride;
    private Color? _colorThree;

    public ColorTracker(Thing thing)
    {
        _thing = thing;
        _colorThree = Color.white;
    }

    public Color ColorOne => _colorOneOverride ?? _thing.DrawColor;
    public Color ColorTwo => _colorTwoOverride ?? _thing.DrawColorTwo;
    public Color ColorThree => _colorThree ?? Color.white;

    public void ExposeData()
    {
        Scribe_References.Look(ref _thing, "thing");
        Scribe_Values.Look(ref _colorThree, "colorThree");
    }

    public void Notify_ColorsChanged()
    {
        _thing.Notify_ColorChanged();
    }
}