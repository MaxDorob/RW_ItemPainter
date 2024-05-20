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
            switch (colorIndex)
            {
                case 0:
                    ColorOne = value;
                    break;
                case 1:
                    ColorTwo = value;
                    break;
                case 2:
                    ColorThree = value;
                    break;
                case 3:
                    ColorFour = value;
                    break;
                case 4:
                    ColorFive = value;
                    break;
                case 5:
                    ColorSix = value;
                    break;
            }
        }
    }
}