using UnityEngine;

namespace RWPaintingTool;

public struct SixColorSet
{
    public Color ColorOne { get; set; }
    public Color ColorTwo { get; set; }
    public Color ColorThree { get; set; }
    public Color ColorFour { get; set; }
    public Color ColorFive { get; set; }
    public Color ColorSix { get; set; }

    public Color this[int colorIndex]
    {
        get
        {
            return colorIndex switch
            {
                0 => ColorOne,
                1 => ColorTwo,
                2 => ColorThree,
                3 => ColorFour,
                4 => ColorFive,
                5 => ColorSix,
                _ => throw new System.ArgumentOutOfRangeException(nameof(colorIndex), colorIndex.ToString())
            };
        }
        set
        {
            _ = colorIndex switch
            {
                0 => ColorOne = value,
                1 => ColorTwo = value,
                2 => ColorThree = value,
                3 => ColorFour = value,
                4 => ColorFive = value,
                5 => ColorSix = value,
                _ => throw new System.ArgumentOutOfRangeException(nameof(colorIndex), colorIndex.ToString())
            };
        }
    }
}