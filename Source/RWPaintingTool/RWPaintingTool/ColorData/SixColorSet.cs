using UnityEngine;

namespace RWPaintingTool;

public struct SixColorSet(Color one, Color two, Color three, Color four, Color five, Color six)
{
    public Color ColorOne { get; set; } = one;
    public Color ColorTwo { get; set; } = two;
    public Color ColorThree { get; set; } = three;
    public Color ColorFour { get; set; } = four;
    public Color ColorFive { get; set; } = five;
    public Color ColorSix { get; set; } = six;

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