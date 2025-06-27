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

            if (pawn.kindDef?.GetModExtension<AvailablePalettes_ModExtension>() == null)
            {
                return;
            }

            foreach (var apparel in pawn.apparel.WornApparel)
            {
                var colorTracker = ColorTrackerDB.GetTracker(apparel);
                AvailablePalettes_ModExtension pawnPalette = pawn.kindDef.modExtensions.OfType<AvailablePalettes_ModExtension>().FirstOrDefault(x => x.apparelLayer == null);
                foreach (var layer in apparel.def.apparel.layers)
                {
                    pawnPalette = pawn.kindDef.modExtensions.OfType<AvailablePalettes_ModExtension>().FirstOrDefault(x=>x.apparelLayer == layer) ?? pawnPalette;
                }
                if (colorTracker != null && pawnPalette != null)
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
