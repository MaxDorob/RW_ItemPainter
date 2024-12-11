using RimWorld;
using Verse;

namespace RWPaintingTool;


[StaticConstructorOnStartup]
public class DefInjector_MaskData
{
    static DefInjector_MaskData()
    {
        foreach (var def in DefDatabase<ThingDef>.AllDefs)
        {
            MaskManager.CacheMasks(def);
        }
        MaskManager.Resolve();
    }
}