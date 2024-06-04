using UnityEngine;
using Verse;

namespace RWPaintingTool;

public static class UIStyles
{
    private static bool _isReady = false;

    private static GUIStyle _textField;
    
    public static GUIStyle TextFieldStyle
    {
        get
        {
            Setup();
            return _textField;
        }
    }
    
    //
    private static void Setup()
    {
        if (_isReady) return;
        _textField = new GUIStyle(GUI.skin.textField);
        _textField.normal.background = _textField.active.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom");
        _textField.hover.background = _textField.focused.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom_Sel");
        _textField.border = new RectOffset(2, 2, 2, 2);
        _isReady = true;
    }
    
    
}