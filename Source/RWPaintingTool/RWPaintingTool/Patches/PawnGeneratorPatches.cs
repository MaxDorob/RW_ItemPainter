using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWPaintingTool.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GenerateGearFor))]
    internal static class PawnGeneratorPatches
    {
        public static void Postfix(Pawn pawn)
        {
            if (pawn?.apparel?.WornApparel == null)
            {
                return;
            }

            var pawnPalette = pawn.kindDef?.GetModExtension<AvailablePalettes_ModExtension>();
            if (pawnPalette == null)
            {
                return;
            }

            foreach (var apparel in pawn.apparel.WornApparel)
            {
                var colorTracker = ColorTrackerDB.GetTracker(apparel);
                if (colorTracker != null)
                {
                    if (pawnPalette.pawnIdAsSeed)
                    {
                        colorTracker.ColorSet = pawnPalette.palettes[Rand.RangeSeeded(0, pawnPalette.palettes.Count, pawn.thingIDNumber)];
                    }
                    else
                    {
                        colorTracker.ColorSet = pawnPalette.palettes.RandomElement();
                    }
                }
            }
        }
    }
}
