using HarmonyLib;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public sealed class PaintingToolMod : Mod
{
    private readonly Harmony _harmony;

    public static PaintingToolMod Mod { get; private set; }
    
    public PaintingToolMod(ModContentPack content) : base(content)
    {
        Mod = this;
        ParseHelper.Parsers<SixColorSet>.Register(SixColorSet.FromString);
        _harmony = new Harmony("telefonmast.paintingtool");
        _harmony.PatchAll();
        
    }

}