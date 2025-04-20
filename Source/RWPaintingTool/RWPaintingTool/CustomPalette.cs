using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWPaintingTool
{
    public class CustomPalette : IPalette, IExposable
    {
        private SixColorSet palette;
        private string label;
        public SixColorSet Palette
        {
            get => palette;
            set => palette = value;
        }
        public string Label
        {
            get=> label;
            set => label = value;
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref palette.colorOne, nameof(palette.colorOne));
            Scribe_Values.Look(ref palette.colorTwo, nameof(palette.colorTwo));
            Scribe_Values.Look(ref palette.colorThree, nameof(palette.colorThree));
            Scribe_Values.Look(ref palette.colorFour, nameof(palette.colorFour));
            Scribe_Values.Look(ref palette.colorFive, nameof(palette.colorFive));
            Scribe_Values.Look(ref palette.colorSix, nameof(palette.colorSix));
            Scribe_Values.Look(ref label, nameof(label));
        }
    }
}
