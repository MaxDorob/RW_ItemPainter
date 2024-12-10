using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RWPaintingTool
{
    internal static class UIUtils
    {
        public static bool DrawTextureWithSlider(Rect rect, Texture texture, Material material, ref float sliderVal, float min = 0f, float max = 1f, Color? sliderColor = null)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
            float num = sliderVal;
            Vector2 mousePosition = Event.current.mousePosition;
            EventType typeForControl = Event.current.GetTypeForControl(controlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (rect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        GUIUtility.hotControl = controlID;
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        float value = mousePosition.x - rect.x;
                        float t = Mathf.InverseLerp(0f, rect.width, value);
                        sliderVal = Mathf.Lerp(min, max, t);
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
                default:
                    if (typeForControl == EventType.Repaint)
                    {
                        float num2 = rect.x + rect.width * Mathf.InverseLerp(min, max, sliderVal);
                        Rect rect2 = new Rect(num2 - 1f, rect.y - 1f, 3f, rect.height + 2f);
                        Rect rect3 = rect.ExpandedBy(2f);
                        Color color = (GUIUtility.hotControl == controlID | rect3.Contains(mousePosition)) ? Color.white : Color.grey;
                        Widgets.DrawBoxSolid(rect.ExpandedBy(1f), color);
                        Graphics.DrawTexture(rect, texture, 0, 0, 0, 0, material);
                        Widgets.DrawBoxSolid(rect2, Color.white);
                    }
                    break;
            }
            return Math.Abs(num - sliderVal) > float.Epsilon;
        }
        public static bool TextFieldNumeric<T>(Rect rect, ref T value, float min = 0f, float max = 3.4028235E+38f) where T : struct
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
            defaultInterpolatedStringHandler.AppendLiteral("TextField");
            defaultInterpolatedStringHandler.AppendFormatted<float>(rect.y, "F0");
            defaultInterpolatedStringHandler.AppendFormatted<float>(rect.x, "F0");
            string text = defaultInterpolatedStringHandler.ToStringAndClear();
            string text2 = value.ToString();
            bool result;

            T value2 = value;
            string text3 = Widgets.TextField(rect, text2);

            result = text2 != text3;
            if (text3 != text2 && Widgets.IsPartiallyOrFullyTypedNumber<T>(ref value, text3, min, max))
            {
                text2 = text3;
                if (text3.IsFullyTypedNumber<T>() || text2.Length == 0)
                {
                    Widgets.ResolveParseNow<T>(text3, ref value, ref text2, min, max, false);
                }
            }



            return result;
        }
        private static Regex HEXREGEX = new Regex("^#([0-9A-Fa-f]{0,6})$");
        public static bool TextFieldHex(Rect rect, out string hexResult, string initialValue = null)
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 3);
            defaultInterpolatedStringHandler.AppendLiteral("TextFieldHex");
            defaultInterpolatedStringHandler.AppendFormatted<float>(rect.y, "F0");
            defaultInterpolatedStringHandler.AppendFormatted<float>(rect.x, "F0");
            string text = defaultInterpolatedStringHandler.ToStringAndClear();
            bool result;
            string buffer = initialValue ?? "FFFFFF";
            string input = GUI.TextField(rect, "#" + buffer, 7);
            Match match = HEXREGEX.Match(input);
            string text2 = buffer;
            if (match.Success)
            {
                text2 = match.Groups[1].Value;
            }
            hexResult = text2;
            result = (hexResult != buffer);

            return result;
        }
    }
}
