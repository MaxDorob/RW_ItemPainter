using UnityEngine;
using Verse;

namespace RWPaintingTool;

[StaticConstructorOnStartup]
public static class UIStyles
{
    public static readonly GUIStyle TextFieldStyle;
    
    static UIStyles()
    {
        TextFieldStyle = new GUIStyle(GUI.skin.textField);
        TextFieldStyle.normal.background = TextFieldStyle.active.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom");
        TextFieldStyle.hover.background = TextFieldStyle.focused.background = ContentFinder<Texture2D>.Get("TextFieldStyleCustom_Sel");
        TextFieldStyle.border = new RectOffset(2, 2, 2, 2);
    }
}