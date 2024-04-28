using UnityEngine;
using Verse;

namespace RWPaintingTool;

public static class PaintingSession
{
    private static Thing _thing;

    private static Color _colorOne;
    private static Color _colorTwo;
    private static Color _colorThree;
    
    internal static void BeginSession(Thing thing)
    {
        _thing = thing;
    }

    internal static void EndSession()
    {
        
        //Clear
        _colorOne = _colorTwo = _colorThree = Color.white;
    }
}