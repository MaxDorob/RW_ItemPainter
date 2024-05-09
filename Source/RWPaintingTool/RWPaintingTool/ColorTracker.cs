using System.Collections.Generic;
using TeleCore.Events;
using TeleCore.Events.Args;
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
        GlobalEventHandler.Things.Despawned += OnThingDeSpawned;
    }

    private void OnThingSpawned(ThingStateChangedEventArgs args)
    {
        var thing = args.Thing;
        if (_trackers.ContainsKey(thing)) return;

        var tracker = new ColorTracker(thing);
        _trackers.Add(thing, tracker);
    }

    private void OnThingDeSpawned(ThingStateChangedEventArgs args)
    {
        var thing = args.Thing;
        if (!_trackers.ContainsKey(thing)) return;

        _trackers.Remove(thing);
    }
    
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref _trackers, "trackers", LookMode.Reference, LookMode.Deep);
    }
}

public class ColorTracker : IExposable
{
    private Thing _thing;

    private Color _one;
    private Color _two;
    private Color _three;
    private Color _four;
    private Color _five;
    private Color _six;

    
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
        Scribe_Values.Look(ref _one, "colorOne");
        Scribe_Values.Look(ref _two, "colorTwo");
        Scribe_Values.Look(ref _three, "colorThree");
        Scribe_Values.Look(ref _four, "colorFour");
        Scribe_Values.Look(ref _five, "colorFive");
        Scribe_Values.Look(ref _six, "colorSix");
    }

    public void Notify_ColorsChanged()
    {
        _thing.Notify_ColorChanged();
    }
}