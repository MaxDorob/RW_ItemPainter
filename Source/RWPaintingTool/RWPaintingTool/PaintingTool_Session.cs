using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool
{
    private Pawn? _carryingPawn;
    private Thing _thing;

    //
    private SixColorSet _colorsSet;
    private int _curColorIndex = 0;
    private int _highLightedIndex = -1;
    
    public bool IsPawn => _carryingPawn != null;
    
    
    private void Notify_ColorChanged(Color color, int index)
    {
        _colorsSet[index] = color;
    }
}