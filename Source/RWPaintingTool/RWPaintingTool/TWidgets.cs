using System;
using TeleCore.Shared;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

//TODO: Push into Telecore
public static class TWidgets
{
    public static void TextFieldNumeric<T>(Rect rect, ref T value, ref string? buffer, float min = 0, float max = float.MaxValue) where T : unmanaged
    {
        if (buffer == null)
        {
            buffer = value.ToString();
        }

        var ctrlName = "TextField" + rect.y.ToString("F0") + rect.x.ToString("F0");
        using var focus = new GUIFocus(rect, ctrlName);
        var fieldValue = TextField(rect, buffer);
        
        if (GUI.GetNameOfFocusedControl() != ctrlName)
        {
            Widgets.ResolveParseNow<T>(buffer, ref val, ref buffer, min, max, true);
            return;
        }
        
        if (fieldValue != buffer && Widgets.IsPartiallyOrFullyTypedNumber<T>(ref val, fieldValue, min, max))
        {
            buffer = fieldValue;
            if (fieldValue.IsFullyTypedNumber<T>())
            {
                Widgets.ResolveParseNow<T>(fieldValue, ref val, ref buffer, min, max, false);
            }
        }
    }
    
    
    
    public static string TextField(Rect rect, string? text)
    {
        text ??= "";
        
        using var context = new GUIStyleContext(GUI.skin.textField, true);
        var result = GUI.TextField(rect, text, context.Style);
        
        return result;
    }

    #region Grouping

    public static void BeginGroup(Rect position)
    {
        BeginGroup(position, GUIContent.none, GUIStyle.none);
    }

    public static void BeginGroup(Rect position, string text)
    {
        BeginGroup(position, text, GUIStyle.none);
    }

    public static void BeginGroup(Rect position, Texture image)
    {
        BeginGroup(position, image, GUIStyle.none);
    }

    public static void BeginGroup(Rect position, GUIContent content)
    {
        BeginGroup(position, content, GUIStyle.none);
    }

    public static void BeginGroup(Rect position, GUIStyle style)
    {
        BeginGroup(position, GUIContent.none, style);
    }

    public static void BeginGroup(Rect position, string text, GUIStyle style)
    {
        GUI.BeginGroup(position, text, style);
        UnityGUIBugsFixer.Notify_BeginGroup();
    }

    public static void BeginGroup(Rect position, Texture image, GUIStyle style)
    {
        GUI.BeginGroup(position, image, style);
        UnityGUIBugsFixer.Notify_BeginGroup();
    }

    public static void BeginGroup(Rect position, GUIContent content, GUIStyle style)
    {
        GUI.BeginGroup(position, content, style);
        UnityGUIBugsFixer.Notify_BeginGroup();
    }
    
    public static void EndGroup()
    {
        GUI.EndGroup();
        UnityGUIBugsFixer.Notify_EndGroup();
    }
    
    #endregion
}

public class GUIStyleContext : IDisposable
{
    private GUIStyle _style;
    private Font _font;
    private TextAnchor _alignment;
    private bool _wordWrap;

    public GUIStyle Style => _style;

    public GUIStyleContext(GUIStyle style, bool setTextStyle) : this(style)
    {
        if (setTextStyle)
        {
            _style.font = Text.CurTextFieldStyle.font;
            _style.alignment = Text.CurTextFieldStyle.alignment;
            _style.wordWrap = Text.CurTextFieldStyle.wordWrap;
        }
    }

    public GUIStyleContext(GUIStyle style) 
    {
        _style = style;
        _font = style.font;
        _alignment = style.alignment;
        _wordWrap = style.wordWrap;
    }
    
    public void Dispose()
    {
        _style.font = _font;
        _style.alignment = _alignment;
        _style.wordWrap = _wordWrap;
    }
}

public class GUIFocus : IDisposable
{
    private Rect focusRect;
    private string controlName;

    public GUIFocus(Rect focusRect, string controlName)
    {
        this.focusRect = focusRect;
        this.controlName = controlName;
        GUI.SetNextControlName(controlName);
    }
    
    public void Dispose()
    {
        var clicked = Event.current.isMouse && Event.current.button == 0;
        if (clicked && !focusRect.Contains(Event.current.mousePosition))
        {
            GUI.FocusControl(null);
        }
    }
}