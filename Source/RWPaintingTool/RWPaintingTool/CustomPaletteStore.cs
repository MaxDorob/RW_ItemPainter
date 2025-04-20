using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWPaintingTool
{
    public class CustomPaletteStore : GameComponent
    {
        public CustomPaletteStore(Game game) { }
        public CustomPaletteStore() { }
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
        }
    }
}
