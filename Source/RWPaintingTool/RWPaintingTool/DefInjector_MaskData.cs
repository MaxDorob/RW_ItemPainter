using TeleCore.Loader;
using Verse;

namespace RWPaintingTool;

public class DefInjector_MaskData : DefInjectBase
{
    public override void OnThingDefInject(ThingDef thingDef)
    {
        MaskManager.CacheMasks(thingDef);
    }

    public override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        MaskManager.Resolve();
    }
}