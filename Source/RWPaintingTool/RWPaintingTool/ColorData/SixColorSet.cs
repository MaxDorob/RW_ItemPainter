﻿using System.Globalization;
using System;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using Verse;
using System.Collections.Generic;

namespace RWPaintingTool;

public struct SixColorSet(Color one, Color two, Color three, Color four, Color five, Color six)
{
    public Color colorOne = one;
    public Color colorTwo = two;
    public Color colorThree = three;
    public Color colorFour = four;
    public Color colorFive = five;
    public Color colorSix = six;

    public Color this[int colorIndex]
    {
        get
        {
            return colorIndex switch
            {
                0 => colorOne,
                1 => colorTwo,
                2 => colorThree,
                3 => colorFour,
                4 => colorFive,
                5 => colorSix,
                _ => throw new System.ArgumentOutOfRangeException(nameof(colorIndex), colorIndex.ToString())
            };
        }
        set
        {
            _ = colorIndex switch
            {
                0 => colorOne = value,
                1 => colorTwo = value,
                2 => colorThree = value,
                3 => colorFour = value,
                4 => colorFive = value,
                5 => colorSix = value,
                _ => throw new System.ArgumentOutOfRangeException(nameof(colorIndex), colorIndex.ToString())
            };
        }
    }
    internal static SixColorSet FromString(string Str)
    {

        var regex = new Regex(@"(?<color>\(\s*\d+\s*,\s*\d+\s*,\s*\d+\s*\))");
        List<Color> parsedValues = new List<Color>();
        foreach (Match match in regex.Matches(Str))
        {
            parsedValues.Add(ParseHelper.ParseColor(match.Value));

        }
        return new SixColorSet(
            parsedValues.ElementAtOrDefault(0),
            parsedValues.ElementAtOrDefault(1),
            parsedValues.ElementAtOrDefault(2),
            parsedValues.ElementAtOrDefault(3),
            parsedValues.ElementAtOrDefault(4),
            parsedValues.ElementAtOrDefault(5)
            );
    }
}