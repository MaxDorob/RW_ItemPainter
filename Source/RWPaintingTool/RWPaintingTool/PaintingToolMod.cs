using HarmonyLib;
using Verse;

namespace RWPaintingTool;

public class PaintingToolMod : Mod
{
    private Harmony _harmony;

    public static PaintingToolMod Mod { get; private set; }
        
    public PaintingToolMod(ModContentPack content) : base(content)
    {
        Mod = this;
        _harmony = new Harmony("telefonmast.paintingtool");
        _harmony.PatchAll();
    }
}