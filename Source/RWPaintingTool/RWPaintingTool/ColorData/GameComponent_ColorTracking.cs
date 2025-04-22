using System.Collections.Generic;
using Verse;

namespace RWPaintingTool;

public class GameComponent_ColorTracking : GameComponent
{
    public GameComponent_ColorTracking(Game game)
    {
    }

    public GameComponent_ColorTracking() { }
    public List<CustomPalette> customPalettes = new List<CustomPalette>();
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref customPalettes, nameof(customPalettes));
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            if (customPalettes == null)
            {
                Log.Message("customPalettes is null");
                customPalettes = new List<CustomPalette>();
            }
        }
        ColorTrackerDB.ExposeData();
    }
}