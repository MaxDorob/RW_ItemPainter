using Verse;

namespace RWPaintingTool;

public class GameComponent_ColorTracking : GameComponent
{
    public GameComponent_ColorTracking(Game game)
    {
    }

    public override void ExposeData()
    {
        base.ExposeData();
        ColorTrackerDB.ExposeData();
    }
}