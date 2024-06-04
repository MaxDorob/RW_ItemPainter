using HarmonyLib;
using TeleCore.Loader;
using Verse;

namespace RWPaintingTool;

public sealed class PaintingToolMod : Mod
{
    private readonly Harmony _harmony;

    public static PaintingToolMod Mod { get; private set; }
    
    public PaintingToolMod(ModContentPack content) : base(content)
    {
        Mod = this;
        _harmony = new Harmony("telefonmast.paintingtool");
        _harmony.PatchAll();
        
        TLog.Debug($"Initialized PaintingToolMod");
    }
}