using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWPaintingTool
{
    public class ColorPicker
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000007 RID: 7 RVA: 0x000020A5 File Offset: 0x000002A5
        // (set) Token: 0x06000008 RID: 8 RVA: 0x000020AD File Offset: 0x000002AD
        public Color Color
        {
            get
            {
                return this._color;
            }
            private set
            {
                this._color = value;
                Action<Color> colorChanged = this.ColorChanged;
                if (colorChanged == null)
                {
                    return;
                }
                colorChanged(value);
            }
        }

        // Token: 0x14000001 RID: 1
        // (add) Token: 0x06000009 RID: 9 RVA: 0x000020C8 File Offset: 0x000002C8
        // (remove) Token: 0x0600000A RID: 10 RVA: 0x00002100 File Offset: 0x00000300
        public event Action<Color> ColorChanged;

        // Token: 0x0600000B RID: 11 RVA: 0x00002138 File Offset: 0x00000338
        public ColorPicker()
        {
            ColorPicker.style = new GUIStyle(GUI.skin.textField);
            ColorPicker.style.normal.background = (ColorPicker.style.active.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom", true));
            ColorPicker.style.hover.background = (ColorPicker.style.focused.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom_Sel", true));
            ColorPicker.style.border = new RectOffset(2, 2, 2, 2);
            ColorPicker.sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
            GUIStyleState normal = ColorPicker.sliderStyle.normal;
            normal.background = null;
            ColorPicker.sliderStyle.normal = (ColorPicker.sliderStyle.active = (ColorPicker.sliderStyle.focused = (ColorPicker.sliderStyle.hover = normal)));
            ColorPicker.sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
            Texture2D background = ContentFinder<Texture2D>.Get("HorizontalSliderThumb", true);
            ColorPicker.sliderThumbStyle.normal.background = (ColorPicker.sliderThumbStyle.active.background = (ColorPicker.sliderThumbStyle.focused.background = (ColorPicker.sliderThumbStyle.hover.background = background)));
            this.colorPickerMat = new Material(ShaderDatabase.LoadShader("ColorPickerGradient"));
            this.hueMat = new Material(ShaderDatabase.LoadShader("ColorPickerHue"));
            this.satMat = new Material(ShaderDatabase.LoadShader("ColorPickerHue"));
            this.valMat = new Material(ShaderDatabase.LoadShader("ColorPickerHue"));
            this.redMat = new Material(ShaderDatabase.LoadShader("ColorPickerRGB"));
            this.greenMat = new Material(ShaderDatabase.LoadShader("ColorPickerRGB"));
            this.blueMat = new Material(ShaderDatabase.LoadShader("ColorPickerRGB"));
            this.hueMat.SetInt("_Invert", 1);
            this.satMat.SetInt("_Invert", 1);
            this.valMat.SetInt("_Invert", 1);
            this.satMat.SetInt("_Mode", 1);
            this.valMat.SetInt("_Mode", 2);
            this.greenMat.SetInt("_Result", 1);
            this.blueMat.SetInt("_Result", 2);
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000C RID: 12 RVA: 0x0000239A File Offset: 0x0000059A
        public static Vector2 DefaultSize
        {
            get
            {
                return ColorPicker._defaultSize;
            }
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000023A4 File Offset: 0x000005A4
        static ColorPicker()
        {
            float num;
            float num2;
            ColorPicker._defaultSize = ColorPicker.GetSize(out num, out num2, 0f);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x000023C4 File Offset: 0x000005C4
        private static Vector2 GetSize(out float sizeSquare, out float selectorWidth, float widthOverride = 0f)
        {
            Vector2 vector = Text.CalcSize("H:");
            Vector2 vector2 = Text.CalcSize("1000");
            sizeSquare = vector.y * 8f + 10f + 35f;
            selectorWidth = Mathf.Max(vector.x + vector2.x * 3f + 5f + vector2.x, widthOverride);
            float x = sizeSquare + 10f + selectorWidth;
            return new Vector2(x, sizeSquare);
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00002440 File Offset: 0x00000640
        private static string ToHex(Color color)
        {
            int num = (int)(color.r * 255f);
            int num2 = (int)(color.g * 255f);
            int num3 = (int)(color.b * 255f);
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 3);
            defaultInterpolatedStringHandler.AppendFormatted<int>(num, "X2");
            defaultInterpolatedStringHandler.AppendFormatted<int>(num2, "X2");
            defaultInterpolatedStringHandler.AppendFormatted<int>(num3, "X2");
            return defaultInterpolatedStringHandler.ToStringAndClear();
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000024B0 File Offset: 0x000006B0
        private static Color HexToColor(string hex)
        {
            Color result;
            ColorUtility.TryParseHtmlString("#" + hex, out result);
            return result;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x000024D4 File Offset: 0x000006D4
        public Rect Draw(Vector2 pos, float extraWidth = 0f)
        {
            Vector2 vector = Text.CalcSize("H:");
            float y = vector.y;
            float num;
            float width;
            Vector2 size = ColorPicker.GetSize(out num, out width, extraWidth);
            Rect inRect = new Rect(pos, new Vector2(num, num));
            Rect parent = new Rect(pos.x + num + 10f, pos.y, width, num);
            Rect result = new Rect(pos, new Vector2(size.x, size.y));
            RectDivider rectDivider = new RectDivider(parent, this.GetHashCode(), new Vector2?(new Vector2(5f, 5f)));
            RectDivider rect = rectDivider.NewRow(y, VerticalJustification.Top, null);
            RectDivider rect2 = rectDivider.NewRow(y, VerticalJustification.Top, null);
            RectDivider rect3 = rectDivider.NewRow(y, VerticalJustification.Top, null);
            RectDivider rect4 = rectDivider.NewRow(y, VerticalJustification.Top, new float?((float)10));
            float x = Text.CalcSize("1000").x;
            float x2 = Text.CalcSize("#FFFFFF").x;
            RectDivider rect5 = rect2.NewCol(x / 2f, HorizontalJustification.Left, null);
            RectDivider rect6 = rect3.NewCol(x / 2f, HorizontalJustification.Left, null);
            RectDivider rect7 = rect4.NewCol(x / 2f, HorizontalJustification.Left, null);
            RectDivider rect8 = rect2.NewCol(x, HorizontalJustification.Right, new float?((float)5));
            RectDivider rect9 = rect3.NewCol(x, HorizontalJustification.Right, new float?((float)5));
            RectDivider rect10 = rect4.NewCol(x, HorizontalJustification.Right, new float?((float)5));
            RectDivider rect11 = rectDivider.NewRow(y, VerticalJustification.Top, null);
            RectDivider rect12 = rectDivider.NewRow(y, VerticalJustification.Top, null);
            RectDivider rect13 = rectDivider.NewRow(y, VerticalJustification.Top, null);
            RectDivider rect14 = rectDivider.NewRow(y, VerticalJustification.Top, null);
            RectDivider rect15 = rect11.NewCol(x / 2f, HorizontalJustification.Left, null);
            RectDivider rect16 = rect12.NewCol(x / 2f, HorizontalJustification.Left, null);
            RectDivider rect17 = rect13.NewCol(x / 2f, HorizontalJustification.Left, null);
            RectDivider rect18 = rect11.NewCol(x, HorizontalJustification.Right, new float?((float)5));
            RectDivider rect19 = rect12.NewCol(x, HorizontalJustification.Right, new float?((float)5));
            RectDivider rect20 = rect13.NewCol(x, HorizontalJustification.Right, new float?((float)5));
            RectDivider rect21 = rect14.NewCol(x2, HorizontalJustification.Right, new float?((float)5));
            Vector2 uvPos = new Vector2(this._HSVRaw.y, 1f - this._HSVRaw.z);
            Color color = this._color;
            Color value = Color.HSVToRGB(this._HSVRaw.x, 1f, 1f, true);
            this.colorPickerMat.SetFloat("_Hue", this._HSVRaw.x);
            this.redMat.SetColor("_Color", color);
            this.greenMat.SetColor("_Color", color);
            this.blueMat.SetColor("_Color", color);
            this.satMat.SetColor("_Color", value);
            this.valMat.SetColor("_Color", value);
            this.OnSelectingInPicker(inRect, uvPos, color);
            Widgets.Label(rect15, "R:");
            Widgets.Label(rect16, "G:");
            Widgets.Label(rect17, "B:");
            Widgets.Label(rect14, "HEX:");
            int num2 = this._colorInt.r;
            int num3 = this._colorInt.g;
            int num4 = this._colorInt.b;
            float num5 = (float)num2;
            float num6 = (float)num3;
            float num7 = (float)num4;
            bool flag =  UIUtils.DrawTextureWithSlider(rect11, BaseContent.BadTex, this.redMat, ref num5, 0f, 255f, null);
            bool flag2 = UIUtils.DrawTextureWithSlider(rect12, BaseContent.BadTex, this.greenMat, ref num6, 0f, 255f, null);
            bool flag3 = UIUtils.DrawTextureWithSlider(rect13, BaseContent.BadTex, this.blueMat, ref num7, 0f, 255f, null);
            if (flag || flag2 || flag3)
            {
                num2 = (int)num5;
                num3 = (int)num6;
                num4 = (int)num7;
            }
            GUIStyle textField = GUI.skin.textField;
            GUI.skin.textField = ColorPicker.style;
            bool flag4 = UIUtils.TextFieldNumeric<int>(rect18, ref num2, 0f, 255f);
            bool flag5 = UIUtils.TextFieldNumeric<int>(rect19, ref num3, 0f, 255f);
            bool flag6 = UIUtils.TextFieldNumeric<int>(rect20, ref num4, 0f, 255f);
            string initialValue = ColorPicker.ToHex(color);
            string hex;
            bool flag7 = UIUtils.TextFieldHex(rect21, out hex, initialValue);
            if (flag7)
            {
                Color color2 = ColorPicker.HexToColor(hex);
                this.SetColors(color2);
                Action<Color> colorChanged = this.ColorChanged;
                if (colorChanged != null)
                {
                    colorChanged(color2);
                }
            }
            if (flag4 || flag5 || flag6)
            {
                this.RGBChanged(new ColorInt(num2, num3, num4));
            }
            Widgets.Label(rect, "HSV - RGB - HEX");
            Widgets.Label(rect5, "H:");
            Widgets.Label(rect6, "S:");
            Widgets.Label(rect7, "V:");
            int num8 = this._HSVInt.x;
            int num9 = this._HSVInt.y;
            int num10 = this._HSVInt.z;
            float num11 = (float)num8;
            float num12 = (float)num9;
            float num13 = (float)num10;
            bool flag8 = UIUtils.DrawTextureWithSlider(rect2, BaseContent.BadTex, this.hueMat, ref num11, 0f, 360f, null);
            bool flag9 = UIUtils.DrawTextureWithSlider(rect3, BaseContent.BadTex, this.satMat, ref num12, 0f, 100f, null);
            bool flag10 = UIUtils.DrawTextureWithSlider(rect4, BaseContent.BadTex, this.valMat, ref num13, 0f, 100f, null);
            if (flag8 || flag9 || flag10)
            {
                num8 = (int)num11;
                num9 = (int)num12;
                num10 = (int)num13;
            }
            bool flag11 = UIUtils.TextFieldNumeric<int>(rect8, ref num8, 0f, 360f);
            bool flag12 = UIUtils.TextFieldNumeric<int>(rect9, ref num9, 0f, 100f);
            bool flag13 = UIUtils.TextFieldNumeric<int>(rect10, ref num10, 0f, 100f);
            GUI.skin.textField = textField;
            if (flag11 || flag12 || flag13)
            {
                this.HSVChanged(num8, num9, num10);
            }
            return result;
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002BFC File Offset: 0x00000DFC
        private void OnSelectingInPicker(Rect inRect, Vector2 uvPos, Color color)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive, inRect);
            EventType typeForControl = Event.current.GetTypeForControl(controlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (inRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        GUIUtility.hotControl = controlID;
                        return;
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        return;
                    }
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 vector = new Vector2(Event.current.mousePosition.x - inRect.x, Event.current.mousePosition.y - inRect.y);
                        Vector2 vector2 = new Vector2(vector.x / inRect.width, 1f - vector.y / inRect.height);
                        Vector2Int vector2Int = new Vector2Int((int)(vector2.x * 100f), (int)(vector2.y * 100f));
                        this.HSVChanged(this._HSVInt.x, vector2Int.x, vector2Int.y);
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
                default:
                    if (typeForControl == EventType.Repaint)
                    {
                        Graphics.DrawTexture(inRect, BaseContent.BadTex, 0, 0, 0, 0, this.colorPickerMat);
                        Vector2 vector3 = new Vector2(7f, 7f);
                        Rect rect = new Rect(inRect.position + uvPos * inRect.size - vector3 / 2f, vector3);
                        Widgets.DrawBoxSolidWithOutline(rect, color, Color.black, 1);
                        return;
                    }
                    break;
            }
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002D9C File Offset: 0x00000F9C
        public void SetColors(Color color)
        {
            float num;
            float num2;
            float num3;
            Color.RGBToHSV(color, out num, out num2, out num3);
            this._color = color;
            this._colorInt = new ColorInt(color);
            this._HSVRaw = new Vector3(num, num2, num3);
            this._HSVInt = new Vector3Int((int)(num * 360f), (int)(num2 * 100f), (int)(num3 * 100f));
        }

        // Token: 0x06000014 RID: 20 RVA: 0x00002E04 File Offset: 0x00001004
        private void RGBChanged(ColorInt newColor)
        {
            this.Color = newColor.ToColor;
            this._colorInt = newColor;
            float num;
            float num2;
            float num3;
            Color.RGBToHSV(this._color, out num, out num2, out num3);
            this._HSVRaw = new Vector3(num, num2, num3);
            this._HSVInt = new Vector3Int((int)(num * 360f), (int)(num2 * 100f), (int)(num3 * 100f));
        }

        // Token: 0x06000015 RID: 21 RVA: 0x00002E74 File Offset: 0x00001074
        private void HSVChanged(int newH, int newS, int newV)
        {
            this._HSVRaw = new Vector3((float)newH / 360f, (float)newS / 100f, (float)newV / 100f);
            this._HSVInt = new Vector3Int(newH, newS, newV);
            this.Color = Color.HSVToRGB(this._HSVRaw.x, this._HSVRaw.y, this._HSVRaw.z);
            this._colorInt = new ColorInt(this._color);
        }

        // Token: 0x04000004 RID: 4
        private Material colorPickerMat;

        // Token: 0x04000005 RID: 5
        private Material hueMat;

        // Token: 0x04000006 RID: 6
        private Material satMat;

        // Token: 0x04000007 RID: 7
        private Material valMat;

        // Token: 0x04000008 RID: 8
        private Material redMat;

        // Token: 0x04000009 RID: 9
        private Material greenMat;

        // Token: 0x0400000A RID: 10
        private Material blueMat;

        // Token: 0x0400000B RID: 11
        private Color _color;

        // Token: 0x0400000C RID: 12
        private ColorInt _colorInt;

        // Token: 0x0400000D RID: 13
        private Vector3 _HSVRaw;

        // Token: 0x0400000E RID: 14
        private Vector3Int _HSVInt;

        // Token: 0x0400000F RID: 15
        private static GUIStyle style;

        // Token: 0x04000010 RID: 16
        private static GUIStyle sliderStyle;

        // Token: 0x04000011 RID: 17
        private static GUIStyle sliderThumbStyle;

        // Token: 0x04000012 RID: 18
        private const string ContextSpace = "TeleColorPicker";

        // Token: 0x04000014 RID: 20
        private const float padding = 5f;

        // Token: 0x04000015 RID: 21
        private const float mid_divide = 10f;

        // Token: 0x04000016 RID: 22
        private static Vector2 _defaultSize;

        // Token: 0x04000017 RID: 23
        private const string string_H = "H:";

        // Token: 0x04000018 RID: 24
        private const string string_S = "S:";

        // Token: 0x04000019 RID: 25
        private const string string_V = "V:";

        // Token: 0x0400001A RID: 26
        private const string string_R = "R:";

        // Token: 0x0400001B RID: 27
        private const string string_G = "G:";

        // Token: 0x0400001C RID: 28
        private const string string_B = "B:";
    }
}
